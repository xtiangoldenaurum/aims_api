using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
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
    public class UniqueTagsRepository : IUniqueTagsRepository
    {
        private string ConnString;
        private string EPCViewConversion;
        IAuditTrailRepository AuditTrailRepo;
        UniqueTagsAudit AuditBuilder;
        IReferredTagsRepository ReferredTagRepo;
        EPCConverter EPCConverter;

        public UniqueTagsRepository(ITenantProvider tenantProvider, 
                                    IAuditTrailRepository auditTrailRepo, 
                                    IReferredTagsRepository referredTagRepo, 
                                    EPCConverter epcConverter)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            EPCViewConversion = tenantProvider.GetTenant().EPCViewConversion;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new UniqueTagsAudit();
            ReferredTagRepo = referredTagRepo;
            EPCConverter = epcConverter;
        }

        public async Task<IEnumerable<UniqueTagsModel>> GetUniqueTagsPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from UniqueTags limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<UniqueTagsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<UniqueTagsModel>> GetUniqueTagsPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from UniqueTags where 
														uniqueTagId like @searchKey or 
														trackId like @searchKey or 
														tagId like @searchKey or 
														epc like @searchKey or 
														serialno like @searchKey or 
														documentRefId like @searchKey or 
														docLineRefId like @searchKey or 
														transactionTypeId like @searchKey or 
														dateCreated like @searchKey or 
														createdBy like @searchKey or 
														remarks like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<UniqueTagsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<UniqueTagsModel> GetUniqueTagsById(int uniqueTagId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from UniqueTags where 
														uniqueTagId = @uniqueTagId";

                var param = new DynamicParameters();
                param.Add("@uniqueTagId", uniqueTagId);
                return await db.QuerySingleOrDefaultAsync<UniqueTagsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<UniqueTagsModel>> GetUniqueTagsByTrackId(IDbConnection db, string trackId)
        {
            string strQry = @"select * 
                                from UniqueTags 
                                where trackId = @trackId";

            var param = new DynamicParameters();
            param.Add("@trackId", trackId);
            return await db.QueryAsync<UniqueTagsModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> UniqueTagsExists(int uniqueTagId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select uniqueTagId from UniqueTags where 
														uniqueTagId = @uniqueTagId";

                var param = new DynamicParameters();
                param.Add("@uniqueTagId", uniqueTagId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateUniqueTags(UniqueTagsModel uniqueTags)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into UniqueTags(uniqueTagId, 
														trackId, 
														tagId, 
														epc, 
														serialno, 
														documentRefId, 
														docLineRefId, 
														transactionTypeId, 
														createdBy, 
														remarks)
 												values(@uniqueTagId, 
														@trackId, 
														@tagId, 
														@epc, 
														@serialno, 
														@documentRefId, 
														@docLineRefId, 
														@transactionTypeId, 
														@createdBy, 
														@remarks)";

                int res = await db.ExecuteAsync(strQry, uniqueTags);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<RecordTagResultCode> CreateUniqueTagsMod(IDbConnection db,
                                                    IEnumerable<UniqueTagsModel> tags,
                                                    string trackId,
                                                    TranType tranType,
                                                    string documentRefId,
                                                    string docLineRefId,
                                                    string userAccountId)
        {
            int commitCounter = 0;

            foreach (var tag in tags)
            {
                // ensure that tag contains either of main fields TID, EPC, SN
                if (string.IsNullOrEmpty(tag.TagId) &&
                    string.IsNullOrEmpty(tag.EPC) &&
                    string.IsNullOrEmpty(tag.Serialno))
                {
                    return RecordTagResultCode.INVALIDTAGENTRY;
                }

                // prepare tag details
                tag.trackId = trackId;
                tag.DocumentRefId = documentRefId;
                tag.DocLineRefId = docLineRefId;
                tag.TransactionTypeId = (tranType).ToString();
                tag.CreatedBy = userAccountId;

                // convert epc following config conversion type
                tag.EPC = await EPCConverter.ConvertString(EPCViewConversion, tag.EPC);

                // check if it's re-capture of unique tag (occurs returned or transfer)
                var uniqTag = await ChkIfTagRecapture(db, tag);
                if (uniqTag != null)
                {
                    // proceed as recapture process
                    return await RecordRecapturedTag(db, uniqTag, tranType);
                }

                // check if has tag id conflict
                if (!string.IsNullOrEmpty(tag.TagId))
                {
                    var tagIdIssue = await ChkHasUniqueIdConflict(db, tag.TagId);
                    if (tagIdIssue)
                    {
                        return RecordTagResultCode.TAGIDCONFLICT;
                    }
                }

                // check if has epc conflict 
                if (!string.IsNullOrEmpty(tag.EPC))
                {
                    var epcIssue = await ChkHasUniqueIdConflict(db, tag.EPC);
                    if (epcIssue)
                    {
                        return RecordTagResultCode.EPCCONFLICT;
                    }
                }

                // check if has serial number conflict
                if (!string.IsNullOrEmpty(tag.Serialno))
                {
                    var snIssue = await ChkHasUniqueIdConflict(db, tag.Serialno);
                    if (snIssue)
                    {
                        return RecordTagResultCode.SERIALNUMBERCONFLICT;
                    }
                }

                string strQry = @"insert into UniqueTags(uniqueTagId, 
														trackId, 
														tagId, 
														epc, 
														serialno, 
														documentRefId, 
														docLineRefId, 
														transactionTypeId, 
														createdBy, 
														remarks)
 												values(@uniqueTagId, 
														@trackId, 
														@tagId, 
														@epc, 
														@serialno, 
														@documentRefId, 
														@docLineRefId, 
														@transactionTypeId, 
														@createdBy, 
														@remarks)";

                int res = await db.ExecuteAsync(strQry, tag);

                if (res < 1)
                {
                    return RecordTagResultCode.FAILED;
                }

                // log audit
                var audit = await AuditBuilder.BuildUniqTagsAuditADD(tag, tranType);
                if (!await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return RecordTagResultCode.FAILEDTOLOGAUDIT;
                }

                commitCounter += 1;
            }

            if (commitCounter == tags.Count())
            {
                return RecordTagResultCode.SUCCESS;
            }

            return RecordTagResultCode.FAILED;
        }

        

        public async Task<bool> UpdateUniqueTags(UniqueTagsModel uniqueTags)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update UniqueTags set 
														trackId = @trackId, 
														tagId = @tagId, 
														epc = @epc, 
														serialno = @serialno, 
														documentRefId = @documentRefId, 
														docLineRefId = @docLineRefId, 
														transactionTypeId = @transactionTypeId, 
														remarks = @remarks where 
														uniqueTagId = @uniqueTagId";

                int res = await db.ExecuteAsync(strQry, uniqueTags);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteUniqueTags(int uniqueTagId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from UniqueTags where 
														uniqueTagId = @uniqueTagId";
                var param = new DynamicParameters();
                param.Add("@uniqueTagId", uniqueTagId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelRcvResultCode> DeleteUniqueTagsMod(IDbConnection db, string trackId, string userAccountId)
        {
            // get unique tag details
            var uniqTags = await GetUniqueTagsByTrackId(db, trackId);
            if (uniqTags == null || !uniqTags.Any())
            {
                // exit function if theres no uniq tag to delete
                return CancelRcvResultCode.SUCCESS;
            }

            // delete po detail and create audit entry
            string strQry = @"delete from UniqueTags 
                                where trackId = @trackId";

            var param = new DynamicParameters();
            param.Add("@trackId", trackId);
            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                var logCounter = 0;

                // loop to each unique tag to generate individual audit entry
                foreach (var tag in uniqTags)
                {
                    var notedRecId = $"UniqueTagId-{tag.UniqueTagId}";

                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditDEL(tag, notedRecId, userAccountId, TranType.CANCELRCV);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                       logCounter += 1;
                    }
                }

                if (logCounter == uniqTags.Count())
                {
                    return CancelRcvResultCode.SUCCESS;
                }
            }

            return CancelRcvResultCode.FAILED;
        }

        private async Task<bool> ChkHasUniqueIdConflict(IDbConnection db, string uniqueId)
        {
            string strQry = @"call `spChkTagConflict`(@uniqueId);";
            var param = new DynamicParameters();
            param.Add("@uniqueId", uniqueId);
            int res = await db.ExecuteScalarAsync<int>(strQry, param);

            if (res < 1)
            {
                return false;
            }

            return true;
        }

        private async Task<UniqueTagsModel> ChkIfTagRecapture(IDbConnection db, UniqueTagsModel tag)
        {
            string strQry = @"select *
                                from uniquetags 
                                where tagId = @tagId 
                                    and epc = @epc 
                                    and serialno = @serialNo";

            var param = new DynamicParameters();
            param.Add("@tagId", tag.TagId);
            param.Add("@epc", tag.EPC);
            param.Add("@serialNo", tag.Serialno);

            return await db.QueryFirstOrDefaultAsync<UniqueTagsModel>(strQry, param);
        }

        public async Task<RecordTagResultCode> RecordRecapturedTag(IDbConnection db, UniqueTagsModel uniqTag, TranType tranType)
        {
            // lock current inUse unique tag
            var uniqTags = await ReferredTagRepo.LockReferredTags(db, uniqTag.UniqueTagId);
            if (uniqTags == null)
            {
                return RecordTagResultCode.CONFLICTEDRACAPTURE;
            }

            // build referred tag detail
            var refTag = new ReferredTagsModel()
            {
                UniqueTagId = uniqTag.UniqueTagId,
                trackId = uniqTag.trackId,
                DocLineRefId = uniqTag.DocLineRefId,
                DocumentRefId = uniqTag.DocumentRefId,
                TransactionTypeId = uniqTag.TransactionTypeId,
                InUse = true,
                CreatedBy = uniqTag.CreatedBy,
                Remarks = uniqTag.Remarks
            };

            var isSaved = await ReferredTagRepo.CreateReferredTagsMod(db, refTag, tranType);

            if (isSaved)
            {
                return RecordTagResultCode.SUCCESS;
            }

            return RecordTagResultCode.FAILED;
        }

    }
}
