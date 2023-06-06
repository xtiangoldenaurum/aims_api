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
    public class SOStatusRepository : ISOStatusRepository
    {
        private string ConnString;

        public SOStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<SOStatusModel>> GetSOStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from SOStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<SOStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<SOStatusModel>> GetSOStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SOStatus where 
														soStatusId like @searchKey or 
														soStatus like @searchKey or 
														description like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<SOStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<SOStatusModel> GetSOStatusById(string soStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SOStatus where 
														soStatusId = @soStatusId";

                var param = new DynamicParameters();
				param.Add("@soStatusId", soStatusId);
                return await db.QuerySingleOrDefaultAsync<SOStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> SOStatusExists(string soStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select soStatusId from SOStatus where 
														soStatusId = @soStatusId";

                var param = new DynamicParameters();
				param.Add("@soStatusId", soStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateSOStatus(SOStatusModel soStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into SOStatus(soStatusId, 
														soStatus, 
														description)
 												values(@soStatusId, 
														@soStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, soStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateSOStatus(SOStatusModel soStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update SOStatus set 
														soStatus = @soStatus, 
														description = @description where 
														soStatusId = @soStatusId";

                int res = await db.ExecuteAsync(strQry, soStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteSOStatus(string soStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from SOStatus where 
														soStatusId = @soStatusId";
                var param = new DynamicParameters();
				param.Add("@soStatusId", soStatusId);
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
