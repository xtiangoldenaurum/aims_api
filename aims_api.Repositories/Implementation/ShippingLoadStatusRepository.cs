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
    public class ShippingLoadStatusRepository : IShippingLoadStatusRepository
    {
        private string ConnString;

        public ShippingLoadStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<ShippingLoadStatusModel>> GetShippingLoadStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from ShippingLoadStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ShippingLoadStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<ShippingLoadStatusModel>> GetShippingLoadStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ShippingLoadStatus where 
														shippingLoadStatusId like @searchKey or 
														shippingLoadStatus like @searchKey or 
														description like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ShippingLoadStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ShippingLoadStatusModel> GetShippingLoadStatusById(string shippingLoadStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ShippingLoadStatus where 
														shippingLoadStatusId = @shippingLoadStatusId";

                var param = new DynamicParameters();
				param.Add("@shippingLoadStatusId", shippingLoadStatusId);
                return await db.QuerySingleOrDefaultAsync<ShippingLoadStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> ShippingLoadStatusExists(string shippingLoadStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select shippingLoadStatusId from ShippingLoadStatus where 
														shippingLoadStatusId = @shippingLoadStatusId";

                var param = new DynamicParameters();
				param.Add("@shippingLoadStatusId", shippingLoadStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateShippingLoadStatus(ShippingLoadStatusModel shippingLoadStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into ShippingLoadStatus(shippingLoadStatusId, 
														shippingLoadStatus, 
														description)
 												values(@shippingLoadStatusId, 
														@shippingLoadStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, shippingLoadStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateShippingLoadStatus(ShippingLoadStatusModel shippingLoadStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update ShippingLoadStatus set 
														shippingLoadStatus = @shippingLoadStatus, 
														description = @description where 
														shippingLoadStatusId = @shippingLoadStatusId";

                int res = await db.ExecuteAsync(strQry, shippingLoadStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteShippingLoadStatus(string shippingLoadStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from ShippingLoadStatus where 
														shippingLoadStatusId = @shippingLoadStatusId";
                var param = new DynamicParameters();
				param.Add("@shippingLoadStatusId", shippingLoadStatusId);
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
