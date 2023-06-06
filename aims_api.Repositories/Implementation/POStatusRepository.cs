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
    public class POStatusRepository : IPOStatusRepository
    {
        private string ConnString;

        public POStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<POStatusModel>> GetPOStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from POStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<POStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<POStatusModel>> GetPOStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from POStatus where 
														poStatusId like @searchKey or 
														poStatus like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<POStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<POStatusModel> GetPOStatusById(string poStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from POStatus where 
														poStatusId = @poStatusId";

                var param = new DynamicParameters();
				param.Add("@poStatusId", poStatusId);
                return await db.QuerySingleOrDefaultAsync<POStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> POStatusExists(string poStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select poStatusId from POStatus where 
														poStatusId = @poStatusId";

                var param = new DynamicParameters();
				param.Add("@poStatusId", poStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreatePOStatus(POStatusModel poStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into POStatus(poStatusId, 
														poStatus, 
														description)
 												values(@poStatusId, 
														@poStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, poStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdatePOStatus(POStatusModel poStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update POStatus set 
														poStatus = @poStatus, 
														description = @description where 
														poStatusId = @poStatusId";

                int res = await db.ExecuteAsync(strQry, poStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeletePOStatus(string poStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from POStatus where 
														poStatusId = @poStatusId";
                var param = new DynamicParameters();
				param.Add("@poStatusId", poStatusId);
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
