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
    public class RunningBalanceRepository : IRunningBalanceRepository
    {
        private string ConnString;

        public RunningBalanceRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<RunningBalanceModel>> GetRunningBalancePg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from RunningBalance limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<RunningBalanceModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<RunningBalanceModel>> GetRunningBalancePgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from RunningBalance where 
														movementTypeId like @searchKey or 
														documentRefId like @searchKey or 
														sku like @searchKey or 
														qtyCommited like @searchKey or 
														dateCreated like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<RunningBalanceModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<RunningBalanceModel> GetRunningBalanceById(string movementTypeId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from RunningBalance where 
														movementTypeId = @movementTypeId";

                var param = new DynamicParameters();
				param.Add("@movementTypeId", movementTypeId);
                return await db.QuerySingleOrDefaultAsync<RunningBalanceModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> RunningBalanceExists(string movementTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select movementTypeId from RunningBalance where 
														movementTypeId = @movementTypeId";

                var param = new DynamicParameters();
				param.Add("@movementTypeId", movementTypeId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateRunningBalance(RunningBalanceModel runningBalance)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into RunningBalance(movementTypeId, 
														documentRefId, 
														sku, 
														qtyCommited)
 												values(@movementTypeId, 
														@documentRefId, 
														@sku, 
														@qtyCommited)";

                int res = await db.ExecuteAsync(strQry, runningBalance);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateRunningBalance(RunningBalanceModel runningBalance)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update RunningBalance set 
														documentRefId = @documentRefId, 
														sku = @sku, 
														qtyCommited = @qtyCommited where 
														movementTypeId = @movementTypeId";

                int res = await db.ExecuteAsync(strQry, runningBalance);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteRunningBalance(string movementTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from RunningBalance where 
														movementTypeId = @movementTypeId";
                var param = new DynamicParameters();
				param.Add("@movementTypeId", movementTypeId);
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
