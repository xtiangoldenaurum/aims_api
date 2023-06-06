using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities.Interface;
using Dapper;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class IdNumberRepository : IIdNumberRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        IPrintHelperRepository PrintHelperRepo;
        IdNumberAudit AuditBuilder;

        public IdNumberRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo, IPrintHelperRepository printHelperRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new IdNumberAudit();
            PrintHelperRepo = printHelperRepo;
        }

        public async Task<IEnumerable<IdNumberModel>> GetIdNumberPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from IdNumber limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<IdNumberModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<IdNumberModel>> GetIdNumberPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from IdNumber where 
														transactionTypeId like @searchKey or 
														prefix like @searchKey or 
														suffix like @searchKey or 
														startId like @searchKey or 
														lastId like @searchKey or 
														zeroesLength like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey or 
														remarks like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<IdNumberModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IdNumberModel> GetIdNumberById(string transactionTypeId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from IdNumber where 
														transactionTypeId = @transactionTypeId";

                var param = new DynamicParameters();
                param.Add("@transactionTypeId", transactionTypeId);
                return await db.QuerySingleOrDefaultAsync<IdNumberModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IdNumberModel> GetIdNumberByIdMod(IDbConnection db, string transactionTypeId)
        {

            string strQry = @"select * from IdNumber where 
														transactionTypeId = @transactionTypeId for update;";

            var param = new DynamicParameters();
            param.Add("@transactionTypeId", transactionTypeId);

            return await db.QuerySingleOrDefaultAsync<IdNumberModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> IdNumberExists(string transactionTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select transactionTypeId from IdNumber where 
														transactionTypeId = @transactionTypeId";

                var param = new DynamicParameters();
                param.Add("@transactionTypeId", transactionTypeId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<string?> GetNextIdNum(string tranTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock table
                string qryLocker = @"LOCK TABLES idnumber WRITE;";
                var lockTbl = await db.ExecuteAsync(qryLocker);

                var docDetail = await GetIdNumberByIdMod(db, tranTypeId);

                if (docDetail != null)
                {
                    // build next doc number
                    if (docDetail.ZeroesLength != null)
                    {
                        var nxtNum = docDetail.LastId + 1;
                        var formatLen = docDetail.ZeroesLength.Length.ToString();
                        var nxtDocNum = string.Format("{0:D" + formatLen + "}", nxtNum);
                        nxtDocNum = $"{docDetail.Prefix}{nxtDocNum}{docDetail.Suffix}";

                        // append last used id on doc number table
                        var res = await SaveLastUsedID(db, nxtNum, tranTypeId);

                        if (res)
                        {
                            // unlock table
                            string qryUnlocker = @"UNLOCK TABLES;";
                            var unlockTbl = await db.ExecuteAsync(qryUnlocker);

                            // log audit
                            var audit = await AuditBuilder.BuildTranAuditMOD(docDetail);
                            audit.Remarks += " - NxtDocNum: " + nxtDocNum;

                            if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                            {
                                return nxtDocNum;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public async Task<string?> GetNxtDocNum(string tranTypeId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string? nxtdocNum = null;

                string strQry = @"call spGetNxtDocNum(@tranTypeId)";

                var param = new DynamicParameters();
                param.Add("@tranTypeId", tranTypeId);

                var res = await db.ExecuteReaderAsync(strQry, param);
                var hasNxtResult = res.NextResult();

                if (hasNxtResult)
                {
                    while (res.Read())
                    {
                        if (res["NxtDocNum"] != null && res["NxtDocNum"] != DBNull.Value)
                        {
                            nxtdocNum = (string?)res["NxtDocNum"];
                        }
                    }
                }

                // log audit
                var audit = await AuditBuilder.BuildTranAuditDocNum(tranTypeId, nxtdocNum, userAccountId);
                var logged = await AuditTrailRepo.CreateAuditTrail(audit);

                return nxtdocNum;
            }
        }

        public async Task<bool> SaveLastUsedID(IDbConnection db, int lastId, string tranTypeId)
        {
            string strQry = @"update idnumber set lastId = @lastId where transactionTypeId = @tranTypeId";

            var param = new DynamicParameters();
            param.Add("@lastId", lastId);
            param.Add("@tranTypeId", tranTypeId);

            var res = await db.ExecuteAsync(strQry, param, commandType: CommandType.Text);

            if (res > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> CreateIdNumber(IdNumberModel idNumber)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"insert into IdNumber(transactionTypeId, 
														prefix, 
														suffix, 
														startId, 
														lastId, 
														zeroesLength, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@transactionTypeId, 
														@prefix, 
														@suffix, 
														@startId, 
														@lastId, 
														@zeroesLength, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

                int res = await db.ExecuteAsync(strQry, idNumber);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateIdNumber(IdNumberModel idNumber)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"update IdNumber set 
														prefix = @prefix, 
														suffix = @suffix, 
														zeroesLength = @zeroesLength, 
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														transactionTypeId = @transactionTypeId";

                int res = await db.ExecuteAsync(strQry, idNumber);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(idNumber);
                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }

                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteIdNumber(string transactionTypeId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"delete from IdNumber where 
														transactionTypeId = @transactionTypeId";
                var param = new DynamicParameters();
                param.Add("@transactionTypeId", transactionTypeId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<string?> GetIdPrefix(IDbConnection db, string tranTypeId)
        {
            string strQry = @"select prefix from idnumber where transactionTypeId = @tranTypeId;";

            var param = new DynamicParameters();
            param.Add("@tranTypeId", tranTypeId);

            return await db.ExecuteScalarAsync<string>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<ReceivingResultModel?> PrePrintEPC(int count)
        {
            var ret = new ReceivingResultModel();
            var tranTypeId = (TranType.EPC).ToString();

            // get to be printed id list
            var res = await PrePrintDocId(count, tranTypeId);

            if (res != null)
            {
                if (res.Any())
                {
                    // build zpl detail reference
                    var zplRefs = await PrintHelperRepo.BuildEPCZplsFromList(res);
                    if (zplRefs != null)
                    {
                        if (zplRefs.Any())
                        {
                            // build actual printing zpl details
                            ret.ZplDetails = await PrintHelperRepo.BuildZplDetails(zplRefs.ToList());
                            ret.LabelsToPrint = zplRefs;
                            return ret;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<ReceivingResultModel?> PrePrintLPN(int count)
        {
            var ret = new ReceivingResultModel();
            var tranTypeId = (TranType.LPN).ToString();

            // get to be printed id list
            var res = await PrePrintDocId(count, tranTypeId);

            if (res != null)
            {
                if (res.Any())
                {
                    // build zpl detail reference
                    var zplRefs = await PrintHelperRepo.BuildEPCZplsFromList(res);
                    if (zplRefs != null)
                    {
                        if (zplRefs.Any())
                        {
                            // build actual printing zpl details
                            ret.ZplDetails = await PrintHelperRepo.BuildZplDetails(zplRefs.ToList());
                            ret.LabelsToPrint = zplRefs;
                            return ret;
                        }
                    }
                }
            }

            return null;
        }

        private async Task<List<string>?> PrePrintDocId(int count, string tranTypeId)
        {
            // proceed if target count is not zero
            if (count > 0)
            {
                // init return holder
                var ret = new List<string>();

                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // get id number detail
                    var docDetail = await GetIdNumberByIdMod(db, tranTypeId);

                    if (docDetail != null)
                    {
                        // build next doc number
                        if (docDetail.ZeroesLength != null)
                        {
                            var lastNum = docDetail.LastId + count;
                            var startNum = docDetail.LastId + 1;
                            string nxtDocNum = string.Empty;

                            // loop each number to create formatted doc number
                            for (int i = startNum; i <= lastNum; i++)
                            {
                                // clear nxtDocNum
                                nxtDocNum = string.Empty;
                                var formatLen = docDetail.ZeroesLength.Length.ToString();
                                nxtDocNum = string.Format("{0:D" + formatLen + "}", i);
                                nxtDocNum = $"{docDetail.Prefix}{nxtDocNum}{docDetail.Suffix}";
                                ret.Add(nxtDocNum);
                            }

                            if (ret.Any())
                            {
                                // append last used id on doc number table
                                var res = await SaveLastUsedID(db, lastNum, tranTypeId);

                                // log audit
                                var audit = await AuditBuilder.BuildTranAuditMOD(docDetail);
                                audit.Remarks += " - LastDocNum: " + nxtDocNum;

                                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                                {
                                    return ret;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

    }
}
