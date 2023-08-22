using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Repositories.Sub;
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
    public class InvCountRepository : IInvCountRepository
    {
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        IInvCountDetailRepository InvCountDetailRepo;
        IAuditTrailRepository AuditTrailRepo;
        //MovementTaskRepoSub MovementTaskRepoSub;
        InvCountAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public InvCountRepository(ITenantProvider tenantProvider,
                            IAuditTrailRepository auditTrailRepo,
                            IIdNumberRepository idNumberRepo,
                            IInvCountDetailRepository invCountDetailsRepo
                            /*MovementTaskRepoSub MovementTaskRepoSub*/)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            InvCountDetailRepo = invCountDetailsRepo;
            //MovemmentTaskRepoSub = MovementTaskRepoSub;
            PagingRepo = new PagingRepository();
            AuditBuilder = new InvCountAudit();
        }


        public async Task<InvCountPagedMdl?> GetInvCountPaged(int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                int offset = (pageNum - 1) * pageItem;
                string strQry = @"select invcount.*, 
                                            invcountstatus.invCountStatus 
                                    from invcount 
                                    inner join invcountstatus on invcount.invCountStatusId = invcountstatus.invCountStatusId 
                                    order by invCountId 
                                    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvCountModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvCountPageDetail(db, pageNum, pageItem, ret.Count());

                    return new InvCountPagedMdl()
                    {
                        Pagination = pageDetail,
                        InvCount = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetInvCountPageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = "select count(invCountId) from invcount";
            return await PagingRepo.GetPageDetail(db, strQry, pageNum, pageItem, rowCount);
        }

        public async Task<InvCountPagedMdl?> GetInvCountFilteredPaged(InvCountFilteredMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select invcount.*, invcountstatus.invCountStatus from invcount";
            string strFltr = " where ";
            var param = new DynamicParameters();

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(invCountId) from invcount";

            if (!string.IsNullOrEmpty(filter.InvCountId))
            {
                strFltr += $"invCountId = @invCountId ";
                param.Add("@invCountId", filter.InvCountId);
            }

            if (!string.IsNullOrEmpty(filter.InvCountStatusId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"invcount.invCountStatusId = @invCountStatusId ";
                param.Add("@invCountStatusId", filter.InvCountStatusId);
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
            string strJoins = @" inner join invcountstatus on invcount.invCountStatusId = invCountStatus.invCountStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by invCountId";
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

                var ret = await db.QueryAsync<InvCountModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new InvCountPagedMdl()
                    {
                        Pagination = pageDetail,
                        InvCount = ret
                    };
                }
            }

            return null;
        }

        public async Task<InvCountPagedMdl?> GetInvCountForCntPaged(int pageNum, int pageItem)
        {
            string strQry = "select invcount.*, invcountstatus.invcountStatus from invcount";
            string strFltr = @" where invcount.invCountStatusId = @statsCreated";

            var param = new DynamicParameters();
            param.Add("@statsCreated", (InvCountStatus.CREATED).ToString());

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(invCountId) from invcount";

            // build inner joins
            string strJoins = @" inner join invcountstatus on invcount.invCountStatusId = invCountStatus.invCountStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by invCountId desc";
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

                var ret = await db.QueryAsync<InvCountModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new InvCountPagedMdl()
                    {
                        Pagination = pageDetail,
                        InvCount = ret
                    };
                }
            }

            return null;
        }

        public async Task<InvCountPagedMdl?> GetInvCountSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invcount.*, invcountstatus.invCountStatus from InvCount 
                                                inner join invcountstatus on invcount.invCountStatusId = invcountstatus.invCountStatusId 
                                                where invcount.invCountId like @searchKey or 
                                                invcount.invCountStatusId like @searchKey or
                                                invcount.warehouseId like @searchKey or 
                                                invcount.dateCreated like @searchKey or 
                                                invcount.dateModified like @searchKey or 
                                                invcount.createdBy like @searchKey or 
                                                invcount.modifiedBy like @searchKey or 
                                                invcount.remarks like @searchKey or 
                                                invcountstatus.invCountStatus like @searchKey 
                                                limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvCountModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvCountSrchPageDetail(db, searchKey, pageNum, pageItem, ret.Count());

                    return new InvCountPagedMdl()
                    {
                        Pagination = pageDetail,
                        InvCount = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetInvCountSrchPageDetail(IDbConnection db, string searchKey, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"select count(invCountId) from InvCount 
                                    inner join invcountstatus on invcount.invCountStatusId = invCountstatus.invCountStatusId 
                               where invcount.invCountId like @searchKey or 
                                    invcount.invCountStatusId like @searchKey or
                                    invcount.warehouseId like @searchKey or 
                                    invcount.dateCreated like @searchKey or 
                                    invcount.dateModified like @searchKey or 
                                    invcount.createdBy like @searchKey or 
                                    invcount.modifiedBy like @searchKey or 
                                    invcount.remarks like @searchKey or 
                                    invCountstatus.invCountStatus like @searchKey";

            var param = new DynamicParameters();
            param.Add("@searchKey", $" %{searchKey}% ");

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<InvCountModel>> GetInvCountPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvCount limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvCountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvCountModel>> GetInvCountPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvCount where 
                                     invCountId like @searchKey or 
                                     invcount.invCountStatusId like @searchKey or
                                     invcount.warehouseId like @searchKey or
                                     invcount.dateCreated like @searchKey or 
                                     invcount.dateModified like @searchKey or 
                                     invcount.createdBy like @searchKey or 
                                     invcount.modifiedBy like @searchKey or 
                                     invcount.remarks like @searchKey
                limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvCountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvCountModel> GetInvCountById(string invCountId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT * FROM invcount 
                                    INNER JOIN invcountstatus stats 
	                                    ON invcount.invCountStatusId = stats.invCountStatusId 
                                    WHERE invCountId = @invCountId;";

                var param = new DynamicParameters();
                param.Add("@invCountId", invCountId);
                return await db.QuerySingleOrDefaultAsync<InvCountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        #region InvMoveExists
        //public async Task<bool> InvMoveExists(string invMoveId)
        //{
        //    using (IDbConnection db = new MySqlConnection(ConnString))
        //    {
        //        db.Open();
        //        string strQry = @"select invMoveId from InvMove where 
        //						invMoveId = @invMoveId";
        //
        //        var param = new DynamicParameters();
        //        param.Add("@invMoveId", invMoveId);
        //
        //        var res = await db.ExecuteScalarAsync(strQry, param);
        //        if (res != null && res != DBNull.Value)
        //        {
        //            return true;
        //        }
        //    }
        //
        //    return false;
        //}
        #endregion

        // MAY ISSUE PA
        public async Task<InvCountModel> LockInvCount(IDbConnection db, string invCountId)
        {
            string strQry = @"SELECT * FROM invcount WHERE invCountId = @invCountId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@invCountId", invCountId);

            return await db.QuerySingleOrDefaultAsync<InvCountModel>(strQry, param);
        }

        // MAY ISSUE PA
        public async Task<string?> GetInvCountUpdatedStatus(IDbConnection db, string invCountId)
        {
            string strQry = @"call `spGetInvCountUpdatedStatus`(@paramInvCountId);";

            var param = new DynamicParameters();
            param.Add("@paramInvCountId", invCountId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        // MAY ISSUE PA
        public async Task<InvCountCreateTranResult> CreateInvCountMod(InvCountModelMod invCount)
        {
            // get InvCount id number
            var invCountId = await IdNumberRepo.GetNextIdNum("INVCNT");

            if (!string.IsNullOrEmpty(invCountId) &&
                invCount.InvCountHeader != null &&
                invCount.InvCountDetails != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // create header
                    invCount.InvCountHeader.InvCountId = invCountId;
                    var headCreated = await CreateInvCount(db, invCount.InvCountHeader);

                    if (headCreated)
                    {
                        #region init InvCount user fields default data
                        // init InvCount user fields default data
                        //var initInvMoveUFld = await InvCountUFieldRepo.InitInvMoveUField(db, invCountId);
                        //if (!initInvMoveUFld)
                        //{
                        //    return new InvCountCreateTranResult()
                        //    {
                        //        ResultCode = InvCountTranResultCode.USRFIELDSAVEFAILED
                        //    };
                        //}
                        #endregion

                        #region insert InvCount user fields values
                        // insert InvCount user fields values
                        //if (invCount.InvCountUfields != null)
                        //{
                        //    var uFieldsCreated = await InvCountUFieldRepo.UpdateinvMoveUField(db, invCountId, invCount.InvCountHeader.CreatedBy, invCount.InvCountUfields);
                        //    if (!uFieldsCreated)
                        //    {
                        //        return new InvCountCreateTranResult()
                        //        {
                        //            ResultCode = InvMoveTranResultCode.USRFIELDSAVEFAILED
                        //        };
                        //    }
                        //}
                        #endregion

                        // create detail
                        if (invCount.InvCountDetails.Any())
                        {
                            var details = invCount.InvCountDetails.ToList();

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];

                                // check if similar Inventory ID exists under this Count
                                var invIDExists = await InvIDExistsInInvCount(db, detail.InventoryId, invCountId);
                                if (invIDExists)
                                {
                                    return new InvCountCreateTranResult()
                                    {
                                        ResultCode = InvCountTranResultCode.INVENTORYIDCONFLICT
                                    };
                                }

                                // set detail id, status and header invCount id
                                detail.InvCountLineId = $"{invCountId}-{i + 1}";
                                detail.InvCountLineStatusId = (InvCountLneStatus.CREATED).ToString();
                                detail.InvCountId = invCountId;
                                //detail.InventoryId = 

                                // create detail
                                bool dtlSaved = await InvCountDetailRepo.CreateInvCountDetailMod(db, detail);

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return new InvCountCreateTranResult()
                                    {
                                        ResultCode = InvCountTranResultCode.COUNTDOCLINESAVEFAILED
                                    };
                                }
                            }
                        }

                        return new InvCountCreateTranResult()
                        {
                            ResultCode = InvCountTranResultCode.SUCCESS,
                            InvCountId = invCountId
                        };
                    }
                }
            }

            return new InvCountCreateTranResult()
            {
                ResultCode = InvCountTranResultCode.FAILED
            };
        }

        // MAY ISSUE PA
        public async Task<bool> CreateInvCount(IDbConnection db, InvCountModel invCount)
        {
            // define invCount status
            invCount.InvCountStatusId = (InvCountStatus.CREATED).ToString();

            string strQry = @"insert into invCount(invCountId, 
														invCountStatusId, 
														warehouseId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@invCountId, 
														@invCountStatusId, 
														@warehouseId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, invCount);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditADD(invCount, TranType.INVCNT);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        // MAY ISSUE PA
        public async Task<bool> InvIDExistsInInvCount(IDbConnection db, string inventoryId, string invCountId)
        {
            string strQry = @"SELECT COUNT(inventoryId) FROM invCountDetail details
                                INNER JOIN invCount header 
                                    ON details.invCountId = details.invCountId 
                                WHERE inventoryId = @inventoryId 
                                    AND details.invCountLineStatusId = 'CREATED';";

            var param = new DynamicParameters();
            param.Add("@inventoryId", inventoryId);
            param.Add("@invCountId", invCountId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);
            if (res == 0)
            {
                return false;
            }

            // default true to ensure no conflict will occur on error
            return true;
        }

        #region MoveQtyIsValid
        //private async Task<bool> MoveQtyIsValid(IDbConnection db, string inventoryId, int? qtyTo)
        //{
        //    string strQry = @"SELECT count(inventoryId)
        //                        FROM inventoryhistory ih
        //                        WHERE ih.inventoryId = @inventoryId AND @qtyTo > ih.qtyTo";

        //    var param = new DynamicParameters();
        //    param.Add("@inventoryId", inventoryId);
        //    param.Add("@qtyTo", qtyTo);

        //    var res = await db.ExecuteScalarAsync<int>(strQry, param);
        //    if (res == 0)
        //    {
        //        return false;
        //    }

        //    // default true to ensure no conflict will occur on error
        //    return true;
        //}
        #endregion

        // MAY ISSUE PA
        public async Task<InvCountTranResultCode> UpdateInvCountMod(InvCountModelMod invCount)
        {
            if (invCount.InvCountHeader != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // update header
                    var modHeader = await UpdateInvCount(db, invCount.InvCountHeader, TranType.INVCNT);

                    if (modHeader)
                    {
                        //// update InvMove user fields values
                        //if (invMove.InvMoveUfields != null)
                        //{
                        //    var uFieldsCreated = await InvMoveUFieldRepo.UpdateinvMoveUFieldMOD(db, invMove.InvMoveHeader.InvMoveId, invMove.InvMoveHeader.ModifiedBy, invMove.InvMoveUfields);
                        //    if (!uFieldsCreated)
                        //    {
                        //        return InvMoveTranResultCode.USRFIELDSAVEFAILED;
                        //    }
                        //}

                        // update detail
                        if (invCount.InvCountDetails != null && invCount.InvCountDetails.Any())
                        {
                            var details = invCount.InvCountDetails.ToList();

                            // get last InvMove detail line number
                            var invCountDetailsFromDb = await InvCountDetailRepo.LockInvCountDetails(db, invCount.InvCountHeader.InvCountId);
                            var lastinvMoveLneId = invCountDetailsFromDb.OrderByDescending(x => x.InvCountLineId).Select(y => y.InvCountLineId).FirstOrDefault();
                            int lastLneNum = 0;

                            if (!string.IsNullOrEmpty(lastinvMoveLneId))
                            {
                                lastLneNum = Convert.ToInt32(lastinvMoveLneId.Substring(lastinvMoveLneId.LastIndexOf('-') + 1));
                            }
                            else
                            {
                                lastLneNum = 0;
                            }

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];
                                bool dtlSaved = false;

                                // check if current and target location is not the same
                                //if (detail.LocationTo == detail.LocationFrom)
                                //{
                                //    return InvMoveTranResultCode.TARGETLOCCONFLICT;
                                //}

                                if (detail.InvCountLineId == null)
                                {
                                    // check if similar Inventory exists under this InvCount
                                    var invIDExists = await InvIDExistsInInvCount(db, detail.InventoryId, invCount.InvCountHeader.InvCountId);
                                    if (invIDExists)
                                    {
                                        return InvCountTranResultCode.INVENTORYIDCONFLICT;
                                    }

                                    // detail considered as new
                                    // set detail id, status and header po id
                                    lastLneNum += 1;
                                    detail.InvCountLineId = $"{invCount.InvCountHeader.InvCountId}-{lastLneNum}";
                                    detail.InvCountLineStatusId = (InvCountLneStatus.CREATED).ToString();
                                    detail.InvCountId = invCount.InvCountHeader.InvCountId;

                                    // create detail
                                    dtlSaved = await InvCountDetailRepo.CreateInvCountDetailMod(db, detail);
                                }
                                else
                                {
                                    // update existing details
                                    var prevDetail = await InvCountDetailRepo.GetInvCountDetailByIdMod(db, detail.InvCountLineId);

                                    if (prevDetail.InvCountLineStatusId == (InvCountLneStatus.CREATED).ToString())
                                    {
                                        if (prevDetail != detail)
                                        {
                                            dtlSaved = await InvCountDetailRepo.UpdateInvCountDetailMod(db, detail, TranType.INVCNT);
                                        }
                                    }
                                }

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return InvCountTranResultCode.COUNTDOCLINESAVEFAILED;
                                }
                            }
                        }
                        return InvCountTranResultCode.SUCCESS;
                    }
                }
            }
            return InvCountTranResultCode.FAILED;
        }

        // MAY ISSUE PA
        public async Task<bool> UpdateInvCount(IDbConnection db, InvCountModel invCount, TranType tranTyp)
            {
            string strQry = @"update InvCount set 
							                invCountStatusId = @invCountStatusId, 
                                            warehouseId = @warehouseId,
							                modifiedBy = @modifiedBy, 
							                remarks = @remarks where 
							                invCountId = @invCountId";

            int res = await db.ExecuteAsync(strQry, invCount);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditMOD(invCount, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteInvCount(string invCountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvCount where 
														invCountId = @invCountId";
                var param = new DynamicParameters();
                param.Add("@invCountId", invCountId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelInvCountResultCode> CancelInvCount(string invCountId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get InvCount details
                var invCountDetails = await InvCountDetailRepo.LockInvCountDetails(db, invCountId);

                if (invCountDetails == null || !invCountDetails.Any())
                {
                    return CancelInvCountResultCode.INVCOUNTDETAILLOCKFAILED;
                }

                // check if InvCount details is all in create status
                var mods = invCountDetails.Where(y => y.InvCountLineStatusId != (InvCountLneStatus.CREATED).ToString());
                if (mods.Any())
                {
                    return CancelInvCountResultCode.INVCOUNTDETAILSSTATUSALTERED;
                }

                // lock InvCount header
                var invCount = await LockInvCount(db, invCountId);
                if (invCount == null)
                {
                    return CancelInvCountResultCode.INVCOUNTLOCKFAILED;
                }

                // check if InvCount header is in create status
                if (invCount.InvCountStatusId != (InvCountStatus.CREATED).ToString())
                {
                    return CancelInvCountResultCode.INVCOUNTSTATUSALTERED;
                }

                // update InvCount status into canceled
                invCount.InvCountStatusId = (InvCountStatus.CANCELLED).ToString();
                var poAltered = await UpdateInvCount(db, invCount, TranType.INVCNT);

                if (!poAltered)
                {
                    return CancelInvCountResultCode.INVCOUNTSTATUSUPDATEFAILED;
                }

                // update InvCount details staus
                int alteredDtlCnt = 0;
                foreach (var invCountDetail in invCountDetails)
                {
                    invCountDetail.InvCountLineStatusId = (InvCountLneStatus.CLOSED).ToString();
                    var invCountDtlAltered = await InvCountDetailRepo.UpdateInvCountDetailMod(db, invCountDetail, TranType.CANCELCNT);

                    if (!invCountDtlAltered)
                    {
                        return CancelInvCountResultCode.INVCOUNTDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == invCountDetails.Count())
                {
                    return CancelInvCountResultCode.SUCCESS;
                }
            }

            return CancelInvCountResultCode.FAILED;
        }

        #region ForceCancelInvMove
        //public async Task<CancelInvMoveResultCode> ForceCancelInvMove(string invMoveId, string userAccountId)
        //{
        //    using (IDbConnection db = new MySqlConnection(ConnString))
        //    {
        //        db.Open();

        //        // lock and get InvMove details
        //        var invMoveDetails = await InvMoveDetailRepo.LockInvMoveDetails(db, invMoveId);

        //        if (invMoveDetails == null || !invMoveDetails.Any())
        //        {
        //            return CancelInvMoveResultCode.INVMOVEDETAILLOCKFAILED;
        //        }

        //        // check if InvMove details is valid for force close process
        //        var detailsValid = await ChkInvMoveDtlsCanFClose(invMoveDetails);
        //        if (!detailsValid)
        //        {
        //            return CancelInvMoveResultCode.INVMOVEDETAILSNOTVALID;
        //        }

        //        // check InvMove details if there's no pending task
        //        var hasPendingMoveTask = await MovementTaskRepoSub.HasPendingMovementTask(db, invMoveId);
        //        if (hasPendingMoveTask)
        //        {
        //            return CancelInvMoveResultCode.HASMOVETASKPENDING;
        //        }

        //        // lock InvMove header
        //        var po = await LockInvMove(db, invMoveId);
        //        if (po == null)
        //        {
        //            return CancelInvMoveResultCode.INVMOVELOCKFAILED;
        //        }

        //        // check if InvMove header is in partial move status
        //        if (po.InvMoveStatusId != (InvMoveStatus.PRTMV).ToString())
        //        {
        //            return CancelInvMoveResultCode.INVMOVESTATUSNOTVALID;
        //        }

        //        // update InvMove status into forced closed
        //        po.InvMoveStatusId = (InvMoveStatus.FRCCLOSED).ToString();
        //        var invMoveAltered = await UpdateInvMove(db, po, TranType.INVMOV);

        //        if (!invMoveAltered)
        //        {
        //            return CancelInvMoveResultCode.INVMOVESTATUSUPDATEFAILED;
        //        }

        //        // update InvMove details status
        //        int alteredDtlCnt = 0;
        //        foreach (var invMoveDetail in invMoveDetails)
        //        {
        //            invMoveDetail.InvMoveLineStatusId = (InvMoveLneStatus.FRCCLOSED).ToString();
        //            var poDtlAltered = await InvMoveDetailRepo.UpdateInvMoveDetailMod(db, invMoveDetail, TranType.CANCELMV);

        //            if (!poDtlAltered)
        //            {
        //                return CancelInvMoveResultCode.INVMOVEDETAILSSTATUSUPDATEFAILED;
        //            }

        //            alteredDtlCnt += 1;
        //        }

        //        if (alteredDtlCnt == invMoveDetails.Count())
        //        {
        //            return CancelInvMoveResultCode.SUCCESS;
        //        }
        //    }

        //    return CancelInvMoveResultCode.FAILED;
        //}
        #endregion

        #region ChkInvMoveDtlsCanFClose
        //private async Task<bool> ChkInvMoveDtlsCanFClose(IEnumerable<InvMoveDetailModel>? invMoveDetails)
        //{
        //    return await Task.Run(() =>
        //    {
        //        // check if InvMove contains force close-able details
        //        var dtlCreateCnt = invMoveDetails.Where(x => x.InvMoveLineStatusId == (InvMoveLneStatus.CREATED).ToString()).Count();
        //        var dtlPrtMvCnt = invMoveDetails.Where(x => x.InvMoveLineStatusId == (InvMoveLneStatus.PRTMV).ToString()).Count();
        //        var dtlFullMvCnt = invMoveDetails.Where(x => x.InvMoveLineStatusId == (InvMoveLneStatus.COMPLETED).ToString()).Count();

        //        if (dtlPrtMvCnt > 0)
        //        {
        //            return true;
        //        }
        //        else if (dtlCreateCnt > 0 && dtlFullMvCnt > 0)
        //        {
        //            return true;
        //        }

        //        return false;
        //    });
        //}
        #endregion

        #region InvMoveMovable
        //public async Task<bool> InvMoveMovable(string invMoveId)
        //{
        //    using (IDbConnection db = new MySqlConnection(ConnString))
        //    {
        //        db.Open();
        //        string strQry = @"select count(invMoveId) from InvMove where 
        //                                                (invMoveStatusId = 'CREATED' or 
        //                                                invMoveStatusId = 'PRTMV') and 
        //						invMoveId = @invMoveId";

        //        var param = new DynamicParameters();
        //        param.Add("@invMoveId", invMoveId);

        //        var res = await db.ExecuteScalarAsync<bool>(strQry, param);
        //        if (res)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}
        #endregion
    }
}
