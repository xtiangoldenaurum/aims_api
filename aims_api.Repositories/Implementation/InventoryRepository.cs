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
    public class InventoryRepository : IInventoryRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        InventoryAudit AuditBuilder;

        public InventoryRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new InventoryAudit();
        }

        public async Task<IEnumerable<InventoryModel>> GetInventoryPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from Inventory limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InventoryModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InventoryModel>> GetInventoryPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Inventory where 
														inventoryId like @searchKey or 
														sku like @searchKey or 
														inventoryStatusId like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InventoryModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InventoryModel> GetInventoryById(string inventoryId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Inventory where 
														inventoryId = @inventoryId";

                var param = new DynamicParameters();
                param.Add("@inventoryId", inventoryId);
                return await db.QuerySingleOrDefaultAsync<InventoryModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InventoryModel> GetInventoryByIdMod(IDbConnection db, string inventoryId)
        {
            string strQry = @"select * from Inventory where 
													inventoryId = @inventoryId";

            var param = new DynamicParameters();
            param.Add("@inventoryId", inventoryId);

            return await db.QuerySingleOrDefaultAsync<InventoryModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> InventoryExists(string inventoryId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select inventoryId from Inventory where 
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

        public async Task<bool> CreateInventory(InventoryModel inventory)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into Inventory(inventoryId, 
														sku, 
														inventoryStatusId)
 												values(@inventoryId, 
														@sku, 
														@inventoryStatusId)";

                int res = await db.ExecuteAsync(strQry, inventory);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInventoryMod(IDbConnection db, InventoryModel inventory, string userAccountId, TranType tranTyp)
        {
            string strQry = @"insert into Inventory(inventoryId, 
														sku, 
														inventoryStatusId)
 												values(@inventoryId, 
														@sku, 
														@inventoryStatusId)";

            int res = await db.ExecuteAsync(strQry, inventory);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildInvAuditADD(inventory, userAccountId, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInventory(InventoryModel inventory)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update Inventory set 
														sku = @sku, 
														inventoryStatusId = @inventoryStatusId where 
														inventoryId = @inventoryId";

                int res = await db.ExecuteAsync(strQry, inventory);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> SetInventoryStatus(string inventoryId, string invStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update inventory set inventoryStatusId = @invStatus 
                                    where inventoryId = @inventoryId;";

                var param = new DynamicParameters();
                param.Add("@invStatus", invStatus);
                param.Add("@inventoryId", inventoryId);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> SetInventoryStatus(IDbConnection db, string inventoryId, string invStatus, string userAccountId, TranType tranTyp)
        {
            string strQry = @"update inventory set inventoryStatusId = @invStatus 
                                    where inventoryId = @inventoryId;";

            var param = new DynamicParameters();
            param.Add("@invStatus", invStatus);
            param.Add("@inventoryId", inventoryId);

            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                var inv = await GetInventoryByIdMod(db, inventoryId);

                if (inv != null)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(inv, userAccountId, tranTyp);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteInventory(string inventoryId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from Inventory where 
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

        public async Task<InventoryModel> LockInventoryByInvId(IDbConnection db, string inventoryId)
        {
            string strQry = @"select * 
                                from Inventory where 
                                        inventoryId = @inventoryId and 
                                        inventoryStatusId = 'REFERRED' 
                                for update;";

            var param = new DynamicParameters();
            param.Add("@inventoryId", inventoryId);

            return await db.QuerySingleOrDefaultAsync<InventoryModel>(strQry, param, commandType: CommandType.Text);
        }

        //public async Task<>

    }
}
