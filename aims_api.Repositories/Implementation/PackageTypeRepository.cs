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
    public class PackageTypeRepository : IPackageTypeRepository
    {
        private string ConnString;

        public PackageTypeRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<PackageTypeModel>> GetPackageTypePg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from PackageType limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<PackageTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<PackageTypeModel>> GetPackageTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from PackageType where 
														packageTypeId like @searchKey or 
														packageType like @searchKey or 
														description like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<PackageTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<PackageTypeModel> GetPackageTypeById(string packageTypeId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from PackageType where 
														packageTypeId = @packageTypeId";

                var param = new DynamicParameters();
				param.Add("@packageTypeId", packageTypeId);
                return await db.QuerySingleOrDefaultAsync<PackageTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> PackageTypeExists(string packageTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select packageTypeId from PackageType where 
														packageTypeId = @packageTypeId";

                var param = new DynamicParameters();
				param.Add("@packageTypeId", packageTypeId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreatePackageType(PackageTypeModel packageType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into PackageType(packageTypeId, 
														packageType, 
														description, 
														createdBy, 
														modifiedBy)
 												values(@packageTypeId, 
														@packageType, 
														@description, 
														@createdBy, 
														@modifiedBy)";

                int res = await db.ExecuteAsync(strQry, packageType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdatePackageType(PackageTypeModel packageType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update PackageType set 
														packageType = @packageType, 
														description = @description, 
														modifiedBy = @modifiedBy where 
														packageTypeId = @packageTypeId";

                int res = await db.ExecuteAsync(strQry, packageType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeletePackageType(string packageTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from PackageType where 
														packageTypeId = @packageTypeId";
                var param = new DynamicParameters();
				param.Add("@packageTypeId", packageTypeId);
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
