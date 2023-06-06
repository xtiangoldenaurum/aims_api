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
    public class LocationGroupRepository : ILocationGroupRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        LocationGroupAudit AuditBuilder;

        public LocationGroupRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new LocationGroupAudit();
        }

        public async Task<IEnumerable<LocationGroupModel>> GetLocationGroupPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from LocationGroup limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<LocationGroupModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<LocationGroupModel>> GetLocationGroupPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from LocationGroup where 
														locationGroupId like @searchKey or 
														locationGroupName like @searchKey or 
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
                return await db.QueryAsync<LocationGroupModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<LocationGroupModel> GetLocationGroupById(string locationGroupId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from LocationGroup where 
														locationGroupId = @locationGroupId";

                var param = new DynamicParameters();
				param.Add("@locationGroupId", locationGroupId);
                return await db.QuerySingleOrDefaultAsync<LocationGroupModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> LocationGroupExists(string locationGroupId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select locationGroupId from LocationGroup where 
														locationGroupId = @locationGroupId";

                var param = new DynamicParameters();
				param.Add("@locationGroupId", locationGroupId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateLocationGroup(LocationGroupModel locationGroup)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"insert into LocationGroup(locationGroupId, 
														locationGroupName, 
														description, 
                                                        createdBy, 
                                                        modifiedBy)
 												values(@locationGroupId, 
														@locationGroupName, 
														@description,
                                                        @createdBy, 
                                                        @modifiedBy)";

                int res = await db.ExecuteAsync(strQry, locationGroup);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(locationGroup);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateLocationGroup(LocationGroupModel locationGroup)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"update LocationGroup set 
														locationGroupName = @locationGroupName, 
														description = @description, 
                                                        createdBy = @createdBy, 
                                                        modifiedBy = @modifiedBy where 
														locationGroupId = @locationGroupId";

                int res = await db.ExecuteAsync(strQry, locationGroup);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(locationGroup);

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

        public async Task<bool> DeleteLocationGroup(string locationGroupId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                var locationGroup = await GetLocationGroupById(locationGroupId);

                if (locationGroup != null)
                {
                    string strQry = @"delete from LocationGroup where 
														locationGroupId = @locationGroupId";
                    var param = new DynamicParameters();
                    param.Add("@locationGroupId", locationGroupId);
                    int res = await db.ExecuteAsync(strQry, param);

                    if (res > 0)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(locationGroup, userAccountId);

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
