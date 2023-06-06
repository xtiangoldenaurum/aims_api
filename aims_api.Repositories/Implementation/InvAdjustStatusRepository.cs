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
    public class InvAdjustStatusRepository : IInvAdjustStatusRepository
    {
        private string ConnString;

        public InvAdjustStatusRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<InvAdjustStatusModel>> GetInvAdjustStatusPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvAdjustStatus limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvAdjustStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvAdjustStatusModel>> GetInvAdjustStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvAdjustStatus where 
														invAdjustStatusId like @searchKey or 
														invAdjustStatus like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvAdjustStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvAdjustStatusModel> GetInvAdjustStatusById(string invAdjustStatusId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvAdjustStatus where 
														invAdjustStatusId = @invAdjustStatusId";

                var param = new DynamicParameters();
				param.Add("@invAdjustStatusId", invAdjustStatusId);
                return await db.QuerySingleOrDefaultAsync<InvAdjustStatusModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> InvAdjustStatusExists(string invAdjustStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invAdjustStatusId from InvAdjustStatus where 
														invAdjustStatusId = @invAdjustStatusId";

                var param = new DynamicParameters();
				param.Add("@invAdjustStatusId", invAdjustStatusId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInvAdjustStatus(InvAdjustStatusModel invAdjustStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InvAdjustStatus(invAdjustStatusId, 
														invAdjustStatus, 
														description)
 												values(@invAdjustStatusId, 
														@invAdjustStatus, 
														@description)";

                int res = await db.ExecuteAsync(strQry, invAdjustStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvAdjustStatus(InvAdjustStatusModel invAdjustStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InvAdjustStatus set 
														invAdjustStatus = @invAdjustStatus, 
														description = @description where 
														invAdjustStatusId = @invAdjustStatusId";

                int res = await db.ExecuteAsync(strQry, invAdjustStatus);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteInvAdjustStatus(string invAdjustStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvAdjustStatus where 
														invAdjustStatusId = @invAdjustStatusId";
                var param = new DynamicParameters();
				param.Add("@invAdjustStatusId", invAdjustStatusId);
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
