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
    public class WhTransferDetailRepository : IWhTransferDetailRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        WhTransferDetailAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public WhTransferDetailRepository(ITenantProvider tenantProvider, IPagingRepository pagingRepo, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            PagingRepo = pagingRepo;
            AuditBuilder = new WhTransferDetailAudit();
            AuditTrailRepo = auditTrailRepo;
        }

        public async Task<WhtransDetailPagedMdl?> GetWhTransDetailByTransIdPaged(string whTransferId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * 
                                    from whtransferdetail  
                                    where whTransferId = @whTransferId 
                                    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@whTransferId", whTransferId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<WhTransferDetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetWhTransDetailPageDetail(db, whTransferId, pageNum, pageItem, ret.Count());

                    return new WhtransDetailPagedMdl()
                    {
                        Pagination = pageDetail,
                        WhTransDetailModel = ret
                    };
                }
            }

            return null;
        }

        public async Task<WhtransDetailPagedMdlMod?> GetWhTransDetailByTrnasIdPagedMod(string whTransId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "call `spGetWhTransDetailByWhTransId`(@currWhTransId, @pageItem, @offset)"; ;

                var param = new DynamicParameters();
                param.Add("@currWhTransId", whTransId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<WhTransferDetailModelMod>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetWhTransDetailPageDetailMod(db, whTransId, pageNum, pageItem, ret.Count());

                    return new WhtransDetailPagedMdlMod()
                    {
                        Pagination = pageDetail,
                        WhTransDetailModel = ret
                    };
                }
            }

            return null;
        }

        private async Task<Pagination?> GetWhTransDetailPageDetail(IDbConnection db, string whTransferId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"select count(whTransferId) from whtransferdetail where whTransferId = @whTransferId";

            var param = new DynamicParameters();
            param.Add("@whTransferId", whTransferId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }
        private async Task<Pagination?> GetWhTransDetailPageDetailMod(IDbConnection db, string whTransId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"call `spCountWhTransDetailByWhTransId`(@currWhTransId)";

            var param = new DynamicParameters();
            param.Add("@currWhTransId", whTransId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<WhTransferDetailModel>> GetWhTransferDetailPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from WhTransferDetail limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<WhTransferDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<WhTransferDetailModel>> GetWhTransferDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from WhTransferDetail where 
														whTransferLineId like @searchKey or 
														whTransferId like @searchKey or 
														sku like @searchKey or 
														expectedQty like @searchKey or 
														whTransLineStatusId like @searchKey or 
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
                return await db.QueryAsync<WhTransferDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<WhTransferDetailModel> GetWhTransferDetailById(string whTransferLineId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from WhTransferDetail where 
														whTransferLineId = @whTransferLineId";

                var param = new DynamicParameters();
                param.Add("@whTransferLineId", whTransferLineId);
                return await db.QuerySingleOrDefaultAsync<WhTransferDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<WhTransferDetailModel> GetWhTransferDetailByIdmod(IDbConnection db, string whTransferLineId)
        {
            string strQry = @"select * from WhTransferDetail where 
													whTransferLineId = @whTransferLineId";

            var param = new DynamicParameters();
            param.Add("@whTransferLineId", whTransferLineId);
            return await db.QuerySingleOrDefaultAsync<WhTransferDetailModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<string> GetWhTransDtlCancelRcvUpdatedStatus(IDbConnection db, string whTransDetailId, string receivingId)
        {
            string strQry = @"call `spGetWhTransDtlCancelRcvUpdtStatus`(@whTransDetailId, @rcvId);";

            var param = new DynamicParameters();
            param.Add("@whTransDetailId", whTransDetailId);
            param.Add("@rcvId", receivingId);

            return await db.ExecuteScalarAsync<string>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> WhTransferDetailExists(string whTransferLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select whTransferLineId from WhTransferDetail where 
														whTransferLineId = @whTransferLineId";

                var param = new DynamicParameters();
                param.Add("@whTransferLineId", whTransferLineId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> ChkWhTransDetailLock(string whTransLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"select count(whTransferLineId) 
                                    from whTransferdetail 
                                    where whTransferLineId = @whTransLineId skip locked;";

                var param = new DynamicParameters();
                param.Add("@whTransLineId", whTransLineId);

                return await db.ExecuteScalarAsync<bool>(strQry, param);
            }
        }

        public async Task<bool> WhTransDetailReceivable(string whTransLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select count(whTransferLineId) from whtransferdetail where 
                                                        (whTransLineStatusId = 'CREATED' or 
                                                        whTransLineStatusId = 'PRTRCV') and 
														whTransferLineId = @whTransLineId";

                var param = new DynamicParameters();
                param.Add("@whTransLineId", whTransLineId);

                var res = await db.ExecuteScalarAsync<bool>(strQry, param);
                if (res)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<WhTransferDetailModel> LockWhTransDetail(IDbConnection db, string whTransLineId)
        {
            string strQry = @"SELECT * FROM whtransferdetail WHERE whTransferLineId = @whTransLineId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@whTransLineId", whTransLineId);

            return await db.QuerySingleOrDefaultAsync<WhTransferDetailModel>(strQry, param);
        }

        public async Task<IEnumerable<WhTransferDetailModel>> LockWhTransDetails(IDbConnection db, string whTransId)
        {
            string strQry = @"SELECT * FROM whtransferdetail WHERE whTransferId = @whTransId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@whTransId", whTransId);

            return await db.QueryAsync<WhTransferDetailModel>(strQry, param);
        }

        public async Task<bool> CreateWhTransferDetail(WhTransferDetailModel whTransferDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into WhTransferDetail(whTransferLineId, 
														whTransferId, 
														sku, 
														expectedQty, 
														whTransLineStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@whTransferLineId, 
														@whTransferId, 
														@sku, 
														@expectedQty, 
														@whTransLineStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

                int res = await db.ExecuteAsync(strQry, whTransferDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateWhTransferDetailMod(IDbConnection db, WhTransferDetailModel whTransferDetail)
        {
            string strQry = @"insert into WhTransferDetail(whTransferLineId, 
													whTransferId, 
													sku, 
													expectedQty, 
													whTransLineStatusId, 
													createdBy, 
													modifiedBy, 
													remarks)
 											values(@whTransferLineId, 
													@whTransferId, 
													@sku, 
													@expectedQty, 
													@whTransLineStatusId, 
													@createdBy, 
													@modifiedBy, 
													@remarks)";

            int res = await db.ExecuteAsync(strQry, whTransferDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildWHTransDtlAuditADD(whTransferDetail, TranType.RCVTRANS);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateWhTransDetailMod(IDbConnection db, WhTransferDetailModel whTransDetail, TranType tranTyp)
        {
            string strQry = @"update WhTransferDetail set 
													whTransferId = @whTransferId, 
													sku = @sku, 
													expectedQty = @expectedQty, 
													whTransLineStatusId = @whTransLineStatusId, 
													modifiedBy = @modifiedBy, 
													remarks = @remarks where 
													whTransferLineId = @whTransferLineId";

            int res = await db.ExecuteAsync(strQry, whTransDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildWhTransDtlAuditMOD(whTransDetail, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateWhTransferDetail(WhTransferDetailModel whTransferDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update WhTransferDetail set 
														whTransferId = @whTransferId, 
														sku = @sku, 
														expectedQty = @expectedQty, 
														whTransLineStatusId = @whTransLineStatusId, 
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														whTransferLineId = @whTransferLineId";

                int res = await db.ExecuteAsync(strQry, whTransferDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteWhTransferDetail(string whTransferLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from WhTransferDetail where 
														whTransferLineId = @whTransferLineId";
                var param = new DynamicParameters();
                param.Add("@whTransferLineId", whTransferLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<WhTransDetailDelResultCode> DeleteWhTransDetailMod(string whTransLineId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock current returns detail
                var whTransDetail = await LockWhTransDetail(db, whTransLineId);
                if (whTransDetail == null)
                {
                    return WhTransDetailDelResultCode.WHTRANSDTAILINUSED;
                }

                // check is returns detail is still in create status
                if (whTransDetail.WhTransLineStatusId != (POLneStatus.CREATED).ToString())
                {
                    return WhTransDetailDelResultCode.DETAILSTATUSMODIFIED;
                }

                // delete returns detail and create audit entry
                string strQry = @"delete from whtransferdetail where 
                                                        whTransferLineId = @whTransLineId";

                var param = new DynamicParameters();
                param.Add("@whTransLineId", whTransLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // alter audittrail entries containing deleted recordId
                    // append "-DEL" on recordId to avoid record inconsistency incase similar recordId is generated
                    var alteredRecId = $"{whTransDetail.WhTransferLineId}-DEL";
                    var auditAltered = await AuditTrailRepo.AlterRecordId(db, whTransDetail.WhTransferLineId, alteredRecId);

                    if (auditAltered)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(whTransDetail, userAccountId, TranType.RCVTRANS);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                        {
                            return WhTransDetailDelResultCode.SUCCESS;
                        }
                    }
                }
            }

            return WhTransDetailDelResultCode.FAILED;
        }

    }
}
