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
    public class InvCountDetailRepository : IInvCountDetailRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        InvCountDetailAudit AuditBuilder;
        IPagingRepository PagingRepo;
        public InvCountDetailRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            PagingRepo = new PagingRepository();
            AuditBuilder = new InvCountDetailAudit();
        }

        #region GetInvCountDetailByInvCountIDPaged
        ////public async Task<InvCountDetailPagedMdl?> GetInvCountDetailByInvCountIDPaged(string invCountId, int pageNum, int pageItem)
        ////{
        ////    // pagination setup
        ////    int offset = (pageNum - 1) * pageItem;
        ////    using (IDbConnection db = new MySqlConnection(ConnString))
        ////    {
        ////        db.Open();
        ////        string strQry = "select * from InvCountDetail where invCountId = @invCountId limit @pageItem offset @offset";

        ////        var param = new DynamicParameters();
        ////        param.Add("@invCountId", invCountId);
        ////        param.Add("@pageItem", pageItem);
        ////        param.Add("@offset", offset);

        ////        var ret = await db.QueryAsync<InvCountDetailModel>(strQry, param, commandType: CommandType.Text);

        ////        if (ret != null && ret.Any())
        ////        {
        ////            // build pagination detail
        ////            var pageDetail = await GetInvCountDetailPageDetail(db, invCountId, pageNum, pageItem, ret.Count());

        ////            return new InvCountDetailPagedMdl()
        ////            {
        ////                Pagination = pageDetail,
        ////                InvCountDetailModel = ret
        ////            };
        ////        }
        ////    }

        ////    return null;
        ////}
        #endregion

        public async Task<InvCountDetailPagedMdlMod?> GetInvCountDetailByInvCountIDPagedMod(string invCountId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "call `spGetInvCountDetailByInvCountId`(@currInvCountId, @pageItem, @offset)";

                var param = new DynamicParameters();
                param.Add("@currInvCountId", invCountId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<InvCountDetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetInvCountDetailPageDetailMod(db, invCountId, pageNum, pageItem, ret.Count());

                    return new InvCountDetailPagedMdlMod()
                    {
                        Pagination = pageDetail,
                        InvCountDetailModel = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetInvCountDetailPageDetailMod(IDbConnection db, string invCountId, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"call `spCountInvCountDetailByinvCountId`(@currInvCountId)";

            var param = new DynamicParameters();
            param.Add("@currInvCountId", invCountId);

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<InvCountDetailModel>> GetInvCountDetailPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvCountDetail limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvCountDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvCountDetailModel>> GetInvCountDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT imd.*, inv.sku
                                    FROM InvCountDetail imd
                                    INNER JOIN inventory inv ON imd.inventoryId = inv.inventoryId
                                    WHERE inv.sku LIKE @searchKey
                                       OR imd.invCountLineId LIKE @searchKey
                                       OR imd.invCountId LIKE @searchKey
                                       OR imd.qtyCount LIKE @searchKey
                                       OR imd.invCountLineStatusId LIKE @searchKey
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
                return await db.QueryAsync<InvCountDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<InvCountDetailModel> GetInvCountDetailById(string invCountLineId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                //  string strQry = @"select * from InvCountDetail where 
                //invCountLineId = @invCountLineId";
                string strQry = "call `spGetInvCountDetailByInvCountLineId`(@currInvCountLineId)";

                var param = new DynamicParameters();
                param.Add("@currInvCountLineId", invCountLineId);
                return await db.QuerySingleOrDefaultAsync<InvCountDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> InvCountDetailExists(string invCountLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invCountLineId from InvCountDetail where 
        						invCountLineId = @invCountLineId";

                var param = new DynamicParameters();
                param.Add("@invCountLineId", invCountLineId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvCountDetail(InvCountDetailModel invCountDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InvCountDetail set 
        						invCountId = @invCountId, 
        						invCountLineStatusId = @invCountLineStatusId, 
                                                        inventoryId = @inventoryId,
                                                        qtyCount = @qtyCount,
        						modifiedBy = @modifiedBy, 
        						remarks = @remarks where 
        						invCountLineId = @invCountLineId";

                int res = await db.ExecuteAsync(strQry, invCountDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteInvCountDetail(string invCountLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvCountDetail where 
        						invCountLineId = @invCountLineId";
                var param = new DynamicParameters();
                param.Add("@invCountLineId", invCountLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<InvCountDetailDelResultCode> DeleteInvCountDetailMod(string invCountLineId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock current InvCount detail
                var invCountDetail = await LockInvCountDetail(db, invCountLineId);
                if (invCountDetail == null)
                {
                    return InvCountDetailDelResultCode.INVCOUNTTAILINUSED;
                }

                // check if InvCount detail is still in create status
                if (invCountDetail.InvCountLineStatusId != (InvCountLneStatus.CREATED).ToString())
                {
                    return InvCountDetailDelResultCode.DETAILSTATUSMODIFIED;
                }

                // delete InvCount detail and create audit entry
                string strQry = @"delete from InvCountDetail where 
                                                        invCountLineId = @invCountLineId";

                var param = new DynamicParameters();
                param.Add("@invCountLineId", invCountLineId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // alter audittrail entries containing deleted recordId
                    // append "-DEL" on recordId to avoid record inconsistency incase similar recordId is generated
                    var alteredRecId = $"{invCountDetail.InvCountLineId}-DEL";
                    var auditAltered = await AuditTrailRepo.AlterRecordId(db, invCountDetail.InvCountLineId, alteredRecId);

                    if (auditAltered)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(invCountDetail, userAccountId, TranType.INVCNT);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                        {
                            return InvCountDetailDelResultCode.SUCCESS;
                        }
                    }
                }
            }

            return InvCountDetailDelResultCode.FAILED;
        }

        public async Task<bool> ChkInvCountDetailLock(string invCountLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"select count(invCountLineId) 
                                    from invcountdetail 
                                    where invCountLineId = @invCountLineId skip locked;";

                var param = new DynamicParameters();
                param.Add("@invCountLineId", invCountLineId);

                return await db.ExecuteScalarAsync<bool>(strQry, param);
            }
        }

        public async Task<bool> CreateInvCountDetail(InvCountDetailModel invCountDetail)
        {
            // define po detail status
            invCountDetail.InvCountLineStatusId = InvCountLneStatus.CREATED.ToString();

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InvCountDetail(invCountLineId, 
														    invCountId, 
														    invCountLineStatusId, 
                                                            inventoryId,
                                                            qtyCount,
														    createdBy, 
														    modifiedBy, 
														    remarks)
 												values(@invCountLineId, 
														    @invCountId, 
                                                            @invCountLineStatusId,
                                                            @inventoryId,
                                                            @qtyCount,
														    @createdBy, 
														    @modifiedBy, 
														    @remarks)";

                int res = await db.ExecuteAsync(strQry, invCountDetail);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateInvCountDetailMod(IDbConnection db, InvCountDetailModel invCountDetail)
        {
            string strQry = @"insert into InvCountDetail(invCountLineId, 
														    invCountId, 
														    invCountLineStatusId, 
                                                            inventoryId,
                                                            qtyCount,
														    createdBy, 
														    modifiedBy, 
														    remarks)
 												values(@invCountLineId, 
														    @invCountId, 
                                                            @invCountLineStatusId,
                                                            @inventoryId,
                                                            @qtyCount,
														    @createdBy, 
														    @modifiedBy, 
														    @remarks)";

            int res = await db.ExecuteAsync(strQry, invCountDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildInvCountDtlAuditADD(invCountDetail, TranType.INVCNT);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        #region GetInvCountDetailByIdMod
        public async Task<InvCountDetailModel> GetInvCountDetailByIdMod(IDbConnection db, string invCountLineId)
        {
            string strQry = @"select * from InvCountDetail where 
        					invCountLineId = @invCountLineId";

            var param = new DynamicParameters();
            param.Add("@invCountLineId", invCountLineId);
            return await db.QuerySingleOrDefaultAsync<InvCountDetailModel>(strQry, param, commandType: CommandType.Text);
        }
        #endregion

        #region GetInvCountDetailPageDetail
        //public async Task<Pagination?> GetInvCountDetailPageDetail(IDbConnection db, string invCountId, int pageNum, int pageItem, int rowCount)
        //{
        //    // provide query here then get page detail from paging repository
        //    string strQry = @"select * from InvCountDetail where invCountId = @invCountId";

        //    var param = new DynamicParameters();
        //    param.Add("@invCountId", invCountId);

        //    return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        //}
        #endregion

        #region GetInvCountDtlCancelMvUpdatedStatus
        //public async Task<string> GetInvCountDtlCancelMvUpdatedStatus(IDbConnection db, string invCountDetailId, string mvId)
        //{
        //    string strQry = @"call `spGetInvCountDtlCancelMvUpdtStatus`(@invCountDetailId, @mvId);";

        //    var param = new DynamicParameters();
        //    param.Add("@invCountDetailId", invCountDetailId);
        //    param.Add("@mvId", mvId);

        //    return await db.ExecuteScalarAsync<string>(strQry, param, commandType: CommandType.Text);
        //}
        #endregion

        #region InvCountDetailMovable
        //public async Task<bool> InvCountDetailMovable(string invCountLineId)
        //{
        //    using (IDbConnection db = new MySqlConnection(ConnString))
        //    {
        //        db.Open();
        //        string strQry = @"select count(invCountLineId) from InvCountDetail where 
        //                                                (invCountLineStatusId = 'CREATED' or 
        //                                                invCountLineStatusId = 'PRTMV') and 
        //						invCountLineId = @invCountLineId";

        //        var param = new DynamicParameters();
        //        param.Add("@invCountLineId", invCountLineId);

        //        var res = await db.ExecuteScalarAsync<bool>(strQry, param);
        //        if (res)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}
        #endregion

        public async Task<InvCountDetailModel> LockInvCountDetail(IDbConnection db, string invCountLineId)
        {
            string strQry = @"SELECT * FROM invcountdetail WHERE invCountLineId = @invCountLineId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@invCountLineId", invCountLineId);

            return await db.QuerySingleOrDefaultAsync<InvCountDetailModel>(strQry, param);
        }

        public async Task<IEnumerable<InvCountDetailModel>> LockInvCountDetails(IDbConnection db, string invCountId)
        {
            string strQry = @"SELECT * FROM invcountdetail WHERE invCountId = @invCountId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@invCountId", invCountId);

            return await db.QueryAsync<InvCountDetailModel>(strQry, param);
        }

        public async Task<bool> UpdateInvCountDetailMod(IDbConnection db, InvCountDetailModel invCountDetail, TranType tranTyp)
        {
            string strQry = @"update InvCountDetail set 
        						invCountId = @invCountId, 
        						invCountLineStatusId = @invCountLineStatusId, 
                                                        inventoryId = @inventoryId,
                                                        qtyCount = @qtyCount,
        						modifiedBy = @modifiedBy, 
        						remarks = @remarks where 
        						invCountLineId = @invCountLineId";

            int res = await db.ExecuteAsync(strQry, invCountDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildInvCountDtlAuditMOD(invCountDetail, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
