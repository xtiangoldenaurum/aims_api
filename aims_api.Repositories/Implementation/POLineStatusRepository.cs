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
    public class POLineStatusRepository : IPOLineStatusRepository
    {
        private string ConnString;

        public POLineStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<POLineStatusModel>> GetPOLineStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from POLineStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<POLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<POLineStatusModel>> GetPOLineStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from POLineStatus where 
														poLineStatusId like @searchKey or 
														poLineStatus like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<POLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<POLineStatusModel> GetPOLineStatusById(string poLineStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from POLineStatus where 
														poLineStatusId = @poLineStatusId";

                var param = new DynamicParameters();
				param.Add("@poLineStatusId", poLineStatusId);
                return await db.QuerySingleOrDefaultAsync<POLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> POLineStatusExists(string poLineStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select poLineStatusId from POLineStatus where 
														poLineStatusId = @poLineStatusId";

                var param = new DynamicParameters();
				param.Add("@poLineStatusId", poLineStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreatePOLineStatus(POLineStatusModel poLineStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into POLineStatus(poLineStatusId, 
														poLineStatus, 
														description)
 												values(@poLineStatusId, 
														@poLineStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, poLineStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdatePOLineStatus(POLineStatusModel poLineStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update POLineStatus set 
														poLineStatus = @poLineStatus, 
														description = @description where 
														poLineStatusId = @poLineStatusId";

                int res = await db.ExecuteAsync(strQry, poLineStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeletePOLineStatus(string poLineStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from POLineStatus where 
														poLineStatusId = @poLineStatusId";
                var param = new DynamicParameters();
				param.Add("@poLineStatusId", poLineStatusId);
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
