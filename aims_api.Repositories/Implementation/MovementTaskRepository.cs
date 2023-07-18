using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
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
    public class MovementTaskRepository : IMovementTaskRepository
    {
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        IInvMoveRepository InvMoveRepo;
        IInvMoveDetailRepository InvMoveDetailRepo;
        IInventoryRepository InventoryRepo;
        IInventoryHistoryRepository InvHistoryRepo;
        IPagingRepository PagingRepo;
        IAuditTrailRepository AuditTrailRepo;
        MovementTaskAudit AuditBuilder;
        public MovementTaskRepository(ITenantProvider tenantProvider,
                                    IAuditTrailRepository auditTrailRepo,
                                    IIdNumberRepository idNumberRepo,
                                    IInvMoveRepository invMoveRepo,
                                    IInvMoveDetailRepository invMoveDetailRepo,
                                    IInventoryRepository inventoryRepo,
                                    IInventoryHistoryRepository invHistoryRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            InvMoveRepo = invMoveRepo;
            InvMoveDetailRepo = invMoveDetailRepo;
            InventoryRepo = inventoryRepo;
            InvHistoryRepo = invHistoryRepo;
            PagingRepo = new PagingRepository();
            AuditBuilder = new MovementTaskAudit();
        }

        public async Task<IEnumerable<MovementTaskModel>> GetMovementTaskPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from Receiving limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<MovementTaskModel>(strQry, param, commandType: CommandType.Text);
            }
        }
        public async Task<MovementTaskPagedMdl?> GetCancelableMv(int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetCancelableMvs`(@pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                var ret = await db.QueryAsync<InvMoveMovementTaskDetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "CALL `spCountCancelableMv`()";

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new MovementTaskPagedMdl()
                    {
                        Pagination = pageDetail,
                        Moves = ret
                    };
                }
            }

            return null;
        }

        public async Task<MovementTaskPagedMdl?> GetMovementTasksByInvMoveId(string invMoveId, int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetMovesByInvMoveId`(@invMoveId, @pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@invMoveId", invMoveId);
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                var ret = await db.QueryAsync<InvMoveMovementTaskDetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "CALL `spCountMovesByInvMoveId`(@invMoveId)";

                    var pgParam = new DynamicParameters();
                    pgParam.Add("@invMoveId", invMoveId);

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new MovementTaskPagedMdl()
                    {
                        Pagination = pageDetail,
                        Moves = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<InvMoveMovementTaskDetailModel>?> GetCancelableMvsById(string invMoveLineId, int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetCancelableMvsById`(@invMoveDetailId, @pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@invMoveDetailId", invMoveLineId);
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                return await db.QueryAsync<InvMoveMovementTaskDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvMoveMovementTaskDetailModel>?> GetCancelableMvsByInvMoveId(string invMoveId, int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetCancelableMvsByInvMoveId`(@invMoveId, @pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@invMoveId", invMoveId);
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                return await db.QueryAsync<InvMoveMovementTaskDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<MovementTaskModel>> GetMovementTaskPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from MovementTask where 
														movementTaskId like @searchKey or 
														docLineId like @searchKey or 
														inventoryId like @searchKey or 
														seqNum like @searchKey or 
                                                        movementStatusId like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<MovementTaskModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<MovementTaskModel> GetMovementTaskById(string movementTaskId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from MovementTask where 
														movementTaskId = @movementTaskId";

                var param = new DynamicParameters();
                param.Add("@movementTaskId", movementTaskId);
                return await db.QuerySingleOrDefaultAsync<MovementTaskModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<MovementTaskModel> GetMovementTaskByIdMod(IDbConnection db, string movementTaskId)
        {
            string strQry = @"select * from MovementTask where 
														movementTaskId = @movementTaskId";

            var param = new DynamicParameters();
            param.Add("@movementTaskId", movementTaskId);

            return await db.QuerySingleOrDefaultAsync<MovementTaskModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> MovementTaskExists(string movementTaskId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select movementTaskId from MovementTask where 
														movementTaskId = @movementTaskId";

                var param = new DynamicParameters();
                param.Add("@movementTaskId", movementTaskId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public Task<MovementTaskResultModel> MovementTask(MovementTaskModelMod data)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateMovementTask(MovementTaskModel movementTask)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into MovementTask(movementTaskId, 
														docLineId, 
														inventoryId, 
														seqNum, 
                                                        movementStatusId, 
														createdBy, 
														modifiedBy)
 												values(@movementTaskId, 
														@docLineId, 
														@inventoryId, 
														@seqNum, 
                                                        @movementStatusId, 
														@createdBy, 
														@modifiedBy)";

                int res = await db.ExecuteAsync(strQry, movementTask);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateMovementTaskMod(IDbConnection db, MovementTaskModel movementTask, TranType tranTyp)
        {
            string strQry = @"insert into MovementTask(movementTaskId, 
														docLineId, 
														inventoryId, 
														seqNum, 
                                                        movementStatusId, 
														createdBy, 
														modifiedBy)
 												values(@movementTaskId, 
														@docLineId, 
														@inventoryId, 
														@seqNum, 
                                                        @movementStatusId, 
														@createdBy, 
														@modifiedBy)";

            int res = await db.ExecuteAsync(strQry, movementTask);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildMovementTaskAuditADD(movementTask, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateMovementTask(MovementTaskModel movementTask)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update movementtask set 
														docLineId = @docLineId, 
														inventoryId = @inventoryId, 
														seqNum = @seqNum, 
                                                        movementStatusId = @movementStatusId, 
														modifiedBy = @modifiedBy where 
														movementTaskId = @movementTaskId";

                int res = await db.ExecuteAsync(strQry, movementTask);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelMvResultCode> CancelMovementTask(string movementTaskId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock current movement detail
                var mvmntTaskDetail = await LockMovementTaskDetail(db, movementTaskId);
                if (mvmntTaskDetail == null)
                {
                    return CancelMvResultCode.MVLOCKFAIL; // cut process here if movement detail is not retreived
                }

                // lock current movement inv move detail
                var invMvDetail = await InvMoveDetailRepo.LockInvMoveDetail(db, mvmntTaskDetail.DocLineId);
                if (invMvDetail == null)
                {
                    return CancelMvResultCode.INVMOVEDETAILLOCKFAIL; // cut process here if po detail is not retreived
                }

                // lock current inv movement invmove header
                var invMoveHeader = await InvMoveRepo.LockInvMove(db, invMvDetail.InvMoveId);
                if (invMoveHeader == null)
                {
                    return CancelMvResultCode.INVMOVELOCKFAIL; // cut process here if po header is not retreived
                }

                // check if movement detail is still moveable
                if (mvmntTaskDetail.MovementStatusId != (MovementTaskStatus.CREATED).ToString())
                {
                    return CancelMvResultCode.MVINCONSISTENCY;
                }

                // check if inv move detail is in movement status
                if (invMvDetail.InvMoveLineStatusId == (InvMoveLneStatus.CREATED).ToString() ||
                    invMvDetail.InvMoveLineStatusId == (InvMoveLneStatus.FRCCLOSED).ToString() ||
                    invMvDetail.InvMoveLineStatusId == (InvMoveLneStatus.COMPLETED).ToString())
                {
                    return CancelMvResultCode.INVMOVEDTLINCONSISTENCY;
                }

                // check if invmove is still not completed, cancelled or in create status
                if (invMoveHeader.InvMoveStatusId == (InvMoveStatus.CREATED).ToString() ||
                    invMoveHeader.InvMoveStatusId == (InvMoveStatus.CANCELLED).ToString() ||
                    invMoveHeader.InvMoveStatusId == (InvMoveStatus.FRCCLOSED).ToString() ||
                    invMoveHeader.InvMoveStatusId == (InvMoveStatus.COMPLETED).ToString())
                {
                    return CancelMvResultCode.INVMOVEINCONSISTENCY;
                }

                // get most recent inventory history record
                var invHistDetail = await InvHistoryRepo.GetTopInvHistDetail(db, mvmntTaskDetail.InventoryId);

                if (invHistDetail != null)
                {
                    // lock inventory history detail
                    var tmpInvHist = await InvHistoryRepo.LockInvHistDetail(db, invHistDetail.InventoryId, invHistDetail.SeqNum);

                    if (tmpInvHist != null) // && invHistDetail == tmpInvHist) **skipped validation if invHist and lockedInvHist is the same**
                    {
                        // update inventoryhistory detail
                        invHistDetail.QtyFrom = invHistDetail.QtyTo;
                        invHistDetail.QtyTo = 0;
                        invHistDetail.SeqNum += 1;

                        // record new inventory history detail
                        var invHistSave = await InvHistoryRepo.CreateInventoryHistoryMod(db, invHistDetail, TranType.CANCELMV);

                        if (invHistSave)
                        {
                            // update invtory detail status to closed
                            var invClosed = await InventoryRepo.SetInventoryStatus(db, invHistDetail.InventoryId, (InvStatus.CLOSED).ToString(), userAccountId, TranType.CANCELMV);

                            if (invClosed)
                            {
                                // update movement status to canceled 
                                var rcvClosed = await SetMovementTaskStatus(db, movementTaskId, (MovementTaskStatus.CANCELLED).ToString(), TranType.CANCELRCV, userAccountId);

                                if (rcvClosed)
                                {
                                    // get invmove detail updated line status
                                    var invMoveDtlUpdatedStatus = await InvMoveDetailRepo.GetInvMoveDtlCancelMvUpdatedStatus(db, invMvDetail.InvMoveLineId, movementTaskId);

                                    if (!string.IsNullOrEmpty(invMoveDtlUpdatedStatus))
                                    {
                                        // commit update invmove detail status
                                        invMvDetail.ModifiedBy = userAccountId;
                                        invMvDetail.InvMoveLineStatusId = invMoveDtlUpdatedStatus;
                                        var invMoveDtlUpdated = await InvMoveDetailRepo.UpdateInvMoveDetailMod(db, invMvDetail, TranType.CANCELMV);

                                        if (invMoveDtlUpdated)
                                        {
                                            // get update invmove status
                                            var invMoveUpdatedStatus = await InvMoveRepo.GetInvMoveUpdatedStatus(db, invMoveHeader.InvMoveId);

                                            if (!string.IsNullOrEmpty(invMoveUpdatedStatus))
                                            {
                                                // save update invmove record
                                                invMoveHeader.ModifiedBy = userAccountId;
                                                invMoveHeader.InvMoveStatusId = invMoveUpdatedStatus;

                                                var invMoveUpdated = await InvMoveRepo.UpdateInvMove(db, invMoveHeader, TranType.CANCELMV);

                                                if (invMoveUpdated)
                                                {
                                                    return CancelMvResultCode.SUCCESS;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return CancelMvResultCode.FAILED;
        }

        public async Task<MovementTaskModel> LockMovementTaskDetail(IDbConnection db, string movementTaskId)
        {
            // hold current movement transaction
            string strQry = @"select * from movementtask where movementTaskId = @movementTaskId for update;";

            var param = new DynamicParameters();
            param.Add("@movementTaskId", movementTaskId);

            return await db.QuerySingleOrDefaultAsync<MovementTaskModel>(strQry, param);
        }

        public async Task<bool> SetMovementTaskStatus(string movementTaskId, string movementStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update movementtask set movementStatusId = @movementStatusId 
                                    where movementTaskId = @movementTaskId;";

                var param = new DynamicParameters();
                param.Add("@movementStatusId", movementStatusId);
                param.Add("@movementTaskId", movementTaskId);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> SetMovementTaskStatus(IDbConnection db, string movementTaskId, string movementStatusId, TranType tranTyp, string userAccountId)
        {
            string strQry = @"update movementtask set movementStatusId = @movementStatusId, 
                                                    modifiedBy = @modifiedBy
                                    where movementTaskId = @movementTaskId;";

            var param = new DynamicParameters();
            param.Add("@movementStatusId", movementStatusId);
            param.Add("@modifiedBy", userAccountId);
            param.Add("@movementTaskId", movementTaskId);

            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                // get updated movement record
                var mvment = await GetMovementTaskByIdMod(db, movementTaskId);

                if (mvment != null)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(mvment, tranTyp);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> DeleteMovementTask(string movementTaskId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from MovementTask where 
														movementTaskId = @movementTaskId";
                var param = new DynamicParameters();
                param.Add("@movementTaskId", movementTaskId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<MovementTaskModel> LockMovementTaskDetailRefMulti(IDbConnection db, string invMoveLineId, string inventoryId)
        {
            // hold current movement transaction
            string strQry = @"select * 
                                from movementtask 
                                where docLineId = @invMoveLineId and 
                                        inventoryId = @inventoryId and 
                                        seqnum = 1 and 
                                        movementStatusId = 'CREATED' 
                                for update;";

            var param = new DynamicParameters();
            param.Add("@docLineId", invMoveLineId);
            param.Add("@inventoryId", inventoryId);

            return await db.QuerySingleOrDefaultAsync<MovementTaskModel>(strQry, param);
        }
    }
}
