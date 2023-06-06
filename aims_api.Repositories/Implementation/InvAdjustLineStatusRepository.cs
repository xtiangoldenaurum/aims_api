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
    public class InvAdjustLineStatusRepository : IInvAdjustLineStatusRepository
    {
        private string ConnString;

        public InvAdjustLineStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<InvAdjustLineStatusModel>> GetInvAdjustLineStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvAdjustLineStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvAdjustLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvAdjustLineStatusModel>> GetInvAdjustLineStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvAdjustLineStatus where 
														invAdjustLineStatusId like @searchKey or 
														invAdjustStatus like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvAdjustLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvAdjustLineStatusModel> GetInvAdjustLineStatusById(string invAdjustLineStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvAdjustLineStatus where 
														invAdjustLineStatusId = @invAdjustLineStatusId";

                var param = new DynamicParameters();
				param.Add("@invAdjustLineStatusId", invAdjustLineStatusId);
                return await db.QuerySingleOrDefaultAsync<InvAdjustLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> InvAdjustLineStatusExists(string invAdjustLineStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invAdjustLineStatusId from InvAdjustLineStatus where 
														invAdjustLineStatusId = @invAdjustLineStatusId";

                var param = new DynamicParameters();
				param.Add("@invAdjustLineStatusId", invAdjustLineStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInvAdjustLineStatus(InvAdjustLineStatusModel invAdjustLineStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InvAdjustLineStatus(invAdjustLineStatusId, 
														invAdjustStatus, 
														description)
 												values(@invAdjustLineStatusId, 
														@invAdjustStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, invAdjustLineStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvAdjustLineStatus(InvAdjustLineStatusModel invAdjustLineStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InvAdjustLineStatus set 
														invAdjustStatus = @invAdjustStatus, 
														description = @description where 
														invAdjustLineStatusId = @invAdjustLineStatusId";

                int res = await db.ExecuteAsync(strQry, invAdjustLineStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteInvAdjustLineStatus(string invAdjustLineStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvAdjustLineStatus where 
														invAdjustLineStatusId = @invAdjustLineStatusId";
                var param = new DynamicParameters();
				param.Add("@invAdjustLineStatusId", invAdjustLineStatusId);
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
