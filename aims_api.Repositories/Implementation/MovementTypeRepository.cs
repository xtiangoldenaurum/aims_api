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
    public class MovementTypeRepository : IMovementTypeRepository
    {
        private string ConnString;

        public MovementTypeRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<MovementTypeModel>> GetMovementTypePg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from MovementType limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<MovementTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<MovementTypeModel>> GetMovementTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from MovementType where 
														movementTypeId like @searchKey or 
														movementTypeName like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<MovementTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<MovementTypeModel> GetMovementTypeById(string movementTypeId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from MovementType where 
														movementTypeId = @movementTypeId";

                var param = new DynamicParameters();
				param.Add("@movementTypeId", movementTypeId);
                return await db.QuerySingleOrDefaultAsync<MovementTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> MovementTypeExists(string movementTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select movementTypeId from MovementType where 
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

        public async Task<bool> CreateMovementType(MovementTypeModel movementType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into MovementType(movementTypeId, 
														movementTypeName, 
														description)
 												values(@movementTypeId, 
														@movementTypeName, 
														@description)";

                int res = await db.ExecuteAsync(strQry, movementType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateMovementType(MovementTypeModel movementType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update MovementType set 
														movementTypeName = @movementTypeName, 
														description = @description where 
														movementTypeId = @movementTypeId";

                int res = await db.ExecuteAsync(strQry, movementType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteMovementType(string movementTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from MovementType where 
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
