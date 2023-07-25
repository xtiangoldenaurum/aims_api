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
        IUserAccountRepository UserAccountRepo;

        public InvAdjustRepository(ITenantProvider tenantProvider,
                            IAuditTrailRepository auditTrailRepo,
                            IIdNumberRepository idNumberRepo,
                            IInvAdjustDetailRepository dataDetailsRepo,
                            IInventoryRepository inventoryRepo,
                            IInventoryHistoryRepository invHistoryRepo,
                            IInvAdjustUserFieldRepository dataUFieldRepo,
                            AdjustmentTaskRepoSub adjustmentTaskRepoSub,
                            IUserAccountRepository userAccountRepo)
        {
            tenant = tenantProvider.GetTenant();
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            InvAdjustDetailRepo = dataDetailsRepo;
            InvAdjustUFieldRepo = dataUFieldRepo;
            InventoryRepo = inventoryRepo;
            InvHistoryRepo = invHistoryRepo;
            AdjustmentTaskRepoSub = adjustmentTaskRepoSub;
            PagingRepo = new PagingRepository();
            AuditBuilder = new InvAdjustAudit();
            UserAccountRepo = userAccountRepo;
        }
        public async Task<CancelInvAdjustResultCode> CancelInvAdjust(string dataId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get InvAdjust details
                var dataDetails = await InvAdjustDetailRepo.LockInvAdjustDetails(db, dataId);

                if (dataDetails == null || !dataDetails.Any())
                {
                    return CancelInvAdjustResultCode.INVADJUSTDETAILLOCKFAILED;
                }

                // check if InvAdjust details is all in create status
                var mods = dataDetails.Where(y => y.InvAdjustLineStatusId != (InvAdjustLneStatus.CREATED).ToString());
                if (mods.Any())
                {
                    return CancelInvAdjustResultCode.INVADJUSTDETAILSSTATUSALTERED;
                }

                // lock InvAdjust header
                var data = await LockInvAdjust(db, dataId);
                if (data == null)
                {
                    return CancelInvAdjustResultCode.INVADJUSTLOCKFAILED;
                }

                // check if InvAdjust header is in create status
                if (data.InvAdjustStatusId != (InvAdjustStatus.CREATED).ToString())
                {
                    return CancelInvAdjustResultCode.INVADJUSTSTATUSALTERED;
                }

                // update InvAdjust status into canceled
                data.InvAdjustStatusId = (InvAdjustStatus.CANCELLED).ToString();
                var poAltered = await UpdateInvAdjust(db, data, TranType.CANCELADJ);

                if (!poAltered)
                {
                    return CancelInvAdjustResultCode.INVADJUSTSTATUSUPDATEFAILED;
                }

                // update InvAdjust details staus
                int alteredDtlCnt = 0;
                foreach (var dataDetail in dataDetails)
                {
                    dataDetail.InvAdjustLineStatusId = (InvAdjustLneStatus.CLOSED).ToString();
                    var poDtlAltered = await InvAdjustDetailRepo.UpdateInvAdjustDetailMod(db, dataDetail, TranType.CANCELADJ);

                    if (!poDtlAltered)
                    {
                        return CancelInvAdjustResultCode.INVADJUSTDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == dataDetails.Count())
                {
                    return CancelInvAdjustResultCode.SUCCESS;
                }
            }

            return CancelInvAdjustResultCode.FAILED;
        }

        public async Task<bool> CreateInvAdjust(IDbConnection db, InvAdjustModel data)
        {
            // define InvAdjust status
            data.InvAdjustStatusId = (InvAdjustStatus.CREATED).ToString();

            string strQry = @"insert into invadjust(dataId, 
														dataStatusId, 
														warehouseId, 
														reasonCodeId, 
														reason, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@dataId, 
														@dataStatusId, 
														@warehouseId, 
														@reasonCodeId, 
														@reason, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, data);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditADD(data, TranType.INVADJ);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InvAdjustCreateTranResult> CreateInvAdjustMod(InvAdjustModelMod data)
        {
            // get InvAdjust id number
            var dataId = await IdNumberRepo.GetNextIdNum("INVADJ");

            if (!string.IsNullOrEmpty(dataId) &&
                data.InvAdjustHeader != null &&
                data.InvAdjustDetails != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // create header
                    data.InvAdjustHeader.InvAdjustId = dataId;
                    var headCreated = await CreateInvAdjust(db, data.InvAdjustHeader);

                    if (headCreated)
                    {
                        // init InvAdjust user fields default data
                        var initInvAdjustUFld = await InvAdjustUFieldRepo.InitInvAdjustUField(db, dataId);
                        if (!initInvAdjustUFld)
                        {
                            return new InvAdjustCreateTranResult()
                            {
                                ResultCode = InvAdjustTranResultCode.USRFIELDSAVEFAILED
                            };
                        }

                        // insert InvAdjust user fields values
                        if (data.InvAdjustUfields != null)
                        {
                            var uFieldsCreated = await InvAdjustUFieldRepo.UpdateInvAdjustUField(db, dataId, data.InvAdjustHeader.CreatedBy, data.InvAdjustUfields);
                            if (!uFieldsCreated)
                            {
                                return new InvAdjustCreateTranResult()
                                {
                                    ResultCode = InvAdjustTranResultCode.USRFIELDSAVEFAILED
                                };
                            }
                        }

                        // create detail
                        if (data.InvAdjustDetails.Any())
                        {
                            var details = data.InvAdjustDetails.ToList();

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];

                                // check if similar InvID exists under this Adjustment
                                var invIdExists = await InvIdExistsInInvAdjust(db, detail.InventoryId, dataId);
                                if (invIdExists)
                                {
                                    return new InvAdjustCreateTranResult()
                                    {
                                        ResultCode = InvAdjustTranResultCode.INVENTORYIDCONFLICT
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

                                // set detail id, status and header data id
                                detail.InvAdjustLineId = $"{dataId}-{i + 1}";
                                detail.InvAdjustLineStatusId = (InvAdjustLneStatus.CREATED).ToString();
                                detail.InvAdjustId = dataId;
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
                            InvAdjustId = dataId
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

        public async Task<bool> DeleteInvAdjust(string dataId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvAdjust where 
														dataId = @dataId";
                var param = new DynamicParameters();
                param.Add("@dataId", dataId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelInvAdjustResultCode> ForceCancelInvAdjust(string dataId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get InvAdjust details
                var dataDetails = await InvAdjustDetailRepo.LockInvAdjustDetails(db, dataId);

                if (dataDetails == null || !dataDetails.Any())
                {
                    return CancelInvAdjustResultCode.INVADJUSTDETAILLOCKFAILED;
                }

                // check if InvAdjust details is valid for force close process
                var detailsValid = await ChkInvAdjustDtlsCanFClose(dataDetails);
                if (!detailsValid)
                {
                    return CancelInvAdjustResultCode.INVADJUSTDETAILSNOTVALID;
                }

                // check InvAdjust details if there's no pending task
                var hasPendingAdjustTask = await AdjustmentTaskRepoSub.HasPendingAdjustmentTask(db, dataId);
                if (hasPendingAdjustTask)
                {
                    return CancelInvAdjustResultCode.HASADJUSTMENTPENDING;
                }

                // lock InvAdjust header
                var po = await LockInvAdjust(db, dataId);
                if (po == null)
                {
                    return CancelInvAdjustResultCode.INVADJUSTLOCKFAILED;
                }

                // update InvAdjust status into forced closed
                po.InvAdjustStatusId = (InvAdjustStatus.FRCCLOSED).ToString();
                var dataAltered = await UpdateInvAdjust(db, po, TranType.INVADJ);

                if (!dataAltered)
                {
                    return CancelInvAdjustResultCode.INVADJUSTSTATUSUPDATEFAILED;
                }

                // update InvAdjust details status
                int alteredDtlCnt = 0;
                foreach (var dataDetail in dataDetails)
                {
                    dataDetail.InvAdjustLineStatusId = (InvAdjustLneStatus.FRCCLOSED).ToString();
                    var poDtlAltered = await InvAdjustDetailRepo.UpdateInvAdjustDetailMod(db, dataDetail, TranType.CANCELADJ);

                    if (!poDtlAltered)
                    {
                        return CancelInvAdjustResultCode.INVADJUSTDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == dataDetails.Count())
                {
                    return CancelInvAdjustResultCode.SUCCESS;
                }
            }

            return CancelInvAdjustResultCode.FAILED;
        }

        private async Task<bool> ChkInvAdjustDtlsCanFClose(IEnumerable<InvAdjustDetailModel>? dataDetails)
        {
            return await Task.Run(() =>
            {
                // check if InvAdjust contains force close-able details
                var dtlCreateCnt = dataDetails.Where(x => x.InvAdjustLineStatusId == (InvAdjustLneStatus.CREATED).ToString()).Count();
                var dtlFullAdjCnt = dataDetails.Where(x => x.InvAdjustLineStatusId == (InvAdjustLneStatus.CLOSED).ToString()).Count();

                if (dtlCreateCnt > 0 && dtlFullAdjCnt > 0)
                {
                    return true;
                }

                return false;
            });
        }

        public async Task<InvAdjustModel> GetInvAdjustById(string dataId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT * FROM invadjust 
                                    INNER JOIN invadjuststatus stats 
	                                    ON invadjust.dataStatusId = stats.dataStatusId 
                                    WHERE dataId = @dataId;";

                var param = new DynamicParameters();
                param.Add("@dataId", dataId);
                return await db.QuerySingleOrDefaultAsync<InvAdjustModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvAdjustPagedMdl?> GetInvAdjustFilteredPaged(InvAdjustFilteredMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select invadjust.*, invadjuststatus.dataStatus from invadjust";
            string strFltr = " where ";
            var param = new DynamicParameters();

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(dataId) from invadjust";

            if (!string.IsNullOrEmpty(filter.InvAdjustId))
            {
                strFltr += $"dataId = @dataId ";
                param.Add("@dataId", filter.InvAdjustId);
            }

            if (!string.IsNullOrEmpty(filter.InvAdjustStatusId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"invadjust.dataStatusId = @dataStatusId ";
                param.Add("@dataStatusId", filter.InvAdjustStatusId);
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
            string strJoins = @" inner join invadjuststatus on invadjust.dataStatusId = dataStatus.dataStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by dataId";
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
            string strFltr = @" where invadjust.dataStatusId = @statsCreated ";

            var param = new DynamicParameters();
            param.Add("@statsCreated", (InvAdjustStatus.CREATED).ToString());

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(dataId) from invadjust";

            // build inner joins
            string strJoins = @" inner join invadjuststatus on invadjust.dataStatusId = dataStatus.dataStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by dataId desc";
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
                                            invadjuststatus.dataStatus 
                                    from invadjust 
                                    inner join invadjuststatus on invadjust.dataStatusId = invadjuststatus.dataStatusId 
                                    order by dataId 
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
            string strQry = "select count(dataId) from invadjust";
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
														dataId like @searchKey or 
														invadjust.dataStatusId like @searchKey or
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
                string strQry = @"select invadjust.*, invadjuststats.dataStatus from InvAdjust 
                                                inner join invadjuststatus invadjuststats on invadjust.dataStatusId = invadjuststats.dataStatusId 
                                    where invadjust.dataId like @searchKey or 
                                                invadjust.dataStatusId like @searchKey or
                                                invadjust.warehouseId like @searchKey or
                                                invadjust.reasonCodeId like @searchKey or
                                                invadjust.reason like @searchKey or
											    invadjust.dateCreated like @searchKey or 
											    invadjust.dateModified like @searchKey or 
											    invadjust.createdBy like @searchKey or 
											    invadjust.modifiedBy like @searchKey or 
											    invadjust.remarks like @searchKey or 
                                                invadjuststats.dataStatus like @searchKey 
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
            string strQry = @"select count(dataId) from InvAdjust 
                                    inner join invadjuststatus on invadjust.dataStatusId = invadjuststatus.dataStatusId 
                               where invadjust.dataId like @searchKey or 
                                    invadjust.dataStatusId like @searchKey or
                                    invadjust.warehouseId like @searchKey or
                                    invadjust.reasonCodeId like @searchKey or
                                    invadjust.reason like @searchKey or
							        invadjust.dateCreated like @searchKey or 
							        invadjust.dateModified like @searchKey or 
							        invadjust.createdBy like @searchKey or 
							        invadjust.modifiedBy like @searchKey or 
							        invadjust.remarks like @searchKey or 
                                    invadjuststatus.dataStatus like @searchKey";


            var param = new DynamicParameters();
            param.Add("@searchKey", $" %{ searchKey}% ");

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<string?> GetInvAdjustUpdatedStatus(IDbConnection db, string dataId)
        {
            string strQry = @"call `spGetInvAdjustUpdatedStatus`(@paramInvAdjustId);";

            var param = new DynamicParameters();
            param.Add("@paramInvAdjustId", dataId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        public async Task<bool> InvAdjustExists(string dataId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select dataId from InvAdjust where 
														dataId = @dataId";

                var param = new DynamicParameters();
                param.Add("@dataId", dataId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InvAdjustModel> LockInvAdjust(IDbConnection db, string dataId)
        {
            string strQry = @"SELECT * FROM invadjust WHERE dataId = @dataId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@dataId", dataId);

            return await db.QuerySingleOrDefaultAsync<InvAdjustModel>(strQry, param);
        }

        public async Task<bool> InvIdExistsInInvAdjust(IDbConnection db, string inventoryId, string dataId)
        {
            string strQry = @"select count(inventoryId) from dataDetail 
                                where inventoryId = @inventoryId and 
                                        dataId = @dataId";

            var param = new DynamicParameters();
            param.Add("@inventoryId", inventoryId);
            param.Add("@dataId", dataId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);
            if (res == 0)
            {
                return false;
            }

            // default true to ensure no conflict will occur on error
            return true;
        }

        public async Task<bool> UpdateInvAdjust(IDbConnection db, InvAdjustModel data, TranType tranTyp)
        {
            string strQry = @"update InvAdjust set 
							                reasonCodeId = @reasonCodeId, 
                                            reason = @reason,
							                dataStatusId = @dataStatusId, 
							                modifiedBy = @modifiedBy, 
							                remarks = @remarks where 
							                dataId = @dataId";

            int res = await db.ExecuteAsync(strQry, data);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditMOD(data, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }
        public async Task<InvAdjustTranResultCode> UpdateInvAdjustMod(InvAdjustModelMod data)
        {
            if (data.InvAdjustHeader != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    if (data.InvAdjustHeader.InvAdjustStatusId == InvAdjustLneStatus.CREATED.ToString())
                    {
                        // update header
                        var modHeader = await UpdateInvAdjust(db, data.InvAdjustHeader, TranType.INVADJ);

                        if (modHeader)
                        {
                            // update InvAdjust user fields values
                            if (data.InvAdjustUfields != null)
                            {
                                var uFieldsCreated = await InvAdjustUFieldRepo.UpdateInvAdjustUFieldMOD(db, data.InvAdjustHeader.InvAdjustId, data.InvAdjustHeader.ModifiedBy, data.InvAdjustUfields);
                                if (!uFieldsCreated)
                                {
                                    return InvAdjustTranResultCode.USRFIELDSAVEFAILED;
                                }
                            }

                            // update detail
                            if (data.InvAdjustDetails != null && data.InvAdjustDetails.Any())
                            {
                                var details = data.InvAdjustDetails.ToList();

                                // get last InvAdjust detail line number
                                var dataDetailsFromDb = await InvAdjustDetailRepo.LockInvAdjustDetails(db, data.InvAdjustHeader.InvAdjustId);
                                var lastdataLneId = dataDetailsFromDb.OrderByDescending(x => x.InvAdjustLineId).Select(y => y.InvAdjustLineId).FirstOrDefault();
                                int lastLneNum = 0;

                                if (!string.IsNullOrEmpty(lastdataLneId))
                                {
                                    lastLneNum = Convert.ToInt32(lastdataLneId.Substring(lastdataLneId.LastIndexOf('-') + 1));
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
                                        // check if similar InventoryID exists under this InvAdjust
                                        var invIdExists = await InvIdExistsInInvAdjust(db, detail.InventoryId, data.InvAdjustHeader.InvAdjustId);
                                        if (invIdExists)
                                        {
                                            return InvAdjustTranResultCode.INVENTORYIDCONFLICT;
                                        }

                                        // detail concidered as new
                                        // set detail id, status and header po id
                                        lastLneNum += 1;
                                        detail.InvAdjustLineId = $"{data.InvAdjustHeader.InvAdjustId}-{lastLneNum}";
                                        detail.InvAdjustLineStatusId = (InvAdjustLneStatus.CREATED).ToString();
                                        detail.InvAdjustId = data.InvAdjustHeader.InvAdjustId;

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
        public async Task<InvAdjustTranResultCode> UpdateInvAdjustApprovedMod(AdjustmentModelMod data)
        {
            //if (data.InvHead != null)
            //{

            //    var dataDtls= data.InvHead;

            //    using (IDbConnection db = new MySqlConnection(ConnString))
            //    {
            //        db.Open();

            //        // lock InvAdjustDetail record
            //        var dataDetail = await InvAdjustDetailRepo.LockInvAdjustDetail(db, dataDtls.);

            //        // lock InvAdjust record
            //        var datax = await LockInvAdjust(db, data.InvHead.InvAdjustId);

            //        if (data.InvHead.InvHead == InvAdjustLneStatus.CREATED.ToString())
            //        {
            //            var validUSer = await UserAccountRepo.GetUserAccountById(data.InvHead.ModifiedBy);
            //            if (validUSer.AccessRightId != (UserType.ADMIN).ToString())
            //            {
            //                return InvAdjustTranResultCode.INVALIDUSERACCESSRIGHT;
            //            }

            //            data.InvHead.InvAdjustStatusId = (InvAdjustLneStatus.APPROVED).ToString();

            //            // update header
            //            var modHeader = await UpdateInvAdjust(db, data.InvAdjustHeader, TranType.INVADJ);

            //            if (modHeader)
            //            {
            //                // update InvAdjust user fields values
            //                if (data.InvAdjustUfields != null)
            //                {
            //                    var uFieldsCreated = await InvAdjustUFieldRepo.UpdateInvAdjustUFieldMOD(db, data.InvAdjustHeader.InvAdjustId, data.InvAdjustHeader.ModifiedBy, data.InvAdjustUfields);
            //                    if (!uFieldsCreated)
            //                    {
            //                        return InvAdjustTranResultCode.USRFIELDSAVEFAILED;
            //                    }
            //                }

            //                // update detail
            //                if (data.InvHead != null && data.InvHead.Any())
            //                {
            //                    var details = data.InvHead.ToList();

            //                    // get last InvAdjust detail line number
            //                    var dataDetailsFromDb = await InvAdjustDetailRepo.LockInvAdjustDetails(db, data.InvAdjustHeader.InvAdjustId);
            //                    var lastdataLneId = dataDetailsFromDb.OrderByDescending(x => x.InvAdjustLineId).Select(y => y.InvAdjustLineId).FirstOrDefault();
            //                    int lastLneNum = 0;

            //                    if (!string.IsNullOrEmpty(lastdataLneId))
            //                    {
            //                        lastLneNum = Convert.ToInt32(lastdataLneId.Substring(lastdataLneId.LastIndexOf('-') + 1));
            //                    }
            //                    else
            //                    {
            //                        lastLneNum = 0;
            //                    }

            //                    for (int i = 0; i < details.Count(); i++)
            //                    {
            //                        var detail = details[i];
            //                        bool dtlSaved = false;

            //                        if (detail.InvAdjustLineId == null)
            //                        {
            //                            return InvAdjustTranResultCode.FAILEDTOAPPROVE;
            //                        }
            //                        else
            //                        {
            //                            // update existing details
            //                            var prevDetail = await InvAdjustDetailRepo.GetInvAdjustDetailByIdMod(db, detail.InvAdjustLineId);

            //                            if (prevDetail.InvAdjustLineStatusId == (InvAdjustLneStatus.CREATED).ToString())
            //                            {
            //                                if (prevDetail != detail)
            //                                {
            //                                    detail.InvAdjustLineStatusId = InvAdjustLneStatus.APPROVED.ToString();

            //                                    dtlSaved = await InvAdjustDetailRepo.UpdateInvAdjustDetailMod(db, detail, TranType.INVADJ);

            //                                    var invDetail = await InvHistoryRepo.GetInvHistoryMaxSeqByInvId(db, prevDetail.InventoryId);
            //                                }
            //                            }
            //                        }

            //                        // return false if either of detail failed to save
            //                        if (!dtlSaved)
            //                        {
            //                            return InvAdjustTranResultCode.ADJUSTMENTDOCLINESAVEFAILED;
            //                        }
            //                    }
            //                }
            //                return InvAdjustTranResultCode.SUCCESS;
            //            }
            //        }
            //    }
            //}
            return InvAdjustTranResultCode.FAILED;
        }
    }
}
