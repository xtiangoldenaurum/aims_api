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
    public class InvMoveDetailRepository : IInvMoveDetailRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        InvMoveDetailAudit AuditBuilder;
        IPagingRepository PagingRepo;
        public InvMoveDetailRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            PagingRepo = new PagingRepository();
            AuditBuilder = new InvMoveDetailAudit();
        }
        public async Task<bool> ChkInvMoveDetailLock(string invMoveLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"select count(invMoveLineId) 
                                    from invmovedetail 
                                    where invMoveLineId = @invMoveLineId skip locked;";

                var param = new DynamicParameters();
                param.Add("@invMoveLineId", invMoveLineId);

                return await db.ExecuteScalarAsync<bool>(strQry, param);
            }
        }

        public async Task<bool> CreateInvMoveDetail(InvMoveDetailModel invMoveDetail)
        {
            // define po detail status
            invMoveDetail.InvMoveLineStatusId = (InvMoveLneStatus.CREATED).ToString();

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InvMoveDetail(invMoveLineId, 
														    invMoveId, 
														    invMoveLineStatusId, 
                                                            inventoryId,
                                                            finalLpnFrom,
                                                            qtyTo,
                                                            finalQty,
                                                            locationTo,
                                                            finalLocation,
                                                            lpnTo,
                                                            finalLpn,
														    createdBy, 
														    modifiedBy, 
														    remarks)
 												values(@invMoveLineId, 
														    @invMoveId, 
                                                            @invMoveLineStatusId,
                                                            @inventoryId,
                                                            @finalLpnFrom,
                                                            @qtyTo,
                                                            @finalQty,
                                                            @locationTo,
                                                            @finalLocation,
                                                            @lpnTo,
                                                            @finalLpn,
														    @createdBy, 
														    @modifiedBy, 
														    @remarks)";

                int res = await db.ExecuteAsync(strQry, invMoveDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInvMoveDetailMod(IDbConnection db, InvMoveDetailModel invMoveDetail)
        {
            string strQry = @"insert into InvMoveDetail(invMoveLineId, 
														    invMoveId, 
														    invMoveLineStatusId, 
                                                            inventoryId,
                                                            finalLpnFrom,
                                                            qtyTo,
                                                            finalQty,
                                                            locationTo,
                                                            finalLocation,
                                                            lpnTo,
                                                            finalLpn,
														    createdBy, 
														    modifiedBy, 
														    remarks)
 												values(@invMoveLineId, 
														    @invMoveId, 
                                                            @invMoveLineStatusId,
                                                            @inventoryId,
                                                            @finalLpnFrom,
                                                            @qtyTo,
                                                            @finalQty,
                                                            @locationTo,
                                                            @finalLocation,
                                                            @lpnTo,
                                                            @finalLpn,
														    @createdBy, 
														    @modifiedBy, 
														    @remarks)";

            int res = await db.ExecuteAsync(strQry, invMoveDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildInvMoveDtlAuditADD(invMoveDetail, TranType.INVMOV);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteInvMoveDetail(string invMoveLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvMoveDetail where 
														invMoveLineId = @invMoveLineId";
                var param = new DynamicParameters();
                param.Add("@invMoveLineId", invMoveLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InvMoveDetailDelResultCode> DeleteInvMoveDetailMod(string invMoveLineId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock current InvMove detail
                var invMoveDetail = await LockInvMoveDetail(db, invMoveLineId);
                if (invMoveDetail == null)
                {
                    return InvMoveDetailDelResultCode.INVMOVEDTAILINUSED;
                }

                // check if InvMove detail is still in create status
                if (invMoveDetail.InvMoveLineStatusId != (InvMoveLneStatus.CREATED).ToString())
                {
                    return InvMoveDetailDelResultCode.DETAILSTATUSMODIFIED;
                }

                // delete InvMove detail and create audit entry
                string strQry = @"delete from InvMoveDetail where 
                                                        invMoveLineId = @invMoveLineId";

                var param = new DynamicParameters();
                param.Add("@invMoveLineId", invMoveLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // alter audittrail entries containing deleted recordId
                    // append "-DEL" on recordId to avoid record inconsistency incase similar recordId is generated
                    var alteredRecId = $"{invMoveDetail.InvMoveLineId}-DEL";
                    var auditAltered = await AuditTrailRepo.AlterRecordId(db, invMoveDetail.InvMoveLineId, alteredRecId);

                    if (auditAltered)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(invMoveDetail, userAccountId, TranType.INVMOV);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                        {
                            return InvMoveDetailDelResultCode.SUCCESS;
                        }
                    }
                }
            }

            return InvMoveDetailDelResultCode.FAILED;
        }

        public async Task<InvMoveDetailModel> GetInvMoveDetailById(string invMoveLineId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvMoveDetail where 
														invMoveLineId = @invMoveLineId";

                var param = new DynamicParameters();
                param.Add("@invMoveLineId", invMoveLineId);
                return await db.QuerySingleOrDefaultAsync<InvMoveDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvMoveDetailModel> GetInvMoveDetailByIdMod(IDbConnection db, string invMoveLineId)
        {
            string strQry = @"select * from InvMoveDetail where 
													invMoveLineId = @invMoveLineId";

            var param = new DynamicParameters();
            param.Add("@invMoveLineId", invMoveLineId);
            return await db.QuerySingleOrDefaultAsync<InvMoveDetailModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<InvMoveDetailPagedMdl?> GetInvMoveDetailByInvMoveIDPaged(string invMoveId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvMoveDetail where invMoveId = @invMoveId limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@invMoveId", invMoveId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvMoveDetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvMoveDetailPageDetail(db, invMoveId, pageNum, pageItem, ret.Count());

                    return new InvMoveDetailPagedMdl()
                    {
                        Pagination = pageDetail,
                        InvMoveDetailModel = ret
                    };
                }
            }

            return null;
        }

        public async Task<InvMoveDetailPagedMdlMod?> GetInvMoveDetailByInvMoveIDPagedMod(string invMoveId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "call `spGetInvMoveDetailByInvMoveId`(@currInvMove, @pageItem, @offset)";

                var param = new DynamicParameters();
                param.Add("@currInvMoveId", invMoveId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvMoveDetailModelMod>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvMoveDetailPageDetailMod(db, invMoveId, pageNum, pageItem, ret.Count());

                    return new InvMoveDetailPagedMdlMod()
                    {
                        Pagination = pageDetail,
                        InvMoveDetailModel = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetInvMoveDetailPageDetail(IDbConnection db, string invMoveId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"select * from InvMoveDetail where invMoveId = @invMoveId";

            var param = new DynamicParameters();
            param.Add("@invMoveId", invMoveId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<Pagination?> GetInvMoveDetailPageDetailMod(IDbConnection db, string invMoveId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"call `spCountInvMoveDetailByinvMoveId`(@currInvMoveId)";

            var param = new DynamicParameters();
            param.Add("@currInvMoveId", invMoveId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<InvMoveDetailModel>> GetInvMoveDetailPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvMoveDetail limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvMoveDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvMoveDetailModel>> GetInvMoveDetailPgSrch(string searchKey, int pageNum, int pageItem)
        { 
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT imd.*
                                    FROM InvMoveDetail imd
                                    JOIN inventory inv ON imd.inventoryId = inv.inventoryId
                                    WHERE inv.sku LIKE @searchKey
                                       OR imd.invMoveLineId LIKE @searchKey
                                       OR imd.invMoveId LIKE @searchKey
                                       OR imd.finalLpnFrom LIKE @searchKey
                                       OR imd.qtyTo LIKE @searchKey
                                       OR imd.finalQty LIKE @searchKey
                                       OR imd.locationTo LIKE @searchKey
                                       OR imd.finalLocation LIKE @searchKey
                                       OR imd.lpnTo LIKE @searchKey
                                       OR imd.finalLpn LIKE @searchKey
                                       OR imd.invMoveLineStatusId LIKE @searchKey
                                       OR imd.dateCreated LIKE @searchKey
                                       OR imd.dateModified LIKE @searchKey
                                       OR imd.createdBy LIKE @searchKey
                                       OR imd.modifiedBy LIKE @searchKey
                                       OR imd.remarks LIKE @searchKey
                                    LIMIT @pageItem OFFSET @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvMoveDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<string> GetInvMoveDtlCancelMvUpdatedStatus(IDbConnection db, string invMoveDetailId, string mvId)
        {
            string strQry = @"call `spGetInvMoveDtlCancelMvUpdtStatus`(@invMoveDetailId, @mvId);";

            var param = new DynamicParameters();
            param.Add("@invMoveDetailId", invMoveDetailId);
            param.Add("@mvId", mvId);

            return await db.ExecuteScalarAsync<string>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> InvMoveDetailExists(string invMoveLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invMoveLineId from InvMoveDetail where 
														invMoveLineId = @invMoveLineId";

                var param = new DynamicParameters();
                param.Add("@invMoveLineId", invMoveLineId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InvMoveDetailModel> LockInvMoveDetail(IDbConnection db, string invMoveLineId)
        {
            string strQry = @"SELECT * FROM invmovedetail WHERE invMoveLineId = @invMoveLineId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@invMoveLineId", invMoveLineId);

            return await db.QuerySingleOrDefaultAsync<InvMoveDetailModel>(strQry, param);
        }

        public async Task<IEnumerable<InvMoveDetailModel>> LockInvMoveDetails(IDbConnection db, string invMoveId)
        {
            string strQry = @"SELECT * FROM invmovedetail WHERE invMoveId = @invMoveId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@invMoveId", invMoveId);

            return await db.QueryAsync<InvMoveDetailModel>(strQry, param);
        }

        public async Task<bool> UpdateInvMoveDetail(InvMoveDetailModel invMoveDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InvMoveDetail set 
														invMoveId = @invMoveId, 
														invMoveLineStatusId = @invMoveLineStatusId, 
                                                        inventoryId = @inventoryId,
                                                        qtyTo = @qtyTo,
                                                        locationTo = @locationTo,
                                                        lpnTo = @lpnTo,
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														invMoveLineId = @invMoveLineId";

                int res = await db.ExecuteAsync(strQry, invMoveDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvMoveDetailMod(IDbConnection db, InvMoveDetailModel invMoveDetail, TranType tranTyp)
        {
            string strQry = @"update InvMoveDetail set 
														invMoveId = @invMoveId, 
														invMoveLineStatusId = @invMoveLineStatusId, 
                                                        inventoryId = @inventoryId,
                                                        qtyTo = @qtyTo,
                                                        locationTo = @locationTo,
                                                        lpnTo = @lpnTo,
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														invMoveLineId = @invMoveLineId";

            int res = await db.ExecuteAsync(strQry, invMoveDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildInvMoveDtlAuditMOD(invMoveDetail, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
