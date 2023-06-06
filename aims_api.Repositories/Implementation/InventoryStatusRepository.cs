using aims_api.Models;
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
    public class InventoryStatusRepository : IInventoryStatusRepository
    {
        private string ConnString;

        public InventoryStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<InventoryStatusModel>> GetInventoryStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InventoryStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InventoryStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InventoryStatusModel>> GetInventoryStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InventoryStatus where 
														inventoryStatusId like @searchKey or 
														statusName like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InventoryStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InventoryStatusModel> GetInventoryStatusById(string inventoryStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InventoryStatus where 
														inventoryStatusId = @inventoryStatusId";

                var param = new DynamicParameters();
				param.Add("@inventoryStatusId", inventoryStatusId);
                return await db.QuerySingleOrDefaultAsync<InventoryStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> InventoryStatusExists(string inventoryStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select inventoryStatusId from InventoryStatus where 
														inventoryStatusId = @inventoryStatusId";

                var param = new DynamicParameters();
				param.Add("@inventoryStatusId", inventoryStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInventoryStatus(InventoryStatusModel inventoryStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InventoryStatus(inventoryStatusId, 
														statusName, 
														description)
 												values(@inventoryStatusId, 
														@statusName, 
														@description)";

                int res = await db.ExecuteAsync(strQry, inventoryStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInventoryStatus(InventoryStatusModel inventoryStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InventoryStatus set 
														statusName = @statusName, 
														description = @description where 
														inventoryStatusId = @inventoryStatusId";

                int res = await db.ExecuteAsync(strQry, inventoryStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteInventoryStatus(string inventoryStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InventoryStatus where 
														inventoryStatusId = @inventoryStatusId";
                var param = new DynamicParameters();
				param.Add("@inventoryStatusId", inventoryStatusId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
