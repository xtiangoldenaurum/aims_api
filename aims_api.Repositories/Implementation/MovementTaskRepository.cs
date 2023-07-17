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

        public Task<IEnumerable<MovementTaskModel>> GetMovementTaskPgSrch(string searchKey, int pageNum, int pageItem)
        {
            throw new NotImplementedException();
        }

        public Task<MovementTaskModel> GetMovementTaskById(string movementTaskId)
        {
            throw new NotImplementedException();
        }

        public Task<MovementTaskModel> GetMovementTaskByIdMod(IDbConnection db, string movementTaskId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MovementTaskExists(string movementTaskId)
        {
            throw new NotImplementedException();
        }

        public Task<MovementTaskResultModel> MovementTask(MovementTaskModelMod data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateMovementTask(MovementTaskModel movementTask)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateMovementTaskMod(IDbConnection db, MovementTaskModel movementTask, TranType tranTyp)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateMovementTask(MovementTaskModel movementTask)
        {
            throw new NotImplementedException();
        }

        public Task<CancelMvResultCode> CancelMovementTask(string movementTaskId, string userAccountId)
        {
            throw new NotImplementedException();
        }

        public Task<MovementTaskModel> LockMovementTaskDetail(IDbConnection db, string movementTaskId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetMovementTaskStatus(string movementTaskId, string movementTaskStatus)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetMovementTaskStatus(IDbConnection db, string movementTaskId, string movementTaskStatus, TranType tranTyp, string userAccountId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMovementTask(string movementTaskId)
        {
            throw new NotImplementedException();
        }

        public Task<MovementTaskModel> LockMovementTaskDetailRefMulti(IDbConnection db, string invMoveLineId, string inventoryId)
        {
            throw new NotImplementedException();
        }
    }
}
