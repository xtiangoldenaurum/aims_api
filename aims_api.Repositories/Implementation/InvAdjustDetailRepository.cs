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
    public class InvAdjustDetailRepository : IInvAdjustDetailRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        InvAdjustDetailAudit AuditBuilder;
        IPagingRepository PagingRepo;
        public InvAdjustDetailRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            PagingRepo = new PagingRepository();
            AuditBuilder = new InvAdjustDetailAudit();
        }
        public async Task<bool> ChkInvAdjustDetailLock(string invAdjustLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"select count(invAdjustLineId) 
                                    from invadjustdetail 
                                    where invAdjustLineId = @invAdjustLineId skip locked;";

                var param = new DynamicParameters();
                param.Add("@invAdjustLineId", invAdjustLineId);

                return await db.ExecuteScalarAsync<bool>(strQry, param);
            }
        }

        public async Task<bool> CreateInvAdjustDetail(InvAdjustDetailModel invAdjustDetail)
        {
            // define po detail status
            invAdjustDetail.InvAdjustLineStatusId = (InvAdjustLneStatus.CREATED).ToString();

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InvAdjustDetail(invAdjustLineId, 
														    invAdjustId, 
														    invAdjustLineStatusId, 
                                                            inventoryId,
                                                            qtyTo,
                                                            approvedBy,
														    createdBy, 
														    modifiedBy, 
														    remarks)
 												values(@invAdjustLineId, 
														    @invAdjustId, 
                                                            @invAdjustLineStatusId,
                                                            @inventoryId,
                                                            @qtyTo,
                                                            approvedBy,
														    @createdBy, 
														    @modifiedBy, 
														    @remarks)";

                int res = await db.ExecuteAsync(strQry, invAdjustDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInvAdjustDetailMod(IDbConnection db, InvAdjustDetailModel invAdjustDetail)
        {
            string strQry = @"insert into InvAdjustDetail(invAdjustLineId, 
														    invAdjustId, 
														    invAdjustLineStatusId, 
                                                            inventoryId,
                                                            qtyTo,
                                                            approvedBy,
														    createdBy, 
														    modifiedBy, 
														    remarks)
 												values(@invAdjustLineId, 
														    @invAdjustId, 
                                                            @invAdjustLineStatusId,
                                                            @inventoryId,
                                                            @qtyTo,
                                                            approvedBy,
														    @createdBy, 
														    @modifiedBy, 
														    @remarks)";

            int res = await db.ExecuteAsync(strQry, invAdjustDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildInvAdjustDtlAuditADD(invAdjustDetail, TranType.INVADJ);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteInvAdjustDetail(string invAdjustLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvAdjustDetail where 
														invAdjustLineId = @invAdjustLineId";
                var param = new DynamicParameters();
                param.Add("@invAdjustLineId", invAdjustLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InvAdjustDetailDelResultCode> DeleteInvAdjustDetailMod(string invAdjustLineId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock current InvAdjust detail
                var invAdjustDetail = await LockInvAdjustDetail(db, invAdjustLineId);
                if (invAdjustDetail == null)
                {
                    return InvAdjustDetailDelResultCode.INVADJUSTDTAILINUSED;
                }

                // check if InvAdjust detail is still in create status
                if (invAdjustDetail.InvAdjustLineStatusId != (InvAdjustLneStatus.CREATED).ToString())
                {
                    return InvAdjustDetailDelResultCode.DETAILSTATUSMODIFIED;
                }

                // delete InvAdjust detail and create audit entry
                string strQry = @"delete from InvAdjustDetail where 
                                                        invAdjustLineId = @invAdjustLineId";

                var param = new DynamicParameters();
                param.Add("@invAdjustLineId", invAdjustLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // alter audittrail entries containing deleted recordId
                    // append "-DEL" on recordId to avoid record inconsistency incase similar recordId is generated
                    var alteredRecId = $"{invAdjustDetail.InvAdjustLineId}-DEL";
                    var auditAltered = await AuditTrailRepo.AlterRecordId(db, invAdjustDetail.InvAdjustLineId, alteredRecId);

                    if (auditAltered)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(invAdjustDetail, userAccountId, TranType.INVADJ);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                        {
                            return InvAdjustDetailDelResultCode.SUCCESS;
                        }
                    }
                }
            }

            return InvAdjustDetailDelResultCode.FAILED;
        }

        public async Task<InvAdjustDetailModel> GetInvAdjustDetailById(string invAdjustLineId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvAdjustDetail where 
														invAdjustLineId = @invAdjustLineId";

                var param = new DynamicParameters();
                param.Add("@invAdjustLineId", invAdjustLineId);
                return await db.QuerySingleOrDefaultAsync<InvAdjustDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvAdjustDetailModel> GetInvAdjustDetailByIdMod(IDbConnection db, string invAdjustLineId)
        {
            string strQry = @"select * from InvAdjustDetail where 
													invAdjustLineId = @invAdjustLineId";

            var param = new DynamicParameters();
            param.Add("@invAdjustLineId", invAdjustLineId);
            return await db.QuerySingleOrDefaultAsync<InvAdjustDetailModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<InvAdjustDetailPagedMdl?> GetInvAdjustDetailByInvAdjustIDPaged(string invAdjustId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvAdjustDetail where invAdjustId = @invAdjustId limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@invAdjustId", invAdjustId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvAdjustDetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvAdjustDetailPageDetail(db, invAdjustId, pageNum, pageItem, ret.Count());

                    return new InvAdjustDetailPagedMdl()
                    {
                        Pagination = pageDetail,
                        InvAdjustDetailModel = ret
                    };
                }
            }

            return null;
        }

        public async Task<InvAdjustDetailPagedMdlMod?> GetInvAdjustDetailByInvAdjustIDPagedMod(string invAdjustId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "call `spGetInvAdjustDetailByInvAdjustId`(@currInvAdjust, @pageItem, @offset)";

                var param = new DynamicParameters();
                param.Add("@currInvAdjustId", invAdjustId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvAdjustDetailModelMod>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvAdjustDetailPageDetailMod(db, invAdjustId, pageNum, pageItem, ret.Count());

                    return new InvAdjustDetailPagedMdlMod()
                    {
                        Pagination = pageDetail,
                        InvAdjustDetailModel = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetInvAdjustDetailPageDetail(IDbConnection db, string invAdjustId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"select * from InvAdjustDetail where invAdjustId = @invAdjustId";

            var param = new DynamicParameters();
            param.Add("@invAdjustId", invAdjustId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<Pagination?> GetInvAdjustDetailPageDetailMod(IDbConnection db, string invAdjustId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"call `spCountInvAdjustDetailByinvAdjustId`(@currInvAdjustId)";

            var param = new DynamicParameters();
            param.Add("@currInvAdjustId", invAdjustId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<InvAdjustDetailModel>> GetInvAdjustDetailPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvAdjustDetail limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvAdjustDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvAdjustDetailModel>> GetInvAdjustDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT iad.*
                                    FROM InvAdjustDetail iad
                                    JOIN inventory inv ON iad.inventoryId = inv.inventoryId
                                    WHERE inv.sku LIKE @searchKey
                                       OR iad.invAdjustLineId LIKE @searchKey
                                       OR iad.invAdjustId LIKE @searchKey
                                       OR iad.qtyTo LIKE @searchKey
                                       OR iad.invAdjustLineStatusId LIKE @searchKey
                                       OR iad.approvedBy LIKE @searchKey
                                       OR iad.dateCreated LIKE @searchKey
                                       OR iad.dateModified LIKE @searchKey
                                       OR iad.createdBy LIKE @searchKey
                                       OR iad.modifiedBy LIKE @searchKey
                                       OR iad.remarks LIKE @searchKey
                                    LIMIT @pageItem OFFSET @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvAdjustDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<string> GetInvAdjustDtlCancelAdjUpdatedStatus(IDbConnection db, string invAdjustDetailId, string adjId)
        {
            string strQry = @"call `spGetInvAdjustDtlCancelAdjUpdtStatus`(@invAdjustDetailId, @adjId);";

            var param = new DynamicParameters();
            param.Add("@invAdjustDetailId", invAdjustDetailId);
            param.Add("@adjId", adjId);

            return await db.ExecuteScalarAsync<string>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> InvAdjustDetailExists(string invAdjustLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invAdjustLineId from InvAdjustDetail where 
														invAdjustLineId = @invAdjustLineId";

                var param = new DynamicParameters();
                param.Add("@invAdjustLineId", invAdjustLineId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InvAdjustDetailModel> LockInvAdjustDetail(IDbConnection db, string invAdjustLineId)
        {
            string strQry = @"SELECT * FROM invadjustdetail WHERE invAdjustLineId = @invAdjustLineId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@invAdjustLineId", invAdjustLineId);

            return await db.QuerySingleOrDefaultAsync<InvAdjustDetailModel>(strQry, param);
        }

        public async Task<IEnumerable<InvAdjustDetailModel>> LockInvAdjustDetails(IDbConnection db, string invAdjustId)
        {
            string strQry = @"SELECT * FROM invadjustdetail WHERE invAdjustId = @invAdjustId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@invAdjustId", invAdjustId);

            return await db.QueryAsync<InvAdjustDetailModel>(strQry, param);
        }

        public async Task<bool> UpdateInvAdjustDetail(InvAdjustDetailModel invAdjustDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InvAdjustDetail set 
														invAdjustId = @invAdjustId, 
														invAdjustLineStatusId = @invAdjustLineStatusId, 
                                                        inventoryId = @inventoryId,
                                                        qtyTo = @qtyTo,
                                                        approvedBy = @approvedBy,
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														invAdjustLineId = @invAdjustLineId";

                int res = await db.ExecuteAsync(strQry, invAdjustDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvAdjustDetailMod(IDbConnection db, InvAdjustDetailModel invAdjustDetail, TranType tranTyp)
        {
            string strQry = @"update InvAdjustDetail set 
														invAdjustId = @invAdjustId, 
														invAdjustLineStatusId = @invAdjustLineStatusId, 
                                                        inventoryId = @inventoryId,
                                                        qtyTo = @qtyTo,
                                                        approvedBy = @approvedBy,
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														invAdjustLineId = @invAdjustLineId";

            int res = await db.ExecuteAsync(strQry, invAdjustDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildInvAdjustDtlAuditMOD(invAdjustDetail, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
