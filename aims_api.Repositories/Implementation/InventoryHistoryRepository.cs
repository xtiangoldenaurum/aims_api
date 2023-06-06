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
    public class InventoryHistoryRepository : IInventoryHistoryRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        InventoryHistAudit AuditBuilder;


        public InventoryHistoryRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new InventoryHistAudit();
        }

        public async Task<IEnumerable<InventoryHistoryModel>> GetInventoryHistoryPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InventoryHistory limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InventoryHistoryModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InventoryHistoryModel>> GetInventoryHistoryPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InventoryHistory where 
														inventoryId like @searchKey or 
														seqNum like @searchKey or 
														documentRefId like @searchKey or 
														qtyFrom like @searchKey or 
														qtyTo like @searchKey or 
														locationFrom like @searchKey or 
														locationTo like @searchKey or 
														trackIdFrom like @searchKey or 
														trackIdTo like @searchKey or 
														lpnFrom like @searchKey or 
														lpnTo like @searchKey or 
														lotAttributeId like @searchKey or 
														transactionTypeId like @searchKey or 
														remarks like @searchKey or 
														dateCreated like @searchKey or 
														createdBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InventoryHistoryModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InventoryHistoryModel> GetInventoryHistoryById(string inventoryId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InventoryHistory where 
														inventoryId = @inventoryId";

                var param = new DynamicParameters();
                param.Add("@inventoryId", inventoryId);
                return await db.QuerySingleOrDefaultAsync<InventoryHistoryModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> InventoryHistoryExists(string inventoryId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select inventoryId from InventoryHistory where 
														inventoryId = @inventoryId";

                var param = new DynamicParameters();
                param.Add("@inventoryId", inventoryId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInventoryHistory(InventoryHistoryModel inventoryHistory)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InventoryHistory(inventoryId, 
														seqNum, 
														documentRefId, 
														qtyFrom, 
														qtyTo, 
														locationFrom, 
														locationTo, 
														trackIdFrom, 
														trackIdTo, 
														lpnFrom, 
														lpnTo, 
														lotAttributeId, 
														transactionTypeId, 
														remarks, 
														createdBy)
 												values(@inventoryId, 
														@seqNum, 
														@documentRefId, 
														@qtyFrom, 
														@qtyTo, 
														@locationFrom, 
														@locationTo, 
														@trackIdFrom, 
														@trackIdTo, 
														@lpnFrom, 
														@lpnTo, 
														@lotAttributeId, 
														@transactionTypeId, 
														@remarks, 
														@createdBy)";

                int res = await db.ExecuteAsync(strQry, inventoryHistory);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InventoryHistoryModel> GetTopInvHistDetail(IDbConnection db, string inventoryId)
        {
            // hold currentr eceive transaction
            string strQry = @"call `spGetTopInvHistDetail`(@invId);";

            var param = new DynamicParameters();
            param.Add("@invId", inventoryId);

            return await db.QuerySingleOrDefaultAsync<InventoryHistoryModel>(strQry, param);
        }

        public async Task<InventoryHistoryModel> LockInvHistDetail(IDbConnection db, string inventoryId, int seqNum)
        {
            // hold currentr eceive transaction
            string strQry = @"select * from inventoryhistory where inventoryId = @inventoryId and seqNum = @seqNum for update;";

            var param = new DynamicParameters();
            param.Add("@inventoryId", inventoryId);
            param.Add("@seqNum", seqNum);

            return await db.QuerySingleOrDefaultAsync<InventoryHistoryModel>(strQry, param);
        }

        public async Task<bool> CreateInventoryHistoryMod(IDbConnection db, InventoryHistoryModel inventoryHistory, TranType tranTyp)
        {
            string strQry = @"insert into InventoryHistory(inventoryId, 
														seqNum, 
														documentRefId, 
														qtyFrom, 
														qtyTo, 
														locationFrom, 
														locationTo, 
														trackIdFrom, 
														trackIdTo, 
														lpnFrom, 
														lpnTo, 
														lotAttributeId, 
														transactionTypeId, 
														remarks, 
														createdBy)
 												values(@inventoryId, 
														@seqNum, 
														@documentRefId, 
														@qtyFrom, 
														@qtyTo, 
														@locationFrom, 
														@locationTo, 
														@trackIdFrom, 
														@trackIdTo, 
														@lpnFrom, 
														@lpnTo, 
														@lotAttributeId, 
														@transactionTypeId, 
														@remarks, 
														@createdBy)";

            int res = await db.ExecuteAsync(strQry, inventoryHistory);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildInvHistAuditADD(inventoryHistory, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInventoryHistory(InventoryHistoryModel inventoryHistory)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InventoryHistory set 
														seqNum = @seqNum, 
														documentRefId = @documentRefId, 
														qtyFrom = @qtyFrom, 
														qtyTo = @qtyTo, 
														locationFrom = @locationFrom, 
														locationTo = @locationTo, 
														trackIdFrom = @trackIdFrom, 
														trackIdTo = @trackIdTo, 
														lpnFrom = @lpnFrom, 
														lpnTo = @lpnTo, 
														lotAttributeId = @lotAttributeId, 
														transactionTypeId = @transactionTypeId, 
														remarks = @remarks where 
														inventoryId = @inventoryId";

                int res = await db.ExecuteAsync(strQry, inventoryHistory);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteInventoryHistory(string inventoryId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InventoryHistory where 
														inventoryId = @inventoryId";
                var param = new DynamicParameters();
                param.Add("@inventoryId", inventoryId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InventoryHistoryModel> GetInvHistoryByTrackId(IDbConnection db, string trackId)
        {
            // hold currentr eceive transaction
            string strQry = @"CALL `spGetTopInvHistDetailByTrackId`(@trackId)";

            var param = new DynamicParameters();
            param.Add("@trackId", trackId);

            return await db.QuerySingleOrDefaultAsync<InventoryHistoryModel>(strQry, param);
        }

        public async Task<bool> CheckTrackIdExists(IDbConnection db, string trackId)
        {
            string strQry = @"select count(inventoryId) from InventoryHistory where 
								        trackIdFrom = @trackId or 
                                        trackIdTo = @trackId";

            var param = new DynamicParameters();
            param.Add("@trackId", trackId);
            param.Add("@trackId", trackId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);
            if (res == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ChkLPNIsUsedInStorage(IDbConnection db, string lpnId)
        {
            // hold currentr eceive transaction
            string strQry = @"CALL `spSumLPNContentInStorage`(@lpnId)";

            var param = new DynamicParameters();
            param.Add("@lpnId", lpnId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);

            if (res == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ChkLPNIsUsedInStorage(string lpnId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // hold currentr eceive transaction
                string strQry = @"CALL `spSumLPNContentInStorage`(@lpnId)";

                var param = new DynamicParameters();
                param.Add("@lpnId", lpnId);

                var res = await db.ExecuteScalarAsync<int>(strQry, param);

                if (res == 0)
                {
                    return false;
                }

                return true;
            }
        }

    }
}
