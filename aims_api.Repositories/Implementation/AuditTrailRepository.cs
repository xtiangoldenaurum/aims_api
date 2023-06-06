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
    public class AuditTrailRepository : IAuditTrailRepository
    {
        private string ConnString;
        IPagingRepository PagingRepo;

        public AuditTrailRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            PagingRepo = new PagingRepository();
        }

        public async Task<IEnumerable<AuditTrailModel>> GetAuditTrailPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from AuditTrail limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<AuditTrailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<AuditTrailPagedMdl?> GetAuditTrailPaged(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from AuditTrail order by auditDate desc limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<AuditTrailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "select count(auditId) from audittrail";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new AuditTrailPagedMdl()
                    {
                        Pagination = pageDetail,
                        AuditTrail = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<AuditTrailModel>> GetAuditTrailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from AuditTrail where 
														auditId like @searchKey or 
														auditDate like @searchKey or 
														userAccountId like @searchKey or 
														actionTypeId like @searchKey or 
														transactionTypeId like @searchKey or 
														recordId like @searchKey or 
														data like @searchKey or 
														remarks like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<AuditTrailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<AuditTrailPagedMdl?> GetAuditTrailSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from AuditTrail ";

                string strFltr = @"where auditId like @searchKey or 
									        auditDate like @searchKey or 
									        userAccountId like @searchKey or 
									        actionTypeId like @searchKey or 
									        transactionTypeId like @searchKey or 
									        recordId like @searchKey or 
									        data like @searchKey or 
									        remarks like @searchKey";

                strQry += $"{strFltr} order by auditDate desc limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<AuditTrailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $"select count(auditId) from audittrail {strFltr}";
                    var pgParam = new DynamicParameters();
                    pgParam.Add("@searchKey", $"%{searchKey}%");

                    var pageDeatil = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new AuditTrailPagedMdl()
                    {
                        Pagination = pageDeatil,
                        AuditTrail = ret
                    };
                }
            }

            return null;
        }

        public async Task<AuditTrailModel> GetAuditTrailById(int auditId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from AuditTrail where 
														auditId = @auditId";

                var param = new DynamicParameters();
                param.Add("@auditId", auditId);
                return await db.QuerySingleOrDefaultAsync<AuditTrailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> AuditTrailExists(int auditId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select auditId from AuditTrail where 
														auditId = @auditId";

                var param = new DynamicParameters();
                param.Add("@auditId", auditId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<IEnumerable<AuditTrailModel>> GetAuditTrailPgFiltered(AuditTrailFilterMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select * from audittrail";
            string strFltr = " where ";
            var param = new DynamicParameters();

            // build filter query
            if (!string.IsNullOrEmpty(filter.RecordId))
            {
                strFltr += $"recordId = @recordId ";
                param.Add("@recordId", filter.RecordId);
            }

            if (!string.IsNullOrEmpty(filter.UserAccountId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"userAccountId = @userAccountId ";
                param.Add("@userAccountId", filter.UserAccountId);
            }

            if (!string.IsNullOrEmpty(filter.TransactionTypeId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"transactionTypeId = @transactionTypeId ";
                param.Add("@transactionTypeId", filter.TransactionTypeId);
            }

            if (filter.AuditDateFrom != null && filter.AuditDateTo != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"date(auditDate) >= @auditDateFrom and date(auditDate) <= @auditDateTo";
                param.Add("@auditDateFrom", filter.AuditDateFrom);
                param.Add("@auditDateTo", filter.AuditDateTo);
            }
            else
            {
                if (filter.AuditDateFrom != null)
                {
                    if (strFltr != " where ")
                    {
                        strFltr += " and ";
                    }

                    strFltr += $"date(auditDate) = @auditDateFrom ";
                    param.Add("@auditDateFrom", filter.AuditDateFrom);
                }

                if (filter.AuditDateTo != null)
                {
                    if (strFltr != " where ")
                    {
                        strFltr += " and ";
                    }

                    strFltr += $"date(auditDate) = @auditDateTo ";
                    param.Add("@auditDateTo", filter.AuditDateTo);
                }
            }

            // build order by and paging
            strQry += strFltr + $" order by auditDate desc limit @pageItem offset @offset";

            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                return await db.QueryAsync<AuditTrailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<AuditTrailPagedMdl?> GetAuditTrailFltrPaged(AuditTrailFilterMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select * from audittrail";
            string strFltr = " where ";
            var param = new DynamicParameters();
            var pgParam = new DynamicParameters();

            // build filter query
            if (!string.IsNullOrEmpty(filter.RecordId))
            {
                strFltr += $"recordId = @recordId ";
                param.Add("@recordId", filter.RecordId);
            }

            if (!string.IsNullOrEmpty(filter.UserAccountId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"userAccountId = @userAccountId ";
                param.Add("@userAccountId", filter.UserAccountId);
            }

            if (!string.IsNullOrEmpty(filter.TransactionTypeId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"transactionTypeId = @transactionTypeId ";
                param.Add("@transactionTypeId", filter.TransactionTypeId);
            }

            if (filter.AuditDateFrom != null && filter.AuditDateTo != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"date(auditDate) >= @auditDateFrom and date(auditDate) <= @auditDateTo";
                param.Add("@auditDateFrom", filter.AuditDateFrom);
                param.Add("@auditDateTo", filter.AuditDateTo);
            }
            else
            {
                if (filter.AuditDateFrom != null)
                {
                    if (strFltr != " where ")
                    {
                        strFltr += " and ";
                    }

                    strFltr += $"date(auditDate) = @auditDateFrom ";
                    param.Add("@auditDateFrom", filter.AuditDateFrom);
                }

                if (filter.AuditDateTo != null)
                {
                    if (strFltr != " where ")
                    {
                        strFltr += " and ";
                    }

                    strFltr += $"date(auditDate) = @auditDateTo ";
                    param.Add("@auditDateTo", filter.AuditDateTo);
                }
            }

            // build order by and paging
            strQry += $"{strFltr} order by auditDate desc limit @pageItem offset @offset";

            // set paging parameter
            pgParam = param;

            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var ret = await db.QueryAsync<AuditTrailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $"select count(auditId) from audittrail {strFltr}";

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new AuditTrailPagedMdl()
                    {
                        Pagination = pageDetail,
                        AuditTrail = ret
                    };
                }
            }

            return null;
        }

        public async Task<bool> CreateAuditTrail(AuditTrailModel auditTrail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into AuditTrail(auditId, 
														auditDate, 
														userAccountId, 
														actionTypeId, 
														transactionTypeId, 
														recordId, 
														data, 
														remarks)
 												values(@auditId, 
														@auditDate, 
														@userAccountId, 
														@actionTypeId, 
														@transactionTypeId, 
														@recordId, 
														@data, 
														@remarks)";

                int res = await db.ExecuteAsync(strQry, auditTrail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateAuditTrail(IDbConnection db, AuditTrailModel auditTrail)
        {
            string strQry = @"insert into AuditTrail(auditId, 
														auditDate, 
														userAccountId, 
														actionTypeId, 
														transactionTypeId, 
														recordId, 
														data, 
														remarks)
 												values(@auditId, 
														@auditDate, 
														@userAccountId, 
														@actionTypeId, 
														@transactionTypeId, 
														@recordId, 
														@data, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, auditTrail);

            if (res > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAuditTrail(AuditTrailModel auditTrail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update AuditTrail set 
														auditDate = @auditDate, 
														userAccountId = @userAccountId, 
														actionTypeId = @actionTypeId, 
														transactionTypeId = @transactionTypeId, 
														recordId = @recordId, 
														data = @data, 
														remarks = @remarks where 
														auditId = @auditId";

                int res = await db.ExecuteAsync(strQry, auditTrail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteAuditTrail(int auditId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from AuditTrail where 
														auditId = @auditId";
                var param = new DynamicParameters();
                param.Add("@auditId", auditId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> AlterRecordId(IDbConnection db, string origRecordId, string newRecordId)
        {
            // just to check if old and new record id differs
            if (origRecordId != newRecordId)
            {
                string strQry = @"update audittrail 
                                    set recordId = @newRecordId 
                                    where recordId = @origRecordId";

                var param = new DynamicParameters();
                param.Add("@newRecordId", newRecordId);
                param.Add("@origRecordId", origRecordId);

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
