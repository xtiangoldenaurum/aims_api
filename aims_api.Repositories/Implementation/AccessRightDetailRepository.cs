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
    public class AccessRightDetailRepository : IAccessRightDetailRepository
    {
        private string ConnString;

        public AccessRightDetailRepository()
        {
            ConnString = String.Empty;
        }

        public AccessRightDetailRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<AccessRightDetailModel>> GetAllAccessRightDetail()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from accessrightdetail";
                return await db.QueryAsync<AccessRightDetailModel>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<AccessRightDetailModelMod>> GetAccessRightDetailById(string accessRightId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT mdl.modulename, 
			                                `ar`.* 
                                from accessrightdetail `ar` left JOIN 
		                                `module` `mdl` ON `ar`.moduleId = `mdl`.moduleId 
                                WHERE `ar`.accessRightId = @accessRightId";

                var param = new DynamicParameters();
                param.Add("@accessRightId", accessRightId);
                return await db.QueryAsync<AccessRightDetailModelMod>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> CheckAccessRightDetailsExists(IDbConnection db, string accessRightId)
        {
            string strQry = @"select count(distinct moduleId) cnt 
                                from accessrightdetail 
                                where accessRightId = @accessrightId";

            var param = new DynamicParameters();
            param.Add("@accessRightId", accessRightId);

            var res = await db.QueryFirstAsync<int>(strQry, param, commandType: CommandType.Text);
            if (res == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<AccessRightDetailModelMod>> GetUserAccessDetails(string accessRightId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetUserAccessDetails`(@usrAccessRightId)";

                var param = new DynamicParameters();
                param.Add("@usrAccessRightId", accessRightId);
                return await db.QueryAsync<AccessRightDetailModelMod>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<UserAllowedActionsModel>> GetUserAllowedActions(string accessRightId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetUserAllowedActions`(@usrAccessRightId)";

                var param = new DynamicParameters();
                param.Add("@usrAccessRightId", accessRightId);
                return await db.QueryAsync<UserAllowedActionsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<UserAllowedActionsModel>> GetUserAllowedActionsWeb(string accessRightId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetUserAllowedActionsWeb`(@usrAccessRightId)";

                var param = new DynamicParameters();
                param.Add("@usrAccessRightId", accessRightId);
                return await db.QueryAsync<UserAllowedActionsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<UserAllowedActionsModel>> GetUserAllowedActionsMob(string accessRightId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetUserAllowedActionsMob`(@usrAccessRightId)";

                var param = new DynamicParameters();
                param.Add("@usrAccessRightId", accessRightId);
                return await db.QueryAsync<UserAllowedActionsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> AccessRightDetailExists(AccessRightDetailModel detail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select accessRightId from 
                                            accessrightdetail where 
                                                accessRightId = @accessRightId and 
                                                moduleId = @moduleId and 
                                                actionTypeId = @actionTypeId";

                var param = new DynamicParameters();
                param.Add("@accessRightId", detail.AccessRightId);
                param.Add("@moduleId", detail.ModuleId);
                param.Add("@actionTypeId", detail.ActionTypeId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateAccessRightDetail(IDbConnection db, string accessRightId, IEnumerable<AccessRightDetailModel> details)
        {
            foreach (var itm in details)
            {
                // set accessrightId
                itm.AccessRightId = accessRightId;

                string strQry = @"insert into accessrightdetail(accessRightID, 
                                                                moduleId, 
                                                                actionTypeId, 
                                                                createdBy, 
                                                                modifiedBy) 
                                                            values(@accessRightID, 
                                                                    @moduleId, 
                                                                    @actionTypeId, 
                                                                    @createdBy, 
                                                                    @modifiedBy)";
                int detailRecorded = await db.ExecuteAsync(strQry, itm);

                if (detailRecorded == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> CreateAccessRightDetail(IDbConnection db, string accessRightId, string createdBy, string modifiedBy, IEnumerable<AccessRightDetailModelMod> details)
        {
            // define actual access details object
            var val = (from mainVal in details.Where(x => x.Allow == true)
                       select new AccessRightDetailModel
                       {
                           AccessRightId = accessRightId,
                           ModuleId = mainVal.HeaderModuleId,
                           ActionTypeId = mainVal.HeaderActionTypeId,
                           CreatedBy = createdBy,
                           ModifiedBy = modifiedBy
                       }).AsEnumerable();


            foreach (var itm in val)
            {
                string strQry = @"insert into accessrightdetail(accessRightID, 
                                                                moduleId, 
                                                                actionTypeId, 
                                                                createdBy, 
                                                                modifiedBy) 
                                                            values(@accessRightID, 
                                                                    @moduleId, 
                                                                    @actionTypeId, 
                                                                    @createdBy, 
                                                                    @modifiedBy)";
                int detailRecorded = await db.ExecuteAsync(strQry, itm);

                if (detailRecorded == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeletePrevAccessRightDetails(IDbConnection db, string accessRightId)
        {
            string strQry = @"delete from accessrightdetail where accessRightId = @accessRightId";
            var param = new DynamicParameters();
            param.Add("@accessRightId", accessRightId);
            int recordAffected = await db.ExecuteAsync(strQry, param);

            if (recordAffected > 0)
            {
                return true;
            }

            return false;
        }
    }
}
