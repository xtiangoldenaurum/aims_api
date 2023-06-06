using aims_api.Models;
using aims_api.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using aims_api.Utilities;
using System.Data;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using aims_api.Utilities.Interface;
using aims_api.Repositories.AuditBuilder;

namespace aims_api.Repositories.Implementation
{
    public class AccessRightRepository : IAccessRightRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        AccessRightAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public AccessRightRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            PagingRepo = new PagingRepository();
            AuditBuilder = new AccessRightAudit();
        }

        public async Task<IEnumerable<AccessRightModel>> GetAllAccessRight()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from accessright";
                return await db.QueryAsync<AccessRightModel>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<AccessRightModel>> GetAccessRightPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from accessright limit @pageItem offset @offset";
                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<AccessRightModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<AccessRightPagedMdl?> GetAccessRightPaged(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from accessright order by accessRightName limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<AccessRightModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "select count(accessRightId) from accessright";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new AccessRightPagedMdl()
                    {
                        Pagination = pageDetail,
                        AccessRight = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<AccessRightModel>> GetAccessRightPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from accessright where 
                                                                accessRightId like @searchKey  
                                                                accessRightName like @searchKey or 
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
                return await db.QueryAsync<AccessRightModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<AccessRightPagedMdl?> GetAccessRightSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from accessright ";
                string strFltr = @"where accessRightId like @searchKey or
                                        accessRightName like @searchKey or 
                                        description like @searchKey or 
                                        dateCreated like @searchKey or 
                                        dateModified like @searchKey or 
                                        createdBy like @searchKey or 
                                        modifiedBy like @searchKey ";

                strQry += strFltr + "order by accessRightName limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<AccessRightModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $"select count(accessRightId) from accessright {strFltr}";
                    var pgParam = new DynamicParameters();
                    pgParam.Add("@searchKey", $"%{searchKey}%");

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new AccessRightPagedMdl()
                    {
                        Pagination = pageDetail,
                        AccessRight = ret
                    };
                }
            }

            return null;
        }

        public async Task<AccessRightModel> GetAccessRightById(string accessRightId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from accessright where accessRightID = @accessRightId";
                var param = new DynamicParameters();
                param.Add("@accessRightId", accessRightId);
                return await db.QuerySingleOrDefaultAsync<AccessRightModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> AccessRightExists(string accessRightId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select accessRightId from accessright where accessRightId = @accessRightId";
                var param = new DynamicParameters();
                param.Add("@accessRightId", accessRightId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateAccessRight(AccessRightModel accessRight, IEnumerable<AccessRightDetailModel> details)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // create header
                string strQry = @"insert into accessright(accessRightId, 
                                                            AccessRightName, 
                                                            description, 
                                                            createdBy, 
                                                            modifiedBy) 
                                                    values(@accessRightId, 
                                                            @accessRightName, 
                                                            @description, 
                                                            @createdBy, 
                                                            @modifiedBy)";

                int headerRecorded = await db.ExecuteAsync(strQry, accessRight);

                if (headerRecorded > 0)
                {
                    // create detail
                    AccessRightDetailRepository detailRepo = new AccessRightDetailRepository();
                    bool result = await detailRepo.CreateAccessRightDetail(db, accessRight.AccessRightId, details);

                    if (result)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> CreateAccessRightHeader(AccessRightModel accessRight)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                // create header
                string strQry = @"insert into accessright(accessRightId, 
                                                            AccessRightName, 
                                                            description, 
                                                            createdBy, 
                                                            modifiedBy) 
                                                    values(@accessRightId, 
                                                            @accessRightName, 
                                                            @description, 
                                                            @createdBy, 
                                                            @modifiedBy)";

                int headerRecorded = await db.ExecuteAsync(strQry, accessRight);

                if (headerRecorded > 0)
                {
                    // log audit
                    var hdrAudit = await AuditBuilder.BuildTranAuditADD(accessRight);
                    var auditSaved = await AuditTrailRepo.CreateAuditTrail(db, hdrAudit);

                    if (auditSaved)
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateAccessRight(AccessRightModel accessRight, IEnumerable<AccessRightDetailModel> details)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                // update header
                string strQry = @"update accessright set AccessRightName = @AccessRightName, 
                                                            description = @description, 
                                                            modifiedBy = @modifiedBy where 
                                                            accessRightId = @accessRightId";

                int headerUpdated = await db.ExecuteAsync(strQry, accessRight);

                if (headerUpdated > 0)
                {
                    AccessRightDetailRepository detailRepo = new AccessRightDetailRepository();

                    // clean previous accessrightdetails
                    bool del = await detailRepo.DeletePrevAccessRightDetails(db, accessRight.AccessRightId);
                    if (del)
                    {
                        bool result = await detailRepo.CreateAccessRightDetail(db, accessRight.AccessRightId, details);

                        if (result)
                        {
                            tran.Commit();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateAccessRight(AccessRightModel accessRight, IEnumerable<AccessRightDetailModelMod> details)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                // update header
                string strQry = @"update accessright set AccessRightName = @AccessRightName, 
                                                            description = @description, 
                                                            modifiedBy = @modifiedBy where 
                                                            accessRightId = @accessRightId";

                int headerUpdated = await db.ExecuteAsync(strQry, accessRight, tran);

                if (headerUpdated > 0)
                {
                    AccessRightDetailRepository detailRepo = new AccessRightDetailRepository();

                    // check if there's existing accessright details
                    var detailsExists = await detailRepo.CheckAccessRightDetailsExists(db, accessRight.AccessRightId);
                    if (detailsExists)
                    {
                        // clean previous accessrightdetails
                        bool del = await detailRepo.DeletePrevAccessRightDetails(db, accessRight.AccessRightId);
                        if (!del)
                        {
                            return false;
                        }
                    }

                    bool result = await detailRepo.CreateAccessRightDetail(db, accessRight.AccessRightId, accessRight.CreatedBy, accessRight.ModifiedBy, details);
                    if (result)
                    {
                        // log audit - header
                        var hdrAudit = await AuditBuilder.BuildTranAuditMOD(accessRight);
                        var hdrSaved = await AuditTrailRepo.CreateAuditTrail(db, hdrAudit);

                        // log audit - detail
                        var dtlAudit = await AuditBuilder.BuildTranAuditDetailMOD(accessRight, details);
                        var dtlSaved = await AuditTrailRepo.CreateAuditTrail(db, dtlAudit);

                        if (hdrSaved && dtlSaved)
                        {
                            tran.Commit();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> AccessRightInUse(string accessRightId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select count(userAccountId) from useraccount where accessRightId = @accessRightId";
                var param = new DynamicParameters();
                param.Add("@accessRightId", accessRightId);
                var res = await db.ExecuteScalarAsync<int>(strQry, param);

                if (res == 0)
                {
                    return false;
                }

                return true;
            }
        }

        public async Task<bool> DeleteAccessRight(string accessRightId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = "delete from accessright where accessRightId = @accessRightId";
                var param = new DynamicParameters();
                param.Add("@accessRightId", accessRightId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // start audit
                    // get header
                    var hdr = await GetAccessRightById(accessRightId);

                    // get detail
                    AccessRightDetailRepository detailRepo = new AccessRightDetailRepository();
                    var dtl = await detailRepo.GetAccessRightDetailById(accessRightId);

                    if (hdr != null && dtl.Any())
                    {
                        hdr.ModifiedBy = userAccountId;

                        // log audit - header
                        var hdrAudit = await AuditBuilder.BuildTranAuditMOD(hdr);
                        var hdrSaved = await AuditTrailRepo.CreateAuditTrail(db, hdrAudit);

                        // log audit - detail
                        var dtlAudit = await AuditBuilder.BuildTranAuditDetailMOD(hdr, dtl);
                        var dtlSaved = await AuditTrailRepo.CreateAuditTrail(db, dtlAudit);

                        if (hdrSaved && dtlSaved)
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
