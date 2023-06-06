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
    public class ModuleRepository : IModuleRepository
    {
        private string ConnString;

        public ModuleRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<ModuleModel>> GetModulePg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from Module limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ModuleModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<ModuleModel>> GetModulePgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Module where 
														moduleId like @searchKey or 
														moduleName like @searchKey or 
														seqNum like @searchKey or 
														description like @searchKey or 
														icon like @searchKey or 
														image like @searchKey or 
														url like @searchKey or 
														envTypeId like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ModuleModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ModuleModel> GetModuleById(string moduleId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Module where 
														moduleId = @moduleId";

                var param = new DynamicParameters();
				param.Add("@moduleId", moduleId);
                return await db.QuerySingleOrDefaultAsync<ModuleModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> ModuleExists(string moduleId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select moduleId from Module where 
														moduleId = @moduleId";

                var param = new DynamicParameters();
				param.Add("@moduleId", moduleId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateModule(ModuleModel module)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into Module(moduleId, 
														moduleName, 
														seqNum, 
														description, 
														icon, 
														image, 
														url, 
                                                        envTypeId)
 												values(@moduleId, 
														@moduleName, 
														@seqNum, 
														@description, 
														@icon, 
														@image, 
														@url, 
                                                        @envTypeId)";

                int res = await db.ExecuteAsync(strQry, module);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateModule(ModuleModel module)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update Module set 
														moduleName = @moduleName, 
														seqNum = @seqNum, 
														description = @description, 
														icon = @icon, 
														image = @image, 
														url = @url, 
                                                        envTypeId = @envTypeId where 
														moduleId = @moduleId";

                int res = await db.ExecuteAsync(strQry, module);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteModule(string moduleId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from Module where 
														moduleId = @moduleId";
                var param = new DynamicParameters();
				param.Add("@moduleId", moduleId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<IEnumerable<ModuleModel>> GetUserModules(string accessRightId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"with tblA AS (SELECT mdl.* FROM module mdl INNER JOIN 
							                            accessrightdetail accd ON mdl.moduleId = accd.moduleId 
                                    WHERE accd.actionTypeId = 'view' AND 
		                                    accd.accessRightId = @accessRightId 
                                    ORDER BY mdl.seqNum)
                                    SELECT * FROM tblA 
                                    UNION 
                                    SELECT * FROM module 
                                    WHERE modulename IN (SELECT distinct parentname FROM tblA) 
                                    ORDER BY seqNum;";

                var param = new DynamicParameters();
                param.Add("@accessRightId", accessRightId);
                return await db.QueryAsync<ModuleModel>(strQry, param, commandType: CommandType.Text);
            }
        }

    }
}
