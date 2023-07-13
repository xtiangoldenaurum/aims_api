using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Repositories.Sub;
using aims_api.Utilities;
using aims_api.Utilities.Interface;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class InvAdjustRepository : IInvAdjustRepository
    {
        Tenant tenant;
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        IInvAdjustDetailRepository InvAdjustDetailRepo;
        IInventoryRepository InventoryRepo;
        IInventoryHistoryRepository InvHistoryRepo;
        IInvAdjustUserFieldRepository InvAdjustUFieldRepo;
        IAuditTrailRepository AuditTrailRepo;
        AdjustmentTaskRepoSub AdjustmentTaskRepoSub;
        InvAdjustAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public InvAdjustRepository(ITenantProvider tenantProvider,
                            IAuditTrailRepository auditTrailRepo,
                            IIdNumberRepository idNumberRepo,
                            IInvAdjustDetailRepository invAdjustDetailsRepo,
                            IInventoryRepository inventoryRepo,
                            IInventoryHistoryRepository invHistoryRepo,
                            IInvAdjustUserFieldRepository invAdjustUFieldRepo,
                            AdjustmentTaskRepoSub adjustmentTaskRepoSub)
        {
            tenant = tenantProvider.GetTenant();
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            InvAdjustDetailRepo = invAdjustDetailsRepo;
            InvAdjustUFieldRepo = invAdjustUFieldRepo;
            InventoryRepo = inventoryRepo;
            InvHistoryRepo = invHistoryRepo;
            AdjustmentTaskRepoSub = adjustmentTaskRepoSub;
            PagingRepo = new PagingRepository();
            AuditBuilder = new InvAdjustAudit();
        }
        public async Task<CancelInvAdjustResultCode> CancelInvAdjust(string invAdjustId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get InvAdjust details
                var invAdjustDetails = await InvAdjustDetailRepo.LockInvAdjustDetails(db, invAdjustId);

                if (invAdjustDetails == null || !invAdjustDetails.Any())
                {
                    return CancelInvAdjustResultCode.INVADJUSTDETAILLOCKFAILED;
                }

                // check if InvAdjust details is all in create status
                var mods = invAdjustDetails.Where(y => y.InvAdjustLineStatusId != (InvAdjustLneStatus.CREATED).ToString());
                if (mods.Any())
                {
                    return CancelInvAdjustResultCode.INVADJUSTDETAILSSTATUSALTERED;
                }

                // lock InvAdjust header
                var invAdjust = await LockInvAdjust(db, invAdjustId);
                if (invAdjust == null)
                {
                    return CancelInvAdjustResultCode.INVADJUSTLOCKFAILED;
                }

                // check if InvAdjust header is in create status
                if (invAdjust.InvAdjustStatusId != (InvAdjustStatus.CREATED).ToString())
                {
                    return CancelInvAdjustResultCode.INVADJUSTSTATUSALTERED;
                }

                // update InvAdjust status into canceled
                invAdjust.InvAdjustStatusId = (InvAdjustStatus.CANCELLED).ToString();
                var poAltered = await UpdateInvAdjust(db, invAdjust, TranType.CANCELADJ);

                if (!poAltered)
                {
                    return CancelInvAdjustResultCode.INVADJUSTSTATUSUPDATEFAILED;
                }

                // update InvAdjust details staus
                int alteredDtlCnt = 0;
                foreach (var invAdjustDetail in invAdjustDetails)
                {
                    invAdjustDetail.InvAdjustLineStatusId = (InvAdjustLneStatus.CLOSED).ToString();
                    var poDtlAltered = await InvAdjustDetailRepo.UpdateInvAdjustDetailMod(db, invAdjustDetail, TranType.CANCELADJ);

                    if (!poDtlAltered)
                    {
                        return CancelInvAdjustResultCode.INVADJUSTDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == invAdjustDetails.Count())
                {
                    return CancelInvAdjustResultCode.SUCCESS;
                }
            }

            return CancelInvAdjustResultCode.FAILED;
        }

        public async Task<bool> CreateInvAdjust(IDbConnection db, InvAdjustModel invAdjust)
        {
            // define InvAdjust status
            invAdjust.InvAdjustStatusId = (InvAdjustStatus.CREATED).ToString();

            string strQry = @"insert into invadjust(invAdjustId, 
														invAdjustStatusId, 
														warehouseId, 
														reasonCodeId, 
														reason, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@invAdjustId, 
														@invAdjustStatusId, 
														@warehouseId, 
														@reasonCodeId, 
														@reason, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, invAdjust);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditADD(invAdjust, TranType.INVADJ);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InvAdjustCreateTranResult> CreateInvAdjustMod(InvAdjustModelMod invAdjust)
        {
            // get InvAdjust id number
            var invAdjustId = await IdNumberRepo.GetNextIdNum("INVADJ");

            if (!string.IsNullOrEmpty(invAdjustId) &&
                invAdjust.InvAdjustHeader != null &&
                invAdjust.InvAdjustDetails != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // create header
                    invAdjust.InvAdjustHeader.InvAdjustId = invAdjustId;
                    var headCreated = await CreateInvAdjust(db, invAdjust.InvAdjustHeader);

                    if (headCreated)
                    {
                        // init InvAdjust user fields default data
                        var initInvAdjustUFld = await InvAdjustUFieldRepo.InitInvAdjustUField(db, invAdjustId);
                        if (!initInvAdjustUFld)
                        {
                            return new InvAdjustCreateTranResult()
                            {
                                ResultCode = InvAdjustTranResultCode.USRFIELDSAVEFAILED
                            };
                        }

                        // insert InvAdjust user fields values
                        if (invAdjust.InvAdjustUfields != null)
                        {
                            var uFieldsCreated = await InvAdjustUFieldRepo.UpdateinvAdjustUField(db, invAdjustId, invAdjust.InvAdjustHeader.CreatedBy, invAdjust.InvAdjustUfields);
                            if (!uFieldsCreated)
                            {
                                return new InvAdjustCreateTranResult()
                                {
                                    ResultCode = InvAdjustTranResultCode.USRFIELDSAVEFAILED
                                };
                            }
                        }

                        // create detail
                        if (invAdjust.InvAdjustDetails.Any())
                        {
                            var details = invAdjust.InvAdjustDetails.ToList();

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];

                                // check if similar SKU exists under this Adjustment
                                var skuExists = await SKUExistsInInvAdjust(db, detail.InventoryId, invAdjustId);
                                if (skuExists)
                                {
                                    return new InvAdjustCreateTranResult()
                                    {
                                        ResultCode = InvAdjustTranResultCode.SKUCONFLICT
                                    };
                                }

                                // check if qty if valid
                                if (detail.QtyTo != null)
                                {
                                    var adjustQty = await AdjustQtyIsValid(db, detail.InventoryId, detail.QtyTo);
                                    if (adjustQty)
                                    {
                                        return new InvAdjustCreateTranResult()
                                        {
                                            ResultCode = InvAdjustTranResultCode.INVALIDQTY
                                        };
                                    }
                                }

                                // set detail id, status and header invAdjust id
                                detail.InvAdjustLineId = $"{invAdjustId}-{i + 1}";
                                detail.InvAdjustLineStatusId = (InvAdjustLneStatus.CREATED).ToString();
                                detail.InvAdjustId = invAdjustId;
                                //detail.InventoryId = 

                                // create detail
                                bool dtlSaved = await InvAdjustDetailRepo.CreateInvAdjustDetailMod(db, detail);

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return new InvAdjustCreateTranResult()
                                    {
                                        ResultCode = InvAdjustTranResultCode.ADJUSTMENTDOCLINESAVEFAILED
                                    };
                                }
                            }
                        }

                        return new InvAdjustCreateTranResult()
                        {
                            ResultCode = InvAdjustTranResultCode.SUCCESS,
                            InvAdjustId = invAdjustId
                        };
                    }
                }
            }

            return new InvAdjustCreateTranResult()
            {
                ResultCode = InvAdjustTranResultCode.FAILED
            };
        }

        public async Task<bool> AdjustQtyIsValid(IDbConnection db, string inventoryId, int? qtyTo)
        {
            string strQry = @"SELECT count(inventoryId)
                                FROM inventoryhistory ih
                                WHERE ih.inventoryId = @inventoryId AND @qtyTo > ih.qtyTo";

            var param = new DynamicParameters();
            param.Add("@inventoryId", inventoryId);
            param.Add("@qtyTo", qtyTo);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);
            if (res == 0)
            {
                return false;
            }

            // default true to ensure no conflict will occur on error
            return true;
        }

        public async Task<bool> DeleteInvAdjust(string invAdjustId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvAdjust where 
														invAdjustId = @invAdjustId";
                var param = new DynamicParameters();
                param.Add("@invAdjustId", invAdjustId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelInvAdjustResultCode> ForceCancelInvAdjust(string invAdjustId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get InvAdjust details
                var invAdjustDetails = await InvAdjustDetailRepo.LockInvAdjustDetails(db, invAdjustId);

                if (invAdjustDetails == null || !invAdjustDetails.Any())
                {
                    return CancelInvAdjustResultCode.INVADJUSTDETAILLOCKFAILED;
                }

                // check if InvAdjust details is valid for force close process
                var detailsValid = await ChkInvAdjustDtlsCanFClose(invAdjustDetails);
                if (!detailsValid)
                {
                    return CancelInvAdjustResultCode.INVADJUSTDETAILSNOTVALID;
                }

                // check InvAdjust details if there's no pending task
                var hasPendingAdjustTask = await AdjustmentTaskRepoSub.HasPendingAdjustmentTask(db, invAdjustId);
                if (hasPendingAdjustTask)
                {
                    return CancelInvAdjustResultCode.HASADJUSTMENTPENDING;
                }

                // lock InvAdjust header
                var po = await LockInvAdjust(db, invAdjustId);
                if (po == null)
                {
                    return CancelInvAdjustResultCode.INVADJUSTLOCKFAILED;
                }

                // update InvAdjust status into forced closed
                po.InvAdjustStatusId = (InvAdjustStatus.FRCCLOSED).ToString();
                var invAdjustAltered = await UpdateInvAdjust(db, po, TranType.INVADJ);

                if (!invAdjustAltered)
                {
                    return CancelInvAdjustResultCode.INVADJUSTSTATUSUPDATEFAILED;
                }

                // update InvAdjust details status
                int alteredDtlCnt = 0;
                foreach (var invAdjustDetail in invAdjustDetails)
                {
                    invAdjustDetail.InvAdjustLineStatusId = (InvAdjustLneStatus.FRCCLOSED).ToString();
                    var poDtlAltered = await InvAdjustDetailRepo.UpdateInvAdjustDetailMod(db, invAdjustDetail, TranType.CANCELADJ);

                    if (!poDtlAltered)
                    {
                        return CancelInvAdjustResultCode.INVADJUSTDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == invAdjustDetails.Count())
                {
                    return CancelInvAdjustResultCode.SUCCESS;
                }
            }

            return CancelInvAdjustResultCode.FAILED;
        }

        private async Task<bool> ChkInvAdjustDtlsCanFClose(IEnumerable<InvAdjustDetailModel>? invAdjustDetails)
        {
            return await Task.Run(() =>
            {
                // check if InvAdjust contains force close-able details
                var dtlCreateCnt = invAdjustDetails.Where(x => x.InvAdjustLineStatusId == (InvAdjustLneStatus.CREATED).ToString()).Count();
                var dtlFullAdjCnt = invAdjustDetails.Where(x => x.InvAdjustLineStatusId == (InvAdjustLneStatus.CLOSED).ToString()).Count();

                if (dtlCreateCnt > 0 && dtlFullAdjCnt > 0)
                {
                    return true;
                }

                return false;
            });
        }

        public async Task<InvAdjustModel> GetInvAdjustById(string invAdjustId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT * FROM invadjust 
                                    INNER JOIN invadjuststatus stats 
	                                    ON invadjust.invAdjustStatusId = stats.invAdjustStatusId 
                                    WHERE invAdjustId = @invAdjustId;";

                var param = new DynamicParameters();
                param.Add("@invAdjustId", invAdjustId);
                return await db.QuerySingleOrDefaultAsync<InvAdjustModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvAdjustPagedMdl?> GetInvAdjustFilteredPaged(InvAdjustFilteredMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select invadjust.*, invadjuststatus.invAdjustStatus from invadjust";
            string strFltr = " where ";
            var param = new DynamicParameters();

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(invAdjustId) from invadjust";

            if (!string.IsNullOrEmpty(filter.InvAdjustId))
            {
                strFltr += $"invAdjustId = @invAdjustId ";
                param.Add("@invAdjustId", filter.InvAdjustId);
            }

            if (!string.IsNullOrEmpty(filter.InvAdjustStatusId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"invadjust.invAdjustStatusId = @invAdjustStatusId ";
                param.Add("@invAdjustStatusId", filter.InvAdjustStatusId);
            }

            if (filter.DateCreated != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"dateCreated = @dateCreated ";
                param.Add("@dateCreated", filter.DateCreated);
            }

            // build inner joins
            string strJoins = @" inner join invadjuststatus on invadjust.invAdjustStatusId = invAdjustStatus.invAdjustStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by invAdjustId";
            strQry += $" limit @pageItem offset @offset";

            // set paging param
            pgParam = param;

            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var ret = await db.QueryAsync<InvAdjustModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new InvAdjustPagedMdl()
                    {
                        Pagination = pageDetail,
                        InvAdjust = ret
                    };
                }
            }

            return null;
        }

        public async Task<InvAdjustPagedMdl?> GetInvAdjustForAdjPaged(int pageNum, int pageItem)
        {
            string strQry = "select invadjust.*, invadjuststatus.invadjustStatus from invadjust";
            string strFltr = @" where invadjust.invAdjustStatusId = @statsCreated ";

            var param = new DynamicParameters();
            param.Add("@statsCreated", (InvAdjustStatus.CREATED).ToString());

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(invAdjustId) from invadjust";

            // build inner joins
            string strJoins = @" inner join invadjuststatus on invadjust.invAdjustStatusId = invAdjustStatus.invAdjustStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by invAdjustId desc";
            strQry += $" limit @pageItem offset @offset";

            // set paging param
            pgParam = param;

            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var ret = await db.QueryAsync<InvAdjustModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new InvAdjustPagedMdl()
                    {
                        Pagination = pageDetail,
                        InvAdjust = ret
                    };
                }
            }

            return null;
        }

        public async Task<InvAdjustPagedMdl?> GetInvAdjustPaged(int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                int offset = (pageNum - 1) * pageItem;
                string strQry = @"select invadjust.*, 
                                            invadjuststatus.invAdjustStatus 
                                    from invadjust 
                                    inner join invadjuststatus on invadjust.invAdjustStatusId = invadjuststatus.invAdjustStatusId 
                                    order by invAdjustId 
                                    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvAdjustModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvAdjustPageDetail(db, pageNum, pageItem, ret.Count());

                    return new InvAdjustPagedMdl()
                    {
                        Pagination = pageDetail,
                        InvAdjust = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetInvAdjustPageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = "select count(invAdjustId) from invadjust";
            return await PagingRepo.GetPageDetail(db, strQry, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<InvAdjustModel>> GetInvAdjustPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvAdjust limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvAdjustModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvAdjustModel>> GetInvAdjustPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvAdjust where 
														invAdjustId like @searchKey or 
														invadjust.invAdjustStatusId like @searchKey or
                                                        invadjust.warehouseId like @searchKey or
                                                        invadjust.reasonCodeId like @searchKey or
                                                        invadjust.reason like @searchKey or
							                            invadjust.dateCreated like @searchKey or 
							                            invadjust.dateModified like @searchKey or 
							                            invadjust.createdBy like @searchKey or 
							                            invadjust.modifiedBy like @searchKey or 
							                            invadjust.remarks like @searchKey
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvAdjustModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvAdjustPagedMdl?> GetInvAdjustSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invadjust.*, invadjuststats.invAdjustStatus from InvAdjust 
                                                inner join invadjuststatus invadjuststats on invadjust.invAdjustStatusId = invadjuststats.invAdjustStatusId 
                                    where invadjust.invAdjustId like @searchKey or 
                                                invadjust.invAdjustStatusId like @searchKey or
                                                invadjust.warehouseId like @searchKey or
                                                invadjust.reasonCodeId like @searchKey or
                                                invadjust.reason like @searchKey or
											    invadjust.dateCreated like @searchKey or 
											    invadjust.dateModified like @searchKey or 
											    invadjust.createdBy like @searchKey or 
											    invadjust.modifiedBy like @searchKey or 
											    invadjust.remarks like @searchKey or 
                                                invadjuststats.invAdjustStatus like @searchKey 
											    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvAdjustModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvAdjustSrchPageDetail(db, searchKey, pageNum, pageItem, ret.Count());

                    return new InvAdjustPagedMdl()
                    {
                        Pagination = pageDetail,
                        InvAdjust = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetInvAdjustSrchPageDetail(IDbConnection db, string searchKey, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"select count(invAdjustId) from InvAdjust 
                                    inner join invadjuststatus on invadjust.invAdjustStatusId = invadjuststatus.invAdjustStatusId 
                               where invadjust.invAdjustId like @searchKey or 
                                    invadjust.invAdjustStatusId like @searchKey or
                                    invadjust.warehouseId like @searchKey or
                                    invadjust.reasonCodeId like @searchKey or
                                    invadjust.reason like @searchKey or
							        invadjust.dateCreated like @searchKey or 
							        invadjust.dateModified like @searchKey or 
							        invadjust.createdBy like @searchKey or 
							        invadjust.modifiedBy like @searchKey or 
							        invadjust.remarks like @searchKey or 
                                    invadjuststatus.invAdjustStatus like @searchKey";


            var param = new DynamicParameters();
            param.Add("@searchKey", $" %{ searchKey}% ");

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<string?> GetInvAdjustUpdatedStatus(IDbConnection db, string invAdjustId)
        {
            string strQry = @"call `spGetInvAdjustUpdatedStatus`(@paramInvAdjustId);";

            var param = new DynamicParameters();
            param.Add("@paramInvAdjustId", invAdjustId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        public async Task<bool> InvAdjustExists(string invAdjustId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invAdjustId from InvAdjust where 
														invAdjustId = @invAdjustId";

                var param = new DynamicParameters();
                param.Add("@invAdjustId", invAdjustId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InvAdjustModel> LockInvAdjust(IDbConnection db, string invAdjustId)
        {
            string strQry = @"SELECT * FROM invadjust WHERE invAdjustId = @invAdjustId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@invAdjustId", invAdjustId);

            return await db.QuerySingleOrDefaultAsync<InvAdjustModel>(strQry, param);
        }

        public async Task<bool> SKUExistsInInvAdjust(IDbConnection db, string inventoryId, string invAdjustId)
        {
            string strQry = @"select count(inventoryId) from invAdjustDetail 
                                where inventoryId = @inventoryId and 
                                        invAdjustId = @invAdjustId";

            var param = new DynamicParameters();
            param.Add("@inventoryId", inventoryId);
            param.Add("@invAdjustId", invAdjustId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);
            if (res == 0)
            {
                return false;
            }

            // default true to ensure no conflict will occur on error
            return true;
        }

        public async Task<bool> UpdateInvAdjust(IDbConnection db, InvAdjustModel invAdjust, TranType tranTyp)
        {
            string strQry = @"update InvAdjust set 
							                reasonCodeId = @reasonCodeId, 
                                            reason = @reason,
							                invAdjustStatusId = @invAdjustStatusId, 
							                modifiedBy = @modifiedBy, 
							                remarks = @remarks where 
							                invAdjustId = @invAdjustId";

            int res = await db.ExecuteAsync(strQry, invAdjust);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditMOD(invAdjust, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }
        public async Task<InvAdjustTranResultCode> UpdateInvAdjustMod(InvAdjustModelMod invAdjust)
        {
            if (invAdjust.InvAdjustHeader != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    if (invAdjust.InvAdjustHeader.InvAdjustStatusId == InvAdjustLneStatus.CREATED.ToString())
                    {
                        // update header
                        var modHeader = await UpdateInvAdjust(db, invAdjust.InvAdjustHeader, TranType.INVADJ);

                        if (modHeader)
                        {
                            // update InvAdjust user fields values
                            if (invAdjust.InvAdjustUfields != null)
                            {
                                var uFieldsCreated = await InvAdjustUFieldRepo.UpdateinvAdjustUFieldMOD(db, invAdjust.InvAdjustHeader.InvAdjustId, invAdjust.InvAdjustHeader.ModifiedBy, invAdjust.InvAdjustUfields);
                                if (!uFieldsCreated)
                                {
                                    return InvAdjustTranResultCode.USRFIELDSAVEFAILED;
                                }
                            }

                            // update detail
                            if (invAdjust.InvAdjustDetails != null && invAdjust.InvAdjustDetails.Any())
                            {
                                var details = invAdjust.InvAdjustDetails.ToList();

                                // get last InvAdjust detail line number
                                var invAdjustDetailsFromDb = await InvAdjustDetailRepo.LockInvAdjustDetails(db, invAdjust.InvAdjustHeader.InvAdjustId);
                                var lastinvAdjustLneId = invAdjustDetailsFromDb.OrderByDescending(x => x.InvAdjustLineId).Select(y => y.InvAdjustLineId).FirstOrDefault();
                                int lastLneNum = 0;

                                if (!string.IsNullOrEmpty(lastinvAdjustLneId))
                                {
                                    lastLneNum = Convert.ToInt32(lastinvAdjustLneId.Substring(lastinvAdjustLneId.LastIndexOf('-') + 1));
                                }
                                else
                                {
                                    lastLneNum = 0;
                                }

                                for (int i = 0; i < details.Count(); i++)
                                {
                                    var detail = details[i];
                                    bool dtlSaved = false;

                                    if (detail.InvAdjustLineId == null)
                                    {
                                        // check if similar SKU exists under this InvAdjust
                                        var skuExists = await SKUExistsInInvAdjust(db, detail.InventoryId, invAdjust.InvAdjustHeader.InvAdjustId);
                                        if (skuExists)
                                        {
                                            return InvAdjustTranResultCode.SKUCONFLICT;
                                        }

                                        // detail concidered as new
                                        // set detail id, status and header po id
                                        lastLneNum += 1;
                                        detail.InvAdjustLineId = $"{invAdjust.InvAdjustHeader.InvAdjustId}-{lastLneNum}";
                                        detail.InvAdjustLineStatusId = (InvAdjustLneStatus.CREATED).ToString();
                                        detail.InvAdjustId = invAdjust.InvAdjustHeader.InvAdjustId;

                                        // create detail
                                        dtlSaved = await InvAdjustDetailRepo.CreateInvAdjustDetailMod(db, detail);
                                    }
                                    else
                                    {
                                        // update existing details
                                        var prevDetail = await InvAdjustDetailRepo.GetInvAdjustDetailByIdMod(db, detail.InvAdjustLineId);

                                        if (prevDetail.InvAdjustLineStatusId == InvAdjustLneStatus.CREATED.ToString())
                                        {
                                            if (prevDetail != detail)
                                            {
                                                dtlSaved = await InvAdjustDetailRepo.UpdateInvAdjustDetailMod(db, detail, TranType.INVADJ);
                                            }
                                        }
                                    }

                                    // return false if either of detail failed to save
                                    if (!dtlSaved)
                                    {
                                        return InvAdjustTranResultCode.ADJUSTMENTDOCLINESAVEFAILED;
                                    }
                                }
                            }
                        }
                        return InvAdjustTranResultCode.SUCCESS;
                    }
                }
            }
            return InvAdjustTranResultCode.FAILED;
        }

        //public async Task<InvAdjustTranResultCode> UpdateInvAdjustApprovedMod(string invAdjustId, string userAccountId)
        //{
        //    using (IDbConnection db = new MySqlConnection(ConnString))
        //    {
        //        db.Open();

        //        //check if user is admin
        //        var validUSer = await IsValidAccessRight(userAccountId);
        //        if (!validUSer)
        //        {
        //            return InvAdjustTranResultCode.INVALIDUSERACCESSRIGHT;
        //        }

        //        var statusApproved = (InvAdjustLneStatus.APPROVED).ToString();



        //        //// check if InvAdjust details is all in create status
        //        //var mods = invAdjustDetails.Where(y => y.InvAdjustLineStatusId != (InvAdjustLneStatus.CREATED).ToString());
        //        //if (mods.Any())
        //        //{
        //        //    return InvAdjustTranResultCode.INVADJUSTDETAILSSTATUSALTERED;
        //        //}

        //        //// lock InvAdjust header
        //        //var invAdjust = await LockInvAdjust(db, invAdjustId);
        //        //if (invAdjust == null)
        //        //{
        //        //    return InvAdjustTranResultCode.INVADJUSTLOCKFAILED;
        //        //}

        //        //// check if InvAdjust header is in create status
        //        //if (invAdjust.InvAdjustStatusId != (InvAdjustStatus.CREATED).ToString())
        //        //{
        //        //    return InvAdjustTranResultCode.INVADJUSTSTATUSALTERED;
        //        //}

        //        //// update InvAdjust status into canceled
        //        //invAdjust.InvAdjustStatusId = (InvAdjustStatus.CANCELLED).ToString();
        //        //var poAltered = await UpdateInvAdjust(db, invAdjust, TranType.CANCELADJ);

        //        //if (!poAltered)
        //        //{
        //        //    return InvAdjustTranResultCode.INVADJUSTSTATUSUPDATEFAILED;
        //        //}

        //        //// update InvAdjust details staus
        //        //int alteredDtlCnt = 0;
        //        //foreach (var invAdjustDetail in invAdjustDetails)
        //        //{
        //        //    invAdjustDetail.InvAdjustLineStatusId = (InvAdjustLneStatus.CLOSED).ToString();
        //        //    var poDtlAltered = await InvAdjustDetailRepo.UpdateInvAdjustDetailMod(db, invAdjustDetail, TranType.CANCELADJ);

        //        //    if (!poDtlAltered)
        //        //    {
        //        //        return InvAdjustTranResultCode.INVADJUSTDETAILSSTATUSUPDATEFAILED;
        //        //    }

        //        //    alteredDtlCnt += 1;
        //        //}

        //        //if (alteredDtlCnt == invAdjustDetails.Count())
        //        //{
        //        //    return InvAdjustTranResultCode.SUCCESS;
        //        //}
        //    }

        //    return InvAdjustTranResultCode.FAILED;
        //}
        public async Task<InvAdjustTranResultCode> UpdateInvAdjustApprovedMod(InvAdjustModelMod invAdjust)
        {
            if (invAdjust.InvAdjustHeader != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    if (invAdjust.InvAdjustHeader.InvAdjustStatusId == InvAdjustLneStatus.CREATED.ToString())
                    {
                        var validUSer = await IsValidAccessRight(invAdjust.InvAdjustHeader.ModifiedBy);
                        if (!validUSer)
                        {
                            return InvAdjustTranResultCode.INVALIDUSERACCESSRIGHT;
                        }

                        invAdjust.InvAdjustHeader.InvAdjustStatusId = (InvAdjustLneStatus.APPROVED).ToString();

                        // update header
                        var modHeader = await UpdateInvAdjust(db, invAdjust.InvAdjustHeader, TranType.INVADJ);

                        if (modHeader)
                        {
                            // update InvAdjust user fields values
                            if (invAdjust.InvAdjustUfields != null)
                            {
                                var uFieldsCreated = await InvAdjustUFieldRepo.UpdateinvAdjustUFieldMOD(db, invAdjust.InvAdjustHeader.InvAdjustId, invAdjust.InvAdjustHeader.ModifiedBy, invAdjust.InvAdjustUfields);
                                if (!uFieldsCreated)
                                {
                                    return InvAdjustTranResultCode.USRFIELDSAVEFAILED;
                                }
                            }

                            // update detail
                            if (invAdjust.InvAdjustDetails != null && invAdjust.InvAdjustDetails.Any())
                            {
                                var details = invAdjust.InvAdjustDetails.ToList();

                                // get last InvAdjust detail line number
                                var invAdjustDetailsFromDb = await InvAdjustDetailRepo.LockInvAdjustDetails(db, invAdjust.InvAdjustHeader.InvAdjustId);
                                var lastinvAdjustLneId = invAdjustDetailsFromDb.OrderByDescending(x => x.InvAdjustLineId).Select(y => y.InvAdjustLineId).FirstOrDefault();
                                int lastLneNum = 0;

                                if (!string.IsNullOrEmpty(lastinvAdjustLneId))
                                {
                                    lastLneNum = Convert.ToInt32(lastinvAdjustLneId.Substring(lastinvAdjustLneId.LastIndexOf('-') + 1));
                                }
                                else
                                {
                                    lastLneNum = 0;
                                }

                                for (int i = 0; i < details.Count(); i++)
                                {
                                    var detail = details[i];
                                    bool dtlSaved = false;

                                    if (detail.InvAdjustLineId == null)
                                    {
                                        return InvAdjustTranResultCode.FAILEDTOAPPROVE;
                                    }
                                    else
                                    {
                                        // update existing details
                                        var prevDetail = await InvAdjustDetailRepo.GetInvAdjustDetailByIdMod(db, detail.InvAdjustLineId);

                                        if (prevDetail.InvAdjustLineStatusId == (InvAdjustLneStatus.CREATED).ToString())
                                        {
                                            if (prevDetail != detail)
                                            {
                                                detail.InvAdjustLineStatusId = InvAdjustLneStatus.APPROVED.ToString();

                                                dtlSaved = await InvAdjustDetailRepo.UpdateInvAdjustDetailMod(db, detail, TranType.INVADJ);

                                                var invDetail = await GetInventoryByInvId(db, prevDetail.InventoryId);
                                            }
                                        }
                                    }

                                    // return false if either of detail failed to save
                                    if (!dtlSaved)
                                    {
                                        return InvAdjustTranResultCode.ADJUSTMENTDOCLINESAVEFAILED;
                                    }
                                }
                            }
                            return InvAdjustTranResultCode.SUCCESS;
                        }
                    }
                }
            }
            return InvAdjustTranResultCode.FAILED;
        }
        public async Task<bool> IsValidAccessRight(string? userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from useraccount 
                                where userAccountId = @userAccountId AND accessRightId = 'ADMIN'";

                var param = new DynamicParameters();
                param.Add("@userAccountId", userAccountId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InventoryHistoryModel> GetInventoryByInvId(IDbConnection db, string inventoryId)
        {
            string strQry = @"select * from inventoryhistory 
                                where seqnum = (
                                select MAX(seqnum)
                                from inventoryhistory 
                                where inventoryId = @inventoryId)";

            var param = new DynamicParameters();
            param.Add("@inventoryId", inventoryId);

            return await db.QuerySingleOrDefaultAsync<InventoryHistoryModel>(strQry, param, commandType: CommandType.Text);
        }
    }
}
