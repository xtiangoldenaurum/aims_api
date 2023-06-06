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
    public class SOLineStatusRepository : ISOLineStatusRepository
    {
        private string ConnString;

        public SOLineStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<SOLineStatusModel>> GetSOLineStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from SOLineStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<SOLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<SOLineStatusModel>> GetSOLineStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SOLineStatus where 
														soLineStatusId like @searchKey or 
														soLineStatus like @searchKey or 
														description like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<SOLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<SOLineStatusModel> GetSOLineStatusById(string soLineStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SOLineStatus where 
														soLineStatusId = @soLineStatusId";

                var param = new DynamicParameters();
				param.Add("@soLineStatusId", soLineStatusId);
                return await db.QuerySingleOrDefaultAsync<SOLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> SOLineStatusExists(string soLineStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select soLineStatusId from SOLineStatus where 
														soLineStatusId = @soLineStatusId";

                var param = new DynamicParameters();
				param.Add("@soLineStatusId", soLineStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateSOLineStatus(SOLineStatusModel soLineStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into SOLineStatus(soLineStatusId, 
														soLineStatus, 
														description)
 												values(@soLineStatusId, 
														@soLineStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, soLineStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateSOLineStatus(SOLineStatusModel soLineStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update SOLineStatus set 
														soLineStatus = @soLineStatus, 
														description = @description where 
														soLineStatusId = @soLineStatusId";

                int res = await db.ExecuteAsync(strQry, soLineStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteSOLineStatus(string soLineStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from SOLineStatus where 
														soLineStatusId = @soLineStatusId";
                var param = new DynamicParameters();
				param.Add("@soLineStatusId", soLineStatusId);
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
