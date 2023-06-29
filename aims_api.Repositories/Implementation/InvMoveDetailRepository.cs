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
														    sku, 
														    qtyFrom, 
														    qtyTo, 
                                                            locationFrom
                                                            locationTo
                                                            trackIdFrom
                                                            trackIdTo
														    invMoveLineStatusId, 
														    createdBy, 
														    modifiedBy, 
														    remarks)
 												values(@invMoveLineId, 
														    @invMoveId, 
														    @sku, 
														    @qtyFrom, 
														    @qtyTo, 
                                                            @locationFrom
                                                            @locationTo
                                                            @trackIdFrom
                                                            @trackIdTo
                                                            @invMoveLineStatusId
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
														    sku, 
														    qtyFrom, 
														    qtyTo, 
                                                            locationFrom
                                                            locationTo
                                                            trackIdFrom
                                                            trackIdTo
														    invMoveLineStatusId, 
														    createdBy, 
														    modifiedBy, 
														    remarks)
 												values(@invMoveLineId, 
														    @invMoveId, 
														    @sku, 
														    @qtyFrom, 
														    @qtyTo, 
                                                            @locationFrom
                                                            @locationTo
                                                            @trackIdFrom
                                                            @trackIdTo
                                                            @invMoveLineStatusId
														    @createdBy, 
														    @modifiedBy, 
														    @remarks)";

            int res = await db.ExecuteAsync(strQry, invMoveDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildPODtlAuditADD(invMoveDetail, TranType.INVMOV);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public Task<bool> DeleteInvMoveDetail(string invMoveLineId)
        {
            throw new NotImplementedException();
        }

        public Task<InvMoveDetailDelResultCode> DeleteInvMoveDetailMod(string invMoveLineId, string userAccountId)
        {
            throw new NotImplementedException();
        }

        public Task<InvMoveDetailModel> GetInvMoveDetailById(string invMoveLineId)
        {
            throw new NotImplementedException();
        }

        public Task<InvMoveDetailModel> GetInvMoveDetailByIdMod(IDbConnection db, string invMoveLineId)
        {
            throw new NotImplementedException();
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

        public Task<InvMoveDetailPagedMdlMod?> GetInvMoveDetailByInvMoveIDPagedMod(string invMoveId, int pageNum, int pageItem)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination?> GetInvMoveDetailPageDetail(IDbConnection db, string InvMoveId, int pageNum, int pageItem, int rowCount)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination?> GetInvMoveDetailPageDetailMod(IDbConnection db, string poId, int pageNum, int pageItem, int rowCount)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InvMoveDetailModel>> GetInvMoveDetailPg(int pageNum, int pageItem)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InvMoveDetailModel>> GetInvMoveDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetInvMoveDtlCancelMvUpdatedStatus(IDbConnection db, string invMoveDetailId, string invMoveId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InvMoveDetailExists(string InvMoveLineId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InvMoveDetailReceivable(string InvMoveLineId)
        {
            throw new NotImplementedException();
        }

        public Task<InvMoveDetailModel> LockInvMoveDetail(IDbConnection db, string invMoveLineId)
        {
            throw new NotImplementedException();
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
														sku = @sku, 
														qtyFrom = @qtyFrom, 
														qtyTo = @qtyTo, 
														locationFrom = @locationFrom, 
														locationTo = @locationTo, 
														trackIdFrom = @trackIdFrom, 
														trackIdTo = @trackIdTo, 
														invMoveLineStatusId = @invMoveLineStatusId, 
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
														sku = @sku, 
														qtyFrom = @qtyFrom, 
														qtyTo = @qtyTo, 
														locationFrom = @locationFrom, 
														locationTo = @locationTo, 
														trackIdFrom = @trackIdFrom, 
														trackIdTo = @trackIdTo, 
														invMoveLineStatusId = @invMoveLineStatusId, 
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														invMoveLineId = @invMoveLineId";

            int res = await db.ExecuteAsync(strQry, invMoveDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildPODtlAuditMOD(invMoveDetail, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
