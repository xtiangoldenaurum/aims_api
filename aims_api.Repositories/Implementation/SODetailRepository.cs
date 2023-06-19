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
    public class SODetailRepository : ISODetailRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        SODetailAudit AuditBuilder;

        public SODetailRepository(ITenantProvider tenantProvider,
                                  IAuditTrailRepository auditTrailRepo
                                 )
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new SODetailAudit();
        }

        public async Task<IEnumerable<SODetailModel>> GetSODetailPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from SODetail limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<SODetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<SODetailModel>> GetSODetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SODetail where 
														soLineId like @searchKey or 
														soId like @searchKey or 
														sku like @searchKey or 
														orderQty like @searchKey or 
														soLineStatusId like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey or 
														remarks like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<SODetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<SODetailModel> GetSODetailById(string soLineId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SODetail where 
														soLineId = @soLineId";

                var param = new DynamicParameters();
				param.Add("@soLineId", soLineId);
                return await db.QuerySingleOrDefaultAsync<SODetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<SODetailModel> GetSODetailByIdMod(IDbConnection db, string soLineId)
        {
            string strQry = @"select * from SODetail where 
													soLineId = @soLineId";

            var param = new DynamicParameters();
            param.Add("@soLineId", soLineId);
            return await db.QuerySingleOrDefaultAsync<SODetailModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<SODetailModel> LockSODetail(IDbConnection db, string soLineId)
        {
            string strQry = @"SELECT * FROM sodetail WHERE soLineId = @soLineId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@soLineId", soLineId);

            return await db.QuerySingleOrDefaultAsync<SODetailModel>(strQry, param);
        }

        public async Task<IEnumerable<SODetailModel>> LockSODetails(IDbConnection db, string soId)
        {
            string strQry = @"SELECT * FROM sodetail WHERE soId = @soId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@soId", soId);

            return await db.QueryAsync<SODetailModel>(strQry, param);
        }

        public async Task<bool> UpdateSODetailMod(IDbConnection db, SODetailModel soDetail, TranType tranTyp)
        {
            string strQry = @"update SODetail set 
							                        soId = @soId, 
							                        sku = @sku, 
							                        orderQty = @orderQty, 
							                        soLineStatusId = @soLineStatusId, 
							                        modifiedBy = @modifiedBy, 
							                        remarks = @remarks where 
							                        soLineId = @soLineId";

            int res = await db.ExecuteAsync(strQry, soDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildSODtlAuditMOD(soDetail, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> SODetailExists(string soLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select soLineId from SODetail where 
														soLineId = @soLineId";

                var param = new DynamicParameters();
				param.Add("@soLineId", soLineId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateSODetail(SODetailModel soDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into SODetail(soLineId, 
														soId, 
														sku, 
														orderQty, 
														soLineStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@soLineId, 
														@soId, 
														@sku, 
														@orderQty, 
														@soLineStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

                int res = await db.ExecuteAsync(strQry, soDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }
        public async Task<bool> CreateSODetailMod(IDbConnection db, SODetailModel soDetail)
        {
            string strQry = @"insert into SoDetail(soLineId, 
														soId, 
														sku, 
														orderQty, 
														soLineStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@soLineId, 
														@soId, 
														@sku, 
														@orderQty, 
														@soLineStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, soDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildSODtlAuditADD(soDetail, TranType.RCVRET);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateSODetail(SODetailModel soDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update SODetail set 
														soId = @soId, 
														sku = @sku, 
														orderQty = @orderQty, 
														soLineStatusId = @soLineStatusId, 
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														soLineId = @soLineId";

                int res = await db.ExecuteAsync(strQry, soDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteSODetail(string soLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from SODetail where 
														soLineId = @soLineId";
                var param = new DynamicParameters();
				param.Add("@soLineId", soLineId);
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
