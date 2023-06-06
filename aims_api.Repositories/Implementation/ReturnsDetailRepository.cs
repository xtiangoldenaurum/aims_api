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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class ReturnsDetailRepository : IReturnsDetailRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        ReturnsDetailAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public ReturnsDetailRepository(ITenantProvider tenantProvider, 
                                        IAuditTrailRepository auditTrailRepo, 
                                        IPagingRepository pagingRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new ReturnsDetailAudit();
            PagingRepo = pagingRepo;
        }

        public async Task<ReturnsDetailPagedMdl?> GetRetDetailByRetIdPaged(string returnsId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * 
                                    from ReturnsDetail 
                                    where returnsId = @returnsId 
                                    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@returnsId", returnsId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<ReturnsDetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetReturnsDetailPageDetail(db, returnsId, pageNum, pageItem, ret.Count());

                    return new ReturnsDetailPagedMdl()
                    {
                        Pagination = pageDetail,
                        ReturnsDetailModel = ret
                    };
                }
            }

            return null;
        }

        public async Task<RetDetailPagedMdlMod?> GetRetDetailByRetIdPagedMod(string returnsId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "call `spGetRetDetailByRetId`(@currRetId, @pageItem, @offset)"; ;

                var param = new DynamicParameters();
                param.Add("@currRetId", returnsId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<ReturnsDetailModelMod>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetReturnsDetailPageDetailMod(db, returnsId, pageNum, pageItem, ret.Count());

                    return new RetDetailPagedMdlMod()
                    {
                        Pagination = pageDetail,
                        RetDetailModel = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetReturnsDetailPageDetail(IDbConnection db, string returnsId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"select count(returnsLineId) from returnsdetail where returnsId = @returnsId";

            var param = new DynamicParameters();
            param.Add("@returnsId", returnsId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<Pagination?> GetReturnsDetailPageDetailMod(IDbConnection db, string returnsId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"call `spCountRetDetailByRetId`(@currRetId)";

            var param = new DynamicParameters();
            param.Add("@currRetId", returnsId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<ReturnsDetailModel>> GetReturnsDetailPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from ReturnsDetail limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ReturnsDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<ReturnsDetailModel>> GetReturnsDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ReturnsDetail where 
														returnsLineId like @searchKey or 
														returnsId like @searchKey or 
														sku like @searchKey or 
														expectedQty like @searchKey or 
														returnsLineStatusId like @searchKey or 
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
                return await db.QueryAsync<ReturnsDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ReturnsDetailModel> GetReturnsDetailById(string returnsLineId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ReturnsDetail where 
														returnsLineId = @returnsLineId";

                var param = new DynamicParameters();
                param.Add("@returnsLineId", returnsLineId);
                return await db.QuerySingleOrDefaultAsync<ReturnsDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ReturnsDetailModel> GetReturnsDetailByIdMod(IDbConnection db, string returnsLineId)
        {
            string strQry = @"select * from ReturnsDetail where 
														returnsLineId = @returnsLineId";

            var param = new DynamicParameters();
            param.Add("@returnsLineId", returnsLineId);
            return await db.QuerySingleOrDefaultAsync<ReturnsDetailModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<string> GetRetDtlCancelRcvUpdatedStatus(IDbConnection db, string retDetailId, string receivingId)
        {
            string strQry = @"call `spGetRetDtlCancelRcvUpdtStatus`(@retDetailId, @rcvId);";

            var param = new DynamicParameters();
            param.Add("@retDetailId", retDetailId);
            param.Add("@rcvId", receivingId);

            return await db.ExecuteScalarAsync<string>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> ReturnsDetailExists(string returnsLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select returnsLineId from ReturnsDetail where 
														returnsLineId = @returnsLineId";

                var param = new DynamicParameters();
                param.Add("@returnsLineId", returnsLineId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> ChkRetDetailLock(string retLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"select count(returnsLineId) 
                                    from returnsdetail 
                                    where returnsLineId = @retLineId skip locked;";

                var param = new DynamicParameters();
                param.Add("@retLineId", retLineId);

                return await db.ExecuteScalarAsync<bool>(strQry, param);
            }
        }

        public async Task<bool> RetDetailReceivable(string retLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select count(returnsLineId) from returnsdetail where 
                                                        (returnsLineStatusId = 'CREATED' or 
                                                        returnsLineStatusId = 'PRTRCV') and 
														returnsLineId = @retLineId";

                var param = new DynamicParameters();
                param.Add("@retLineId", retLineId);

                var res = await db.ExecuteScalarAsync<bool>(strQry, param);
                if (res)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<ReturnsDetailModel> LockRetDetail(IDbConnection db, string retLineId)
        {
            string strQry = @"SELECT * FROM returnsdetail WHERE returnsLineId = @retLineId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@retLineId", retLineId);

            return await db.QuerySingleOrDefaultAsync<ReturnsDetailModel>(strQry, param);
        }

        public async Task<IEnumerable<ReturnsDetailModel>> LockReturnsDetails(IDbConnection db, string returnsId)
        {
            string strQry = @"SELECT * FROM returnsdetail WHERE returnsId = @returnsId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@returnsId", returnsId);

            return await db.QueryAsync<ReturnsDetailModel>(strQry, param);
        }

        public async Task<bool> CreateReturnsDetail(ReturnsDetailModel returnsDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into ReturnsDetail(returnsLineId, 
														returnsId, 
														sku, 
														expectedQty, 
														returnsLineStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@returnsLineId, 
														@returnsId, 
														@sku, 
														@expectedQty, 
														@returnsLineStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

                int res = await db.ExecuteAsync(strQry, returnsDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateReturnsDetailMod(IDbConnection db, ReturnsDetailModel returnsDetail)
        {
            string strQry = @"insert into ReturnsDetail(returnsLineId, 
														returnsId, 
														sku, 
														expectedQty, 
														returnsLineStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@returnsLineId, 
														@returnsId, 
														@sku, 
														@expectedQty, 
														@returnsLineStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, returnsDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildPODtlAuditADD(returnsDetail, TranType.RCVRET);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateReturnsDetailMod(IDbConnection db, ReturnsDetailModel returnsDetail, TranType tranTyp)
        {
            string strQry = @"update ReturnsDetail set 
													returnsId = @returnsId, 
													sku = @sku, 
													expectedQty = @expectedQty, 
													returnsLineStatusId = @returnsLineStatusId, 
													modifiedBy = @modifiedBy, 
													remarks = @remarks where 
													returnsLineId = @returnsLineId";

            int res = await db.ExecuteAsync(strQry, returnsDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildPODtlAuditMOD(returnsDetail, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateReturnsDetail(ReturnsDetailModel returnsDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update ReturnsDetail set 
														returnsId = @returnsId, 
														sku = @sku, 
														expectedQty = @expectedQty, 
														returnsLineStatusId = @returnsLineStatusId, 
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														returnsLineId = @returnsLineId";

                int res = await db.ExecuteAsync(strQry, returnsDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteReturnsDetail(string returnsLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from ReturnsDetail where 
														returnsLineId = @returnsLineId";
                var param = new DynamicParameters();
                param.Add("@returnsLineId", returnsLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<RetDetailDelResultCode> DeleteRetDetailMod(string retLineId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock current returns detail
                var retDetail = await LockRetDetail(db, retLineId);
                if (retDetail == null)
                {
                    return RetDetailDelResultCode.RETDTAILINUSED;
                }

                // check is returns detail is still in create status
                if (retDetail.ReturnsLineStatusId != (POLneStatus.CREATED).ToString())
                {
                    return RetDetailDelResultCode.DETAILSTATUSMODIFIED;
                }

                // delete returns detail and create audit entry
                string strQry = @"delete from returnsdetail where 
                                                        returnsLineId = @retLineId";

                var param = new DynamicParameters();
                param.Add("@retLineId", retLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // alter audittrail entries containing deleted recordId
                    // append "-DEL" on recordId to avoid record inconsistency incase similar recordId is generated
                    var alteredRecId = $"{retDetail.ReturnsLineId}-DEL";
                    var auditAltered = await AuditTrailRepo.AlterRecordId(db, retDetail.ReturnsLineId, alteredRecId);

                    if (auditAltered)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(retDetail, userAccountId, TranType.RCVRET);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                        {
                            return RetDetailDelResultCode.SUCCESS;
                        }
                    }
                }
            }

            return RetDetailDelResultCode.FAILED;
        }

    }
}
