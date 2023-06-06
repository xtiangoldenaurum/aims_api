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
    public class VehicleTypeRepository : IVehicleTypeRepository
    {
        private string ConnString;

        public VehicleTypeRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<VehicleTypeModel>> GetVehicleTypePg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from VehicleType limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<VehicleTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<VehicleTypeModel>> GetVehicleTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from VehicleType where 
														vehicleTypeId like @searchKey or 
														vehicleType like @searchKey or 
														description like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<VehicleTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<VehicleTypeModel> GetVehicleTypeById(string vehicleTypeId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from VehicleType where 
														vehicleTypeId = @vehicleTypeId";

                var param = new DynamicParameters();
				param.Add("@vehicleTypeId", vehicleTypeId);
                return await db.QuerySingleOrDefaultAsync<VehicleTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> VehicleTypeExists(string vehicleTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select vehicleTypeId from VehicleType where 
														vehicleTypeId = @vehicleTypeId";

                var param = new DynamicParameters();
				param.Add("@vehicleTypeId", vehicleTypeId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateVehicleType(VehicleTypeModel vehicleType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into VehicleType(vehicleTypeId, 
														vehicleType, 
														description, 
														createdBy, 
														modifiedBy)
 												values(@vehicleTypeId, 
														@vehicleType, 
														@description, 
														@createdBy, 
														@modifiedBy)";

                int res = await db.ExecuteAsync(strQry, vehicleType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateVehicleType(VehicleTypeModel vehicleType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update VehicleType set 
														vehicleType = @vehicleType, 
														description = @description, 
														modifiedBy = @modifiedBy where 
														vehicleTypeId = @vehicleTypeId";

                int res = await db.ExecuteAsync(strQry, vehicleType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteVehicleType(string vehicleTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from VehicleType where 
														vehicleTypeId = @vehicleTypeId";
                var param = new DynamicParameters();
				param.Add("@vehicleTypeId", vehicleTypeId);
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
