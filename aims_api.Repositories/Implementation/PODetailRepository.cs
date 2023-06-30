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
    public class PODetailRepository : IPODetailRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        PODetailAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public PODetailRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            PagingRepo = new PagingRepository();
            AuditBuilder = new PODetailAudit();
        }

        public async Task<PODetailPagedMdl?> GetPODetailByPoIDPaged(string poId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from PODetail where poId = @poId limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@poId", poId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<PODetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetPODetailPageDetail(db, poId, pageNum, pageItem, ret.Count());

                    return new PODetailPagedMdl()
                    {
                        Pagination = pageDetail,
                        PODetailModel = ret
                    };
                }
            }

            return null;
        }

        public async Task<PODetailPagedMdlMod?> GetPODetailByPoIDPagedMod(string poId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "call `spGetPODetailByPoId`(@currPOId, @pageItem, @offset)";

                var param = new DynamicParameters();
                param.Add("@currPOId", poId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<PODetailModelMod>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetPODetailPageDetailMod(db, poId, pageNum, pageItem, ret.Count());

                    return new PODetailPagedMdlMod()
                    {
                        Pagination = pageDetail,
                        PODetailModel = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetPODetailPageDetail(IDbConnection db, string poId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"select * from PODetail where poId = @poId";

            var param = new DynamicParameters();
            param.Add("@poId", poId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<Pagination?> GetPODetailPageDetailMod(IDbConnection db, string poId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"call `spCountPODetailByPoId`(@currPOId)";

            var param = new DynamicParameters();
            param.Add("@currPOId", poId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<PODetailModel>> GetPODetailPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from PODetail limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<PODetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<PODetailModel>> GetPODetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from PODetail where 
														poLineId like @searchKey or 
														poId like @searchKey or 
														sku like @searchKey or 
														orderQty like @searchKey or 
														poLineStatusId like @searchKey or 
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
                return await db.QueryAsync<PODetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<PODetailModel> GetPODetailById(string poLineId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from PODetail where 
														poLineId = @poLineId";

                var param = new DynamicParameters();
                param.Add("@poLineId", poLineId);
                return await db.QuerySingleOrDefaultAsync<PODetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<PODetailModel> GetPODetailByIdMod(IDbConnection db, string poLineId)
        {
            string strQry = @"select * from PODetail where 
													poLineId = @poLineId";

            var param = new DynamicParameters();
            param.Add("@poLineId", poLineId);
            return await db.QuerySingleOrDefaultAsync<PODetailModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<string> GetPODtlCancelRCvUpdatedStatus(IDbConnection db, string poDetailId, string receivingId)
        {
            string strQry = @"call `spGetPODtlCancelRcvUpdtStatus`(@poDetailId, @rcvId);";

            var param = new DynamicParameters();
            param.Add("@poDetailId", poDetailId);
            param.Add("@rcvId", receivingId);

            return await db.ExecuteScalarAsync<string>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> PODetailExists(string poLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select poLineId from PODetail where 
														poLineId = @poLineId";

                var param = new DynamicParameters();
                param.Add("@poLineId", poLineId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> ChkPoDetailLock(string poLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"select count(poLineId) 
                                    from podetail 
                                    where poLineId = @poLineId skip locked;";

                var param = new DynamicParameters();
                param.Add("@poLineId", poLineId);

                return await db.ExecuteScalarAsync<bool>(strQry, param);
            }
        }

        public async Task<bool> PODetailReceivable(string poLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select count(poLineId) from PODetail where 
                                                        (poLineStatusId = 'CREATED' or 
                                                        poLineStatusId = 'PRTRCV') and 
														poLineId = @poLineId";

                var param = new DynamicParameters();
                param.Add("@poLineId", poLineId);

                var res = await db.ExecuteScalarAsync<bool>(strQry, param);
                if (res)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<PODetailModel> LockPODetail(IDbConnection db, string poLineId)
        {
            string strQry = @"SELECT * FROM podetail WHERE poLineId = @poLineId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@poLineId", poLineId);

            return await db.QuerySingleOrDefaultAsync<PODetailModel>(strQry, param);
        }

        public async Task<IEnumerable<PODetailModel>> LockPODetails(IDbConnection db, string poId)
        {
            string strQry = @"SELECT * FROM podetail WHERE poId = @poId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@poId", poId);

            return await db.QueryAsync<PODetailModel>(strQry, param);
        }

        public async Task<bool> CreatePODetail(PODetailModel poDetail)
        {
            // define po detail status
            poDetail.PoLineStatusId = (POLneStatus.CREATED).ToString();

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into PODetail(poLineId, 
														poId, 
														sku, 
														orderQty, 
														poLineStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@poLineId, 
														@poId, 
														@sku, 
														@orderQty, 
														@poLineStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

                int res = await db.ExecuteAsync(strQry, poDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreatePODetailMod(IDbConnection db, PODetailModel poDetail)
        {
            string strQry = @"insert into PODetail(poLineId, 
														poId, 
														sku, 
														orderQty, 
														poLineStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@poLineId, 
														@poId, 
														@sku, 
														@orderQty, 
														@poLineStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, poDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildPODtlAuditADD(poDetail, TranType.PO);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdatePODetail(PODetailModel poDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update PODetail set 
														poId = @poId, 
														sku = @sku, 
														orderQty = @orderQty, 
														poLineStatusId = @poLineStatusId, 
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														poLineId = @poLineId";

                int res = await db.ExecuteAsync(strQry, poDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdatePODetailMod(IDbConnection db, PODetailModel poDetail, TranType tranTyp)
        {
            string strQry = @"update PODetail set 
							                        poId = @poId, 
							                        sku = @sku, 
							                        orderQty = @orderQty, 
							                        poLineStatusId = @poLineStatusId, 
							                        modifiedBy = @modifiedBy, 
							                        remarks = @remarks where 
							                        poLineId = @poLineId";

            int res = await db.ExecuteAsync(strQry, poDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildPODtlAuditMOD(poDetail, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeletePODetail(string poLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from PODetail where 
														poLineId = @poLineId";
                var param = new DynamicParameters();
                param.Add("@poLineId", poLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<PODetailDelResultCode> DeletePODetailMod(string poLineId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock current PO detail
                var poDetail = await LockPODetail(db, poLineId);
                if (poDetail == null)
                {
                    return PODetailDelResultCode.PODTAILINUSED;
                }

                // check if PO detail is still in create status
                if (poDetail.PoLineStatusId != (POLneStatus.CREATED).ToString())
                {
                    return PODetailDelResultCode.DETAILSTATUSMODIFIED;
                }

                // delete po detail and create audit entry
                string strQry = @"delete from PODetail where 
                                                        poLineId = @poLineId";

                var param = new DynamicParameters();
                param.Add("@poLineId", poLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // alter audittrail entries containing deleted recordId
                    // append "-DEL" on recordId to avoid record inconsistency incase similar recordId is generated
                    var alteredRecId = $"{poDetail.PoLineId}-DEL";
                    var auditAltered = await AuditTrailRepo.AlterRecordId(db, poDetail.PoLineId, alteredRecId);

                    if (auditAltered)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(poDetail, userAccountId, TranType.PO);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                        {
                            return PODetailDelResultCode.SUCCESS;
                        }
                    }
                }
            }

            return PODetailDelResultCode.FAILED;
        }

    }
}
