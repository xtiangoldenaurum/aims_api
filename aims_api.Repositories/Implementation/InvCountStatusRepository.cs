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
    public class InvCountStatusRepository : IInvCountStatusRepository
    {
        private string ConnString;

        public InvCountStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<InvCountStatusModel>> GetInvCountStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvCountStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvCountStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvCountStatusModel>> GetInvCountStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvCountStatus where 
														invCountStatusId like @searchKey or 
														invCountStatus like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvCountStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvCountStatusModel> GetInvCountStatusById(string invCountStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvCountStatus where 
														invCountStatusId = @invCountStatusId";

                var param = new DynamicParameters();
				param.Add("@invCountStatusId", invCountStatusId);
                return await db.QuerySingleOrDefaultAsync<InvCountStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> InvCountStatusExists(string invCountStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invCountStatusId from InvCountStatus where 
														invCountStatusId = @invCountStatusId";

                var param = new DynamicParameters();
				param.Add("@invCountStatusId", invCountStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInvCountStatus(InvCountStatusModel invCountStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InvCountStatus(invCountStatusId, 
														invCountStatus, 
														description)
 												values(@invCountStatusId, 
														@invCountStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, invCountStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvCountStatus(InvCountStatusModel invCountStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InvCountStatus set 
														invCountStatus = @invCountStatus, 
														description = @description where 
														invCountStatusId = @invCountStatusId";

                int res = await db.ExecuteAsync(strQry, invCountStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteInvCountStatus(string invCountStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvCountStatus where 
														invCountStatusId = @invCountStatusId";
                var param = new DynamicParameters();
				param.Add("@invCountStatusId", invCountStatusId);
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
