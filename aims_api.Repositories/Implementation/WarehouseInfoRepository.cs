using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
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
    public class WarehouseInfoRepository : IWarehouseInfoRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        WarehouseInfoAudit AuditBuilder;

        public WarehouseInfoRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new WarehouseInfoAudit();
        }

        public async Task<IEnumerable<WarehouseInfoModel>> GetWarehouseInfoPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from WarehouseInfo limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<WarehouseInfoModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<WarehouseInfoModel>> GetWarehouseInfoPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from WarehouseInfo where 
														warehouseId like @searchKey or 
														warehouseName like @searchKey or 
														description like @searchKey or 
														address like @searchKey or 
														address2 like @searchKey or 
														province like @searchKey or 
														city like @searchKey or 
														zipCode like @searchKey or 
														telephone like @searchKey or 
														phone like @searchKey or 
														email like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<WarehouseInfoModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<WarehouseInfoModel> GetWarehouseInfoById(string warehouseId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from WarehouseInfo where 
														warehouseId = @warehouseId";

                var param = new DynamicParameters();
				param.Add("@warehouseId", warehouseId);
                return await db.QuerySingleOrDefaultAsync<WarehouseInfoModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> WarehouseInfoExists(string warehouseId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select warehouseId from WarehouseInfo where 
														warehouseId = @warehouseId";

                var param = new DynamicParameters();
				param.Add("@warehouseId", warehouseId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateWarehouseInfo(WarehouseInfoModel warehouseInfo)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into WarehouseInfo(warehouseId, 
														warehouseName, 
														description, 
														address, 
														address2, 
														province, 
														city, 
														zipCode, 
														telephone, 
														phone, 
														email, 
														createdBy, 
														modifiedBy)
 												values(@warehouseId, 
														@warehouseName, 
														@description, 
														@address, 
														@address2, 
														@province, 
														@city, 
														@zipCode, 
														@telephone, 
														@phone, 
														@email, 
														@createdBy, 
														@modifiedBy)";

                int res = await db.ExecuteAsync(strQry, warehouseInfo);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateWarehouseInfo(WarehouseInfoModel warehouseInfo)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"update WarehouseInfo set 
														warehouseName = @warehouseName, 
														description = @description, 
														address = @address, 
														address2 = @address2, 
														province = @province, 
														city = @city, 
														zipCode = @zipCode, 
														telephone = @telephone, 
														phone = @phone, 
														email = @email, 
														modifiedBy = @modifiedBy where 
														warehouseId = @warehouseId";

                int res = await db.ExecuteAsync(strQry, warehouseInfo);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(warehouseInfo);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteWarehouseInfo(string warehouseId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from WarehouseInfo where 
														warehouseId = @warehouseId";
                var param = new DynamicParameters();
				param.Add("@warehouseId", warehouseId);
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
