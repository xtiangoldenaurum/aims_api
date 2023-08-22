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
    public class InvMoveRepository : IInvMoveRepository
    {
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        IInvMoveDetailRepository InvMoveDetailRepo;
        IInvMoveUserFieldRepository InvMoveUFieldRepo;
        IAuditTrailRepository AuditTrailRepo;
        MovementTaskRepoSub MovementTaskRepoSub;
        InvMoveAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public InvMoveRepository(ITenantProvider tenantProvider,
                            IAuditTrailRepository auditTrailRepo,
                            IIdNumberRepository idNumberRepo,
                            IInvMoveDetailRepository invMoveDetailsRepo,
                            IInvMoveUserFieldRepository invMoveUFieldRepo,
                            MovementTaskRepoSub movementTaskRepoSub)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            InvMoveDetailRepo = invMoveDetailsRepo;
            InvMoveUFieldRepo = invMoveUFieldRepo;
            MovementTaskRepoSub = movementTaskRepoSub;
            PagingRepo = new PagingRepository();
            AuditBuilder = new InvMoveAudit();
        }


        public async Task<InvMovePagedMdl?> GetInvMovePaged(int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                int offset = (pageNum - 1) * pageItem;
                string strQry = @"select invmove.*, 
                                            invmovestatus.invMoveStatus 
                                    from invmove 
                                    inner join invmovestatus on invmove.invMoveStatusId = invmovestatus.invMoveStatusId 
                                    order by invMoveId 
                                    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvMoveModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvMovePageDetail(db, pageNum, pageItem, ret.Count());

                    return new InvMovePagedMdl()
                    {
                        Pagination = pageDetail,
                        InvMove = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetInvMovePageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = "select count(invMoveId) from invmove";
            return await PagingRepo.GetPageDetail(db, strQry, pageNum, pageItem, rowCount);
        }

        public async Task<InvMovePagedMdl?> GetInvMoveFilteredPaged(InvMoveFilteredMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select invmove.*, invmovestatus.invMoveStatus from invmove";
            string strFltr = " where ";
            var param = new DynamicParameters();

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(invMoveId) from invmove";

            if (!string.IsNullOrEmpty(filter.InvMoveId))
            {
                strFltr += $"invMoveId = @invMoveId ";
                param.Add("@invMoveId", filter.InvMoveId);
            }

            if (!string.IsNullOrEmpty(filter.InvMoveStatusId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"invmove.invMoveStatusId = @invMoveStatusId ";
                param.Add("@invMoveStatusId", filter.InvMoveStatusId);
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
            string strJoins = @" inner join invmovestatus on invmove.invMoveStatusId = invMoveStatus.invMoveStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by invMoveId";
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

                var ret = await db.QueryAsync<InvMoveModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new InvMovePagedMdl()
                    {
                        Pagination = pageDetail,
                        InvMove = ret
                    };
                }
            }

            return null;
        }

        public async Task<InvMovePagedMdl?> GetInvMoveForMvPaged(int pageNum, int pageItem)
        {
            string strQry = "select invmove.*, invmovestatus.invmoveStatus from invmove";
            string strFltr = @" where invmove.invMoveStatusId = @statsCreated or 
                                        invmove.invMoveStatusId = @statsPartMv ";

            var param = new DynamicParameters();
            param.Add("@statsCreated", (InvMoveStatus.CREATED).ToString());
            param.Add("@statsPartMv", (InvMoveStatus.PRTMV).ToString());

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(invMoveId) from invmove";

            // build inner joins
            string strJoins = @" inner join invmovestatus on invmove.invMoveStatusId = invMoveStatus.invMoveStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by invMoveId desc";
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

                var ret = await db.QueryAsync<InvMoveModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new InvMovePagedMdl()
                    {
                        Pagination = pageDetail,
                        InvMove = ret
                    };
                }
            }

            return null;
        }

        public async Task<InvMovePagedMdl?> GetInvMoveSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invmove.*, invmovestatus.invMoveStatus from InvMove 
                                                inner join invmovestatus on invmove.invMoveStatusId = invmovestatus.invMoveStatusId 
                                                where invmove.invMoveId like @searchKey or 
                                                invmove.invMoveStatusId like @searchKey or
                                                invmove.warehouseId like @searchKey or
                                                invmove.reasonCodeId like @searchKey or
                                                invmove.reason like @searchKey or
                                                invmove.dateCreated like @searchKey or 
                                                invmove.dateModified like @searchKey or 
                                                invmove.createdBy like @searchKey or 
                                                invmove.modifiedBy like @searchKey or 
                                                invmove.remarks like @searchKey or 
                                                invmovestatus.invMoveStatus like @searchKey 
                                                limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvMoveModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvMoveSrchPageDetail(db, searchKey, pageNum, pageItem, ret.Count());

                    return new InvMovePagedMdl()
                    {
                        Pagination = pageDetail,
                        InvMove = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetInvMoveSrchPageDetail(IDbConnection db, string searchKey, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"select count(invMoveId) from InvMove 
                                    inner join invmovestatus on invmove.invMoveStatusId = invmovestatus.invMoveStatusId 
                               where invmove.invMoveId like @searchKey or 
                                    invmove.invMoveStatusId like @searchKey or
                                    invmove.warehouseId like @searchKey or
                                    invmove.reason like @searchKey or
                                    invmove.dateCreated like @searchKey or 
                                    invmove.dateModified like @searchKey or 
                                    invmove.createdBy like @searchKey or 
                                    invmove.modifiedBy like @searchKey or 
                                    invmove.remarks like @searchKey or 
                                    invmovestatus.invMoveStatus like @searchKey";

            var param = new DynamicParameters();
            param.Add("@searchKey", $" %{ searchKey}% ");

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<InvMoveModel>> GetInvMovePg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvMove limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvMoveModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvMoveModel>> GetInvMovePgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvMove where 
                                     invMoveId like @searchKey or 
                                     invmove.invMoveStatusId like @searchKey or
                                     invmove.warehouseId like @searchKey or
                                     invmove.reasonCodeId like @searchKey or
                                     invmove.reason like @searchKey or
                                     invmove.dateCreated like @searchKey or 
                                     invmove.dateModified like @searchKey or 
                                     invmove.createdBy like @searchKey or 
                                     invmove.modifiedBy like @searchKey or 
                                     invmove.remarks like @searchKey
                limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvMoveModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvMoveModel> GetInvMoveById(string invMoveId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT * FROM invmove 
                                    INNER JOIN invmovestatus stats 
	                                    ON invmove.invMoveStatusId = stats.invMoveStatusId 
                                    WHERE invMoveId = @invMoveId;";

                var param = new DynamicParameters();
                param.Add("@invMoveId", invMoveId);
                return await db.QuerySingleOrDefaultAsync<InvMoveModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> InvMoveExists(string invMoveId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invMoveId from InvMove where 
														invMoveId = @invMoveId";

                var param = new DynamicParameters();
                param.Add("@invMoveId", invMoveId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InvMoveModel> LockInvMove(IDbConnection db, string invMoveId)
        {
            string strQry = @"SELECT * FROM invmove WHERE invMoveId = @invMoveId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@invMoveId", invMoveId);

            return await db.QuerySingleOrDefaultAsync<InvMoveModel>(strQry, param);
        }

        public async Task<string?> GetInvMoveUpdatedStatus(IDbConnection db, string invMoveId)
        {
            string strQry = @"call `spGetInvMoveUpdatedStatus`(@paramInvMoveId);";

            var param = new DynamicParameters();
            param.Add("@paramInvMoveId", invMoveId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        public async Task<InvMoveCreateTranResult> CreateInvMoveMod(InvMoveModelMod invMove)
        {
            // get InvMove id number
            var invMoveId = await IdNumberRepo.GetNextIdNum("INVMOV");

            if (!string.IsNullOrEmpty(invMoveId) &&
                invMove.InvMoveHeader != null &&
                invMove.InvMoveDetails != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // create header
                    invMove.InvMoveHeader.InvMoveId = invMoveId;
                    var headCreated = await CreateInvMove(db, invMove.InvMoveHeader);

                    if (headCreated)
                    {
                        // init InvMove user fields default data
                        var initInvMoveUFld = await InvMoveUFieldRepo.InitInvMoveUField(db, invMoveId);
                        if (!initInvMoveUFld)
                        {
                            return new InvMoveCreateTranResult()
                            {
                                ResultCode = InvMoveTranResultCode.USRFIELDSAVEFAILED
                            };
                        }

                        // insert InvMove user fields values
                        if (invMove.InvMoveUfields != null)
                        {
                            var uFieldsCreated = await InvMoveUFieldRepo.UpdateinvMoveUField(db, invMoveId, invMove.InvMoveHeader.CreatedBy, invMove.InvMoveUfields);
                            if (!uFieldsCreated)
                            {
                                return new InvMoveCreateTranResult()
                                {
                                    ResultCode = InvMoveTranResultCode.USRFIELDSAVEFAILED
                                };
                            }
                        }

                        // create detail
                        if (invMove.InvMoveDetails.Any())
                        {
                            var details = invMove.InvMoveDetails.ToList();

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];

                                // check if similar Inventory ID exists under this Movement
                                var invIDExists = await InvIDExistsInInvMove(db, detail.InventoryId, invMoveId);
                                if (invIDExists)
                                {
                                    return new InvMoveCreateTranResult()
                                    {
                                        ResultCode = InvMoveTranResultCode.INVENTORYIDCONFLICT
                                    };
                                }

                                // check if qty is valid
                                if (detail.QtyTo != null)
                                {
                                    var moveQty = await MoveQtyIsValid(db, detail.InventoryId, detail.QtyTo);
                                    if (moveQty)
                                    {
                                        return new InvMoveCreateTranResult()
                                        {
                                            ResultCode = InvMoveTranResultCode.INVALIDQTY
                                        };
                                    }
                                }

                                // set detail id, status and header invMove id
                                detail.InvMoveLineId = $"{invMoveId}-{i + 1}";
                                detail.InvMoveLineStatusId = (InvMoveLneStatus.CREATED).ToString();
                                detail.InvMoveId = invMoveId;
                                //detail.InventoryId = 

                                // create detail
                                bool dtlSaved = await InvMoveDetailRepo.CreateInvMoveDetailMod(db, detail);

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return new InvMoveCreateTranResult()
                                    {
                                        ResultCode = InvMoveTranResultCode.MOVEMENTDOCLINESAVEFAILED
                                    };
                                }
                            }
                        }

                        return new InvMoveCreateTranResult()
                        {
                            ResultCode = InvMoveTranResultCode.SUCCESS,
                            InvMoveId = invMoveId
                        };
                    }
                }
            }

            return new InvMoveCreateTranResult()
            {
                ResultCode = InvMoveTranResultCode.FAILED
            };
        }

        public async Task<bool> CreateInvMove(IDbConnection db, InvMoveModel invMove)
        {
            // define InvMove status
            invMove.InvMoveStatusId = (InvMoveStatus.CREATED).ToString();

            string strQry = @"insert into invmove(invMoveId, 
														invMoveStatusId, 
														warehouseId, 
														reasonCodeId, 
                                                        reason,
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@invMoveId, 
														@invMoveStatusId, 
														@warehouseId, 
														@reasonCodeId, 
                                                        @reason,
														@createdBy, 
														@modifiedBy, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, invMove);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditADD(invMove, TranType.INVMOV);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> InvIDExistsInInvMove(IDbConnection db, string inventoryId, string invMoveId)
        {
            string strQry = @"select count(inventoryId) from invMoveDetail 
                                where inventoryId = @inventoryId and 
                                        invMoveId = @invMoveId";

            var param = new DynamicParameters();
            param.Add("@inventoryId", inventoryId);
            param.Add("@invMoveId", invMoveId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);
            if (res == 0)
            {
                return false;
            }

            // default true to ensure no conflict will occur on error
            return true;
        }

        private async Task<bool> MoveQtyIsValid(IDbConnection db, string inventoryId, int? qtyTo)
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

        public async Task<InvMoveTranResultCode> UpdateInvMoveMod(InvMoveModelMod invMove)
        {
            if (invMove.InvMoveHeader != null )
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // update header
                    var modHeader = await UpdateInvMove(db, invMove.InvMoveHeader, TranType.INVMOV);

                    if (modHeader)
                    {
                        // update InvMove user fields values
                        if (invMove.InvMoveUfields != null)
                        {
                            var uFieldsCreated = await InvMoveUFieldRepo.UpdateinvMoveUFieldMOD(db, invMove.InvMoveHeader.InvMoveId, invMove.InvMoveHeader.ModifiedBy, invMove.InvMoveUfields);
                            if (!uFieldsCreated)
                            {
                                return InvMoveTranResultCode.USRFIELDSAVEFAILED;
                            }
                        }

                        // update detail
                        if (invMove.InvMoveDetails != null && invMove.InvMoveDetails.Any())
                        {
                            var details = invMove.InvMoveDetails.ToList();

                            // get last InvMove detail line number
                            var invMoveDetailsFromDb = await InvMoveDetailRepo.LockInvMoveDetails(db, invMove.InvMoveHeader.InvMoveId);
                            var lastinvMoveLneId = invMoveDetailsFromDb.OrderByDescending(x => x.InvMoveLineId).Select(y => y.InvMoveLineId).FirstOrDefault();
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
                                if (detail.LocationTo == detail.LocationFrom)
                                {
                                    return InvMoveTranResultCode.TARGETLOCCONFLICT;
                                }

                                    if (detail.InvMoveLineId == null)
                                {
                                    // check if similar Inventory exists under this InvMove
                                    var invIDExists = await InvIDExistsInInvMove(db, detail.InventoryId, invMove.InvMoveHeader.InvMoveId);
                                    if (invIDExists)
                                    {
                                        return InvMoveTranResultCode.INVENTORYIDCONFLICT;
                                    }

                                    // detail concidered as new
                                    // set detail id, status and header po id
                                    lastLneNum += 1;
                                    detail.InvMoveLineId = $"{invMove.InvMoveHeader.InvMoveId}-{lastLneNum}";
                                    detail.InvMoveLineStatusId = (InvMoveLneStatus.CREATED).ToString();
                                    detail.InvMoveId = invMove.InvMoveHeader.InvMoveId;

                                    // create detail
                                    dtlSaved = await InvMoveDetailRepo.CreateInvMoveDetailMod(db, detail);
                                }
                                else
                                {
                                    // update existing details
                                    var prevDetail = await InvMoveDetailRepo.GetInvMoveDetailByIdMod(db, detail.InvMoveLineId);

                                    if (prevDetail.InvMoveLineStatusId == (InvMoveLneStatus.CREATED).ToString())
                                    {
                                        if (prevDetail != detail)
                                        {
                                            dtlSaved = await InvMoveDetailRepo.UpdateInvMoveDetailMod(db, detail, TranType.INVMOV);
                                        }
                                    }
                                }

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return InvMoveTranResultCode.MOVEMENTDOCLINESAVEFAILED;
                                }
                            }
                        }
                        return InvMoveTranResultCode.SUCCESS;
                    }
                }
            }
            return InvMoveTranResultCode.FAILED;
        }

        public async Task<bool> UpdateInvMove(IDbConnection db, InvMoveModel invMove, TranType tranTyp)
        {
            string strQry = @"update InvMove set 
							                invMoveStatusId = @invMoveStatusId, 
                                            warehouseId = @warehouseId,
							                reasonCodeId = @reasonCodeId, 
                                            reason = @reason,
							                modifiedBy = @modifiedBy, 
							                remarks = @remarks where 
							                invMoveId = @invMoveId";

            int res = await db.ExecuteAsync(strQry, invMove);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditMOD(invMove, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteInvMove(string invMoveId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvMove where 
														invMoveId = @invMoveId";
                var param = new DynamicParameters();
                param.Add("@invMoveId", invMoveId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelInvMoveResultCode> CancelInvMove(string invMoveId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get InvMove details
                var invMoveDetails = await InvMoveDetailRepo.LockInvMoveDetails(db, invMoveId);

                if (invMoveDetails == null || !invMoveDetails.Any())
                {
                    return CancelInvMoveResultCode.INVMOVEDETAILLOCKFAILED;
                }

                // check if InvMove details is all in create status
                var mods = invMoveDetails.Where(y => y.InvMoveLineStatusId != (InvMoveLneStatus.CREATED).ToString());
                if (mods.Any())
                {
                    return CancelInvMoveResultCode.INVMOVEDETAILSSTATUSALTERED;
                }

                // lock InvMove header
                var invMove = await LockInvMove(db, invMoveId);
                if (invMove == null)
                {
                    return CancelInvMoveResultCode.INVMOVELOCKFAILED;
                }

                // check if InvMove header is in create status
                if (invMove.InvMoveStatusId != (InvMoveStatus.CREATED).ToString())
                {
                    return CancelInvMoveResultCode.INVMOVESTATUSALTERED;
                }

                // update InvMove status into canceled
                invMove.InvMoveStatusId = (InvMoveStatus.CANCELLED).ToString();
                var poAltered = await UpdateInvMove(db, invMove, TranType.INVMOV);

                if (!poAltered)
                {
                    return CancelInvMoveResultCode.INVMOVESTATUSUPDATEFAILED;
                }

                // update InvMove details staus
                int alteredDtlCnt = 0;
                foreach (var invMoveDetail in invMoveDetails)
                {
                    invMoveDetail.InvMoveLineStatusId = (InvMoveLneStatus.CLOSED).ToString();
                    var invMoveDtlAltered = await InvMoveDetailRepo.UpdateInvMoveDetailMod(db, invMoveDetail, TranType.CANCELMV);

                    if (!invMoveDtlAltered)
                    {
                        return CancelInvMoveResultCode.INVMOVEDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == invMoveDetails.Count())
                {
                    return CancelInvMoveResultCode.SUCCESS;
                }
            }

            return CancelInvMoveResultCode.FAILED;
        }

        public async Task<CancelInvMoveResultCode> ForceCancelInvMove(string invMoveId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get InvMove details
                var invMoveDetails = await InvMoveDetailRepo.LockInvMoveDetails(db, invMoveId);

                if (invMoveDetails == null || !invMoveDetails.Any())
                {
                    return CancelInvMoveResultCode.INVMOVEDETAILLOCKFAILED;
                }

                // check if InvMove details is valid for force close process
                var detailsValid = await ChkInvMoveDtlsCanFClose(invMoveDetails);
                if (!detailsValid)
                {
                    return CancelInvMoveResultCode.INVMOVEDETAILSNOTVALID;
                }

                // check InvMove details if there's no pending task
                var hasPendingMoveTask = await MovementTaskRepoSub.HasPendingMovementTask(db, invMoveId);
                if (hasPendingMoveTask)
                {
                    return CancelInvMoveResultCode.HASMOVETASKPENDING;
                }

                // lock InvMove header
                var po = await LockInvMove(db, invMoveId);
                if (po == null)
                {
                    return CancelInvMoveResultCode.INVMOVELOCKFAILED;
                }

                // check if InvMove header is in partial move status
                if (po.InvMoveStatusId != (InvMoveStatus.PRTMV).ToString())
                {
                    return CancelInvMoveResultCode.INVMOVESTATUSNOTVALID;
                }

                // update InvMove status into forced closed
                po.InvMoveStatusId = (InvMoveStatus.FRCCLOSED).ToString();
                var invMoveAltered = await UpdateInvMove(db, po, TranType.INVMOV);

                if (!invMoveAltered)
                {
                    return CancelInvMoveResultCode.INVMOVESTATUSUPDATEFAILED;
                }

                // update InvMove details status
                int alteredDtlCnt = 0;
                foreach (var invMoveDetail in invMoveDetails)
                {
                    invMoveDetail.InvMoveLineStatusId = (InvMoveLneStatus.FRCCLOSED).ToString();
                    var poDtlAltered = await InvMoveDetailRepo.UpdateInvMoveDetailMod(db, invMoveDetail, TranType.CANCELMV);

                    if (!poDtlAltered)
                    {
                        return CancelInvMoveResultCode.INVMOVEDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == invMoveDetails.Count())
                {
                    return CancelInvMoveResultCode.SUCCESS;
                }
            }

            return CancelInvMoveResultCode.FAILED;
        }

        private async Task<bool> ChkInvMoveDtlsCanFClose(IEnumerable<InvMoveDetailModel>? invMoveDetails)
        {
            return await Task.Run(() =>
            {
                // check if InvMove contains force close-able details
                var dtlCreateCnt = invMoveDetails.Where(x => x.InvMoveLineStatusId == (InvMoveLneStatus.CREATED).ToString()).Count();
                var dtlPrtMvCnt = invMoveDetails.Where(x => x.InvMoveLineStatusId == (InvMoveLneStatus.PRTMV).ToString()).Count();
                var dtlFullMvCnt = invMoveDetails.Where(x => x.InvMoveLineStatusId == (InvMoveLneStatus.COMPLETED).ToString()).Count();

                if (dtlPrtMvCnt > 0)
                {
                    return true;
                }
                else if (dtlCreateCnt > 0 && dtlFullMvCnt > 0)
                {
                    return true;
                }

                return false;
            });
        }

        public async Task<bool> InvMoveMovable(string invMoveId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select count(invMoveId) from InvMove where 
                                                        (invMoveStatusId = 'CREATED' or 
                                                        invMoveStatusId = 'PRTMV') and 
														invMoveId = @invMoveId";

                var param = new DynamicParameters();
                param.Add("@invMoveId", invMoveId);

                var res = await db.ExecuteScalarAsync<bool>(strQry, param);
                if (res)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
