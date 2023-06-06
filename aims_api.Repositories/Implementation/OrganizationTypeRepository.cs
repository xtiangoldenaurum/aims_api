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
    public class OrganizationTypeRepository : IOrganizationTypeRepository
    {
        private string ConnString;

        public OrganizationTypeRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<OrganizationTypeModel>> GetOrganizationTypePg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from OrganizationType limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<OrganizationTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<OrganizationTypeModel>> GetOrganizationTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from OrganizationType where 
														organizationTypeID like @searchKey or 
														organizationType like @searchKey or 
														description like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<OrganizationTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<OrganizationTypeModel> GetOrganizationTypeById(string organizationTypeID)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from OrganizationType where 
														organizationTypeID = @organizationTypeID";

                var param = new DynamicParameters();
				param.Add("@organizationTypeID", organizationTypeID);
                return await db.QuerySingleOrDefaultAsync<OrganizationTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> OrganizationTypeExists(string organizationTypeID)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select organizationTypeID from OrganizationType where 
														organizationTypeID = @organizationTypeID";

                var param = new DynamicParameters();
				param.Add("@organizationTypeID", organizationTypeID);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateOrganizationType(OrganizationTypeModel organizationType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into OrganizationType(organizationTypeID, 
														organizationType, 
														description)
 												values(@organizationTypeID, 
														@organizationType, 
														@description)";

                int res = await db.ExecuteAsync(strQry, organizationType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateOrganizationType(OrganizationTypeModel organizationType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update OrganizationType set 
														organizationType = @organizationType, 
														description = @description where 
														organizationTypeID = @organizationTypeID";

                int res = await db.ExecuteAsync(strQry, organizationType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteOrganizationType(string organizationTypeID)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from OrganizationType where 
														organizationTypeID = @organizationTypeID";
                var param = new DynamicParameters();
				param.Add("@organizationTypeID", organizationTypeID);
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
