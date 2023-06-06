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
    public class ActionTypeRepository : IActionTypeRepository
    {
        private string ConnString;

        public ActionTypeRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<ActionTypeModel>> GetActionTypePg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from ActionType limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ActionTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<ActionTypeModel>> GetActionTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ActionType where 
														actionTypeId like @searchKey or 
														actionName like @searchKey or 
														description like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ActionTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ActionTypeModel> GetActionTypeById(string actionTypeId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ActionType where 
														actionTypeId = @actionTypeId";

                var param = new DynamicParameters();
				param.Add("@actionTypeId", actionTypeId);
                return await db.QuerySingleOrDefaultAsync<ActionTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> ActionTypeExists(string actionTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select actionTypeId from ActionType where 
														actionTypeId = @actionTypeId";

                var param = new DynamicParameters();
				param.Add("@actionTypeId", actionTypeId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateActionType(ActionTypeModel actionType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into ActionType(actionTypeId, 
														actionName, 
														description)
 												values(@actionTypeId, 
														@actionName, 
														@description)";

                int res = await db.ExecuteAsync(strQry, actionType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateActionType(ActionTypeModel actionType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update ActionType set 
														actionName = @actionName, 
														description = @description where 
														actionTypeId = @actionTypeId";

                int res = await db.ExecuteAsync(strQry, actionType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function
        public async Task<bool> DeleteActionType(string actionTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from ActionType where 
														actionTypeId = @actionTypeId";
                var param = new DynamicParameters();
				param.Add("@actionTypeId", actionTypeId);
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
