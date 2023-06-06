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
    public class InvMoveLineStatusRepository : IInvMoveLineStatusRepository
    {
        private string ConnString;

        public InvMoveLineStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<InvMoveLineStatusModel>> GetInvMoveLineStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvMoveLineStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvMoveLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvMoveLineStatusModel>> GetInvMoveLineStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvMoveLineStatus where 
														invMoveLineStatusId like @searchKey or 
														invMoveLineStatus like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvMoveLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvMoveLineStatusModel> GetInvMoveLineStatusById(string invMoveLineStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvMoveLineStatus where 
														invMoveLineStatusId = @invMoveLineStatusId";

                var param = new DynamicParameters();
				param.Add("@invMoveLineStatusId", invMoveLineStatusId);
                return await db.QuerySingleOrDefaultAsync<InvMoveLineStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> InvMoveLineStatusExists(string invMoveLineStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invMoveLineStatusId from InvMoveLineStatus where 
														invMoveLineStatusId = @invMoveLineStatusId";

                var param = new DynamicParameters();
				param.Add("@invMoveLineStatusId", invMoveLineStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInvMoveLineStatus(InvMoveLineStatusModel invMoveLineStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InvMoveLineStatus(invMoveLineStatusId, 
														invMoveLineStatus, 
														description)
 												values(@invMoveLineStatusId, 
														@invMoveLineStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, invMoveLineStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvMoveLineStatus(InvMoveLineStatusModel invMoveLineStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InvMoveLineStatus set 
														invMoveLineStatus = @invMoveLineStatus, 
														description = @description where 
														invMoveLineStatusId = @invMoveLineStatusId";

                int res = await db.ExecuteAsync(strQry, invMoveLineStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteInvMoveLineStatus(string invMoveLineStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvMoveLineStatus where 
														invMoveLineStatusId = @invMoveLineStatusId";
                var param = new DynamicParameters();
				param.Add("@invMoveLineStatusId", invMoveLineStatusId);
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
