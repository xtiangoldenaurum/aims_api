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
    public class InvMoveStatusRepository : IInvMoveStatusRepository
    {
        private string ConnString;

        public InvMoveStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<InvMoveStatusModel>> GetInvMoveStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvMoveStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvMoveStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvMoveStatusModel>> GetInvMoveStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvMoveStatus where 
														invMoveStatusId like @searchKey or 
														invMoveStatus like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvMoveStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvMoveStatusModel> GetInvMoveStatusById(string invMoveStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvMoveStatus where 
														invMoveStatusId = @invMoveStatusId";

                var param = new DynamicParameters();
				param.Add("@invMoveStatusId", invMoveStatusId);
                return await db.QuerySingleOrDefaultAsync<InvMoveStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> InvMoveStatusExists(string invMoveStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invMoveStatusId from InvMoveStatus where 
														invMoveStatusId = @invMoveStatusId";

                var param = new DynamicParameters();
				param.Add("@invMoveStatusId", invMoveStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInvMoveStatus(InvMoveStatusModel invMoveStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InvMoveStatus(invMoveStatusId, 
														invMoveStatus, 
														description)
 												values(@invMoveStatusId, 
														@invMoveStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, invMoveStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvMoveStatus(InvMoveStatusModel invMoveStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InvMoveStatus set 
														invMoveStatus = @invMoveStatus, 
														description = @description where 
														invMoveStatusId = @invMoveStatusId";

                int res = await db.ExecuteAsync(strQry, invMoveStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteInvMoveStatus(string invMoveStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvMoveStatus where 
														invMoveStatusId = @invMoveStatusId";
                var param = new DynamicParameters();
				param.Add("@invMoveStatusId", invMoveStatusId);
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
