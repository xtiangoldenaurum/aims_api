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
    public class InvCountLineStatusRepository : IInvCountLineStatusRepository
    {
        private string ConnString;

        public InvCountLineStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<InvCountLineStatusModel>> GetInvCountLineStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvCountLineStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvCountLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvCountLineStatusModel>> GetInvCountLineStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvCountLineStatus where 
														invCountLineStatusId like @searchKey or 
														invCountLineStatus like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvCountLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvCountLineStatusModel> GetInvCountLineStatusById(string invCountLineStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvCountLineStatus where 
														invCountLineStatusId = @invCountLineStatusId";

                var param = new DynamicParameters();
				param.Add("@invCountLineStatusId", invCountLineStatusId);
                return await db.QuerySingleOrDefaultAsync<InvCountLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> InvCountLineStatusExists(string invCountLineStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invCountLineStatusId from InvCountLineStatus where 
														invCountLineStatusId = @invCountLineStatusId";

                var param = new DynamicParameters();
				param.Add("@invCountLineStatusId", invCountLineStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInvCountLineStatus(InvCountLineStatusModel invCountLineStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InvCountLineStatus(invCountLineStatusId, 
														invCountLineStatus, 
														description)
 												values(@invCountLineStatusId, 
														@invCountLineStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, invCountLineStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvCountLineStatus(InvCountLineStatusModel invCountLineStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InvCountLineStatus set 
														invCountLineStatus = @invCountLineStatus, 
														description = @description where 
														invCountLineStatusId = @invCountLineStatusId";

                int res = await db.ExecuteAsync(strQry, invCountLineStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteInvCountLineStatus(string invCountLineStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvCountLineStatus where 
														invCountLineStatusId = @invCountLineStatusId";
                var param = new DynamicParameters();
				param.Add("@invCountLineStatusId", invCountLineStatusId);
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
