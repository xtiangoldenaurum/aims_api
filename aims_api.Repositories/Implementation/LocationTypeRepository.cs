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
    public class LocationTypeRepository : ILocationTypeRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        LocationTypeAudit AuditBuilder;

        public LocationTypeRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new LocationTypeAudit();
        }

        public async Task<IEnumerable<LocationTypeModel>> GetLocationTypePg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from LocationType limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<LocationTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<LocationTypeModel>> GetLocationTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from LocationType where 
														locationTypeId like @searchKey or 
														locationTypeName like @searchKey or 
														description like @searchKey or 
                                                        allowPallet like @searchKey or 
                                                        singlePallet like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<LocationTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<LocationTypeModel> GetLocationTypeById(string locationTypeId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from LocationType where 
														locationTypeId = @locationTypeId";

                var param = new DynamicParameters();
				param.Add("@locationTypeId", locationTypeId);
                return await db.QuerySingleOrDefaultAsync<LocationTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> LocationTypeExists(string locationTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select locationTypeId from LocationType where 
														locationTypeId = @locationTypeId";

                var param = new DynamicParameters();
				param.Add("@locationTypeId", locationTypeId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateLocationType(LocationTypeModel locationType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"insert into LocationType(locationTypeId, 
														locationTypeName, 
														description, 
                                                        allowPallet, 
                                                        singlePallet, 
                                                        createdBy, 
                                                        modifiedBy)
 												values(@locationTypeId, 
														@locationTypeName, 
														@description, 
                                                        @allowPallet, 
                                                        @singlePallet, 
                                                        @createdBy, 
                                                        @modifiedBy)";

                int res = await db.ExecuteAsync(strQry, locationType);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(locationType);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateLocationType(LocationTypeModel locationType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"update LocationType set 
														locationTypeName = @locationTypeName, 
														description = @description, 
                                                        allowPallet = @allowPallet, 
                                                        singlePallet = @singlePallet, 
                                                        createdBy = @createdBy, 
                                                        modifiedBy = @modifiedBy where 
														locationTypeId = @locationTypeId";

                int res = await db.ExecuteAsync(strQry, locationType);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(locationType);

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

        public async Task<bool> DeleteLocationType(string locationTypeId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                var locationType = await GetLocationTypeById(locationTypeId);

                if (locationType != null)
                {
                    string strQry = @"delete from LocationType where 
														locationTypeId = @locationTypeId";
                    var param = new DynamicParameters();
                    param.Add("@locationTypeId", locationTypeId);
                    int res = await db.ExecuteAsync(strQry, param);

                    if (res > 0)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(locationType, userAccountId);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                        {
                            tran.Commit();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }
}
