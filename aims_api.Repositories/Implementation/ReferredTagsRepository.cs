using aims_api.Enums;
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
    public class ReferredTagsRepository : IReferredTagsRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        ReferredTagsAudit AuditBuilder;

        public ReferredTagsRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new ReferredTagsAudit();
        }

        public async Task<IEnumerable<ReferredTagsModel>> GetReferredTagsPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from ReferredTags limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ReferredTagsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ReferredTagsModel> LockReferredTags(IDbConnection db, int uniqueTagId)
        {
                string strQry = @"select * 
                                    from ReferredTags 
                                    where uniqueTagId = @uniqueTagId 
                                    and inUse = 1 
                                    FOR UPDATE;";

                var param = new DynamicParameters();
                param.Add("@uniqueTagId", uniqueTagId);

                return await db.QuerySingleOrDefaultAsync<ReferredTagsModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<IEnumerable<ReferredTagsModel>> GetReferredTagsPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ReferredTags where 
														referredTagId like @searchKey or 
														uniqueTagId like @searchKey or 
														trackId like @searchKey or 
														documentRefId like @searchKey or 
														docLineRefId like @searchKey or 
														transactionTypeId like @searchKey or 
                                                        inUse like @searchKey or 
														dateCreated like @searchKey or 
														createdBy like @searchKey or 
														remarks like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ReferredTagsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ReferredTagsModel> GetReferredTagsById(int referredTagId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ReferredTags where 
														referredTagId = @referredTagId";

                var param = new DynamicParameters();
                param.Add("@referredTagId", referredTagId);
                return await db.QuerySingleOrDefaultAsync<ReferredTagsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> ReferredTagsExists(int referredTagId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select referredTagId from ReferredTags where 
														referredTagId = @referredTagId";

                var param = new DynamicParameters();
                param.Add("@referredTagId", referredTagId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateReferredTags(ReferredTagsModel referredTags)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into ReferredTags(referredTagId, 
														uniqueTagId, 
														trackId, 
														documentRefId, 
														docLineRefId, 
														transactionTypeId, 
														inUse, 
														createdBy, 
														remarks)
 												values(@referredTagId, 
														@uniqueTagId, 
														@documentRefId, 
														@docLineRefId, 
														@transactionTypeId, 
														@inUse, 
														@createdBy, 
														@remarks)";

                int res = await db.ExecuteAsync(strQry, referredTags);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateReferredTagsMod(IDbConnection db, ReferredTagsModel referredTags, TranType tranType)
        {
            // update eixting referred tags under of current uniqueTagId into inUse = false
            // then insert the new inUse record

            string strQry = @"update referredtags 
                                    set inUse = 0 
                                    where uniqueTagId = @uniqueTagId; 
                                insert into ReferredTags(referredTagId, 
													uniqueTagId, 
													trackId, 
													documentRefId, 
													docLineRefId, 
													transactionTypeId, 
													inUse, 
													createdBy, 
													remarks)
 											values(@referredTagId, 
													@uniqueTagId,
                                                    @trackId,
													@documentRefId, 
													@docLineRefId, 
													@transactionTypeId, 
													@inUse, 
													@createdBy, 
													@remarks);";

            int res = await db.ExecuteAsync(strQry, referredTags);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildRefTagsAuditADD(referredTags, tranType);
                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateReferredTags(ReferredTagsModel referredTags)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update ReferredTags set 
														uniqueTagId = @uniqueTagId, 
														trackId = @trackId, 
														documentRefId = @documentRefId, 
														docLineRefId = @docLineRefId, 
														transactionTypeId = @transactionTypeId, 
														inUse = @inUse, 
														remarks = @remarks where 
														referredTagId = @referredTagId";

                int res = await db.ExecuteAsync(strQry, referredTags);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteReferredTags(int referredTagId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from ReferredTags where 
														referredTagId = @referredTagId";
                var param = new DynamicParameters();
                param.Add("@referredTagId", referredTagId);
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
