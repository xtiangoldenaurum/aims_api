using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Repositories.Sub;
using aims_api.Utilities.Interface;
using Dapper;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class PORepository : IPORepository
    {
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        IPODetailRepository PODetailRepo;
        IPOUserFieldRepository POUFieldRepo;
        IAuditTrailRepository AuditTrailRepo;
        PutawayTaskRepoSub PutawayTaskRepoSub;
        POAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public PORepository(ITenantProvider tenantProvider,
                            IAuditTrailRepository auditTrailRepo,
                            IIdNumberRepository idNumberRepo,
                            IPODetailRepository poDetailsRepo,
                            IPOUserFieldRepository poUFieldRepo,
                            PutawayTaskRepoSub putawayTaskRepoSub)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            PODetailRepo = poDetailsRepo;
            POUFieldRepo = poUFieldRepo;
            PutawayTaskRepoSub = putawayTaskRepoSub;
            PagingRepo = new PagingRepository();
            AuditBuilder = new POAudit();
        }

        public async Task<POPagedMdl?> GetPOPaged(int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                int offset = (pageNum - 1) * pageItem;
                string strQry = @"select po.*, 
                                            postatus.poStatus 
                                    from po 
                                    inner join postatus on po.poStatusId = postatus.poStatusId 
                                    order by poId 
                                    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<POModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetPOPageDetail(db, pageNum, pageItem, ret.Count());

                    return new POPagedMdl()
                    {
                        Pagination = pageDetail,
                        PO = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetPOPageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = "select count(poId) from po";
            return await PagingRepo.GetPageDetail(db, strQry, pageNum, pageItem, rowCount);
        }

        public async Task<POPagedMdl?> GetPOFilteredPaged(POFilteredMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select po.*, postatus.poStatus from po";
            string strFltr = " where ";
            var param = new DynamicParameters();

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(poId) from po";

            if (!string.IsNullOrEmpty(filter.SupplierId))
            {
                strFltr += $"supplierId = @supplierId ";
                param.Add("@supplierId", filter.SupplierId);
            }

            if (!string.IsNullOrEmpty(filter.CarrierId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"carrierId = @carrierId ";
                param.Add("@carrierId", filter.CarrierId);
            }

            if (filter.OrderDate != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"orderDate = @orderDate ";
                param.Add("@orderDate", filter.OrderDate);
            }

            if (!string.IsNullOrEmpty(filter.PoStatusId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"po.poStatusId = @poStatusId ";
                param.Add("@poStatusId", filter.PoStatusId);
            }

            // build inner joins
            string strJoins = @" inner join postatus on po.poStatusId = poStatus.poStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by poId";
            strQry += $" limit @pageItem offset @offset";

            // set paging param
            pgParam = param;

            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var ret = await db.QueryAsync<POModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new POPagedMdl()
                    {
                        Pagination = pageDetail,
                        PO = ret
                    };
                }
            }

            return null;
        }

        public async Task<POPagedMdl?> GetPOForRcvPaged(int pageNum, int pageItem)
        {
            string strQry = "select po.*, postatus.poStatus from po";
            string strFltr = @" where po.poStatusId = @statsCreated or 
                                        po.poStatusId = @statsPartRcv ";

            var param = new DynamicParameters();
            param.Add("@statsCreated", (POStatus.CREATED).ToString());
            param.Add("@statsPartRcv", (POStatus.PARTRCV).ToString());

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(poId) from po";

            // build inner joins
            string strJoins = @" inner join postatus on po.poStatusId = poStatus.poStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by poId desc";
            strQry += $" limit @pageItem offset @offset";

            // set paging param
            pgParam = param;

            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var ret = await db.QueryAsync<POModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new POPagedMdl()
                    {
                        Pagination = pageDetail,
                        PO = ret
                    };
                }
            }

            return null;
        }

        public async Task<POPagedMdl?> GetPOSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select po.*, postats.poStatus from PO 
                                                inner join postatus postats on po.poStatusId = postats.poStatusId 
                                    where po.poId like @searchKey or 
											    po.refNumber like @searchKey or 
											    po.refNumber2 like @searchKey or 
											    po.supplierId like @searchKey or 
											    po.supplierName like @searchKey or 
											    po.supplierAddress like @searchKey or 
											    po.supplierContact like @searchKey or 
											    po.supplierEmail like @searchKey or 
											    po.carrierId like @searchKey or 
											    po.carrierName like @searchKey or 
											    po.carrierAddress like @searchKey or 
											    po.carrierContact like @searchKey or 
											    po.carrierEmail like @searchKey or 
											    po.orderDate like @searchKey or 
											    po.arrivalDate like @searchKey or 
											    po.arrivalDate2 like @searchKey or 
											    po.poStatusId like @searchKey or 
											    po.dateCreated like @searchKey or 
											    po.dateModified like @searchKey or 
											    po.createdBy like @searchKey or 
											    po.modifiedBy like @searchKey or 
											    po.remarks like @searchKey or 
                                                postats.poStatus like @searchKey 
											    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<POModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetPOSrchPageDetail(db, searchKey, pageNum, pageItem, ret.Count());

                    return new POPagedMdl()
                    {
                        Pagination = pageDetail,
                        PO = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetPOSrchPageDetail(IDbConnection db, string searchKey, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"select count(poId) from PO 
                                    inner join postatus on po.poStatusId = postatus.poStatusId 
                              where po.poId like @searchKey or 
							        po.refNumber like @searchKey or 
							        po.refNumber2 like @searchKey or 
							        po.supplierId like @searchKey or 
							        po.supplierName like @searchKey or 
							        po.supplierAddress like @searchKey or 
							        po.supplierContact like @searchKey or 
							        po.supplierEmail like @searchKey or 
							        po.carrierId like @searchKey or 
							        po.carrierName like @searchKey or 
							        po.carrierAddress like @searchKey or 
							        po.carrierContact like @searchKey or 
							        po.carrierEmail like @searchKey or 
							        po.orderDate like @searchKey or 
							        po.arrivalDate like @searchKey or 
							        po.arrivalDate2 like @searchKey or 
							        po.poStatusId like @searchKey or 
							        po.dateCreated like @searchKey or 
							        po.dateModified like @searchKey or 
							        po.createdBy like @searchKey or 
							        po.modifiedBy like @searchKey or 
							        po.remarks like @searchKey or 
                                    postatus.poStatus like @searchKey";

            var param = new DynamicParameters();
            param.Add("@searchKey", $"%{searchKey}%");

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<POModel>> GetPOPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from PO limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<POModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<POModel>> GetPOPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from PO where 
														poId like @searchKey or 
														refNumber like @searchKey or 
														refNumber2 like @searchKey or 
														supplierId like @searchKey or 
														supplierName like @searchKey or 
														supplierAddress like @searchKey or 
														supplierContact like @searchKey or 
														supplierEmail like @searchKey or 
														carrierId like @searchKey or 
														carrierName like @searchKey or 
														carrierAddress like @searchKey or 
														carrierContact like @searchKey or 
														carrierEmail like @searchKey or 
														orderDate like @searchKey or 
														arrivalDate like @searchKey or 
														arrivalDate2 like @searchKey or 
														poStatusId like @searchKey or 
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
                return await db.QueryAsync<POModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<POModel> GetPOById(string poId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT * FROM po 
                                    INNER JOIN postatus stats 
	                                    ON po.poStatusId = stats.poStatusId 
                                    WHERE poId = @poId;";

                var param = new DynamicParameters();
                param.Add("@poId", poId);
                return await db.QuerySingleOrDefaultAsync<POModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> POExists(string poId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select poId from PO where 
														poId = @poId";

                var param = new DynamicParameters();
                param.Add("@poId", poId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> POReceivable(string poId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select count(poId) from PO where 
                                                        (poStatusId = 'CREATED' or 
                                                        poStatusId = 'PARTRCV') and 
														poId = @poId";

                var param = new DynamicParameters();
                param.Add("@poId", poId);

                var res = await db.ExecuteScalarAsync<bool>(strQry, param);
                if (res)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<POModel> LockPO(IDbConnection db, string poId)
        {
            string strQry = @"SELECT * FROM po WHERE poId = @poId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@poId", poId);

            return await db.QuerySingleOrDefaultAsync<POModel>(strQry, param);
        }

        public async Task<string?> GetPoUpdatedStatus(IDbConnection db, string poId)
        {
            string strQry = @"call `spGetPOUpdatedStatus`(@paramPOId);";

            var param = new DynamicParameters();
            param.Add("@paramPOId", poId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        public async Task<POCreateTranResult> CreatePOMod(POModelMod po)
        {
            // get PO id number
            var poId = await IdNumberRepo.GetNextIdNum("PO");

            if (!string.IsNullOrEmpty(poId) &&
                po.POHeader != null &&
                po.PODetails != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // check if expected arrival date is valid
                    if (po.POHeader.ArrivalDate != null)
                    {
                        if (po.POHeader.OrderDate > po.POHeader.ArrivalDate)
                        {
                            return new POCreateTranResult()
                            {
                                ResultCode = POTranResultCode.INVALIDARRIVALDATE
                            };
                        }
                    }

                    // check if expected arrival2 date is valid
                    if (po.POHeader.ArrivalDate2 != null)
                    {
                        if (po.POHeader.OrderDate > po.POHeader.ArrivalDate2)
                        {
                            return new POCreateTranResult()
                            {
                                ResultCode = POTranResultCode.INVALIDARRIVALDATE
                            };
                        }
                    }

                    // check if PO primary reference number are unique
                    if (!string.IsNullOrEmpty(po.POHeader.RefNumber))
                    {
                        var poCount = await ReferenceNumExists(db, po.POHeader.RefNumber);
                        if (poCount > 0)
                        {
                            return new POCreateTranResult()
                            {
                                ResultCode = POTranResultCode.INVALIDREFNUMONE
                            };
                        }
                    }

                    // check if PO secondary reference number are unique
                    if (!string.IsNullOrEmpty(po.POHeader.RefNumber2))
                    {
                        var poCount = await ReferenceNumExists(db, po.POHeader.RefNumber2);
                        if (poCount > 0)
                        {
                            return new POCreateTranResult()
                            {
                                ResultCode = POTranResultCode.INVALIDREFNUMTWO
                            };
                        }
                    }

                    // create header
                    po.POHeader.PoId = poId;
                    var headCreated = await CreatePO(db, po.POHeader);

                    if (headCreated)
                    {
                        // init po user fields default data
                        var initPOUFld = await POUFieldRepo.InitPOUField(db, poId);
                        if (!initPOUFld)
                        {
                            return new POCreateTranResult()
                            {
                                ResultCode = POTranResultCode.USRFIELDSAVEFAILED
                            };
                        }

                        // insert po user fields values
                        if (po.POUfields != null)
                        {
                            var uFieldsCreated = await POUFieldRepo.UpdatePOUField(db, poId, po.POHeader.CreatedBy, po.POUfields);
                            if (!uFieldsCreated)
                            {
                                return new POCreateTranResult()
                                {
                                    ResultCode = POTranResultCode.USRFIELDSAVEFAILED
                                };
                            }
                        }

                        // create detail
                        if (po.PODetails.Any())
                        {
                            var details = po.PODetails.ToList();

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];

                                // check if similar SKU exists under this PO
                                var skuExists = await SKUExistsInPO(db, detail.Sku, poId);
                                if (skuExists)
                                {
                                    return new POCreateTranResult()
                                    {
                                        ResultCode = POTranResultCode.SKUCONFLICT
                                    };
                                }

                                // set detail id, status and header po id
                                detail.PoLineId = $"{poId}-{i + 1}";
                                detail.PoLineStatusId = (POLneStatus.CREATED).ToString();
                                detail.PoId = poId;

                                // create detail
                                bool dtlSaved = await PODetailRepo.CreatePODetailMod(db, detail);

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return new POCreateTranResult()
                                    {
                                        ResultCode = POTranResultCode.POLINESAVEFAILED
                                    };
                                }
                            }
                        }

                        return new POCreateTranResult()
                        {
                            ResultCode = POTranResultCode.SUCCESS,
                            POId = poId
                        };
                    }
                }
            }

            return new POCreateTranResult()
            {
                ResultCode = POTranResultCode.FAILED
            };
        }

        public async Task<bool> SKUExistsInPO(IDbConnection db, string sku, string poId)
        {
            string strQry = @"select count(sku) from poDetail 
                                where sku = @sku and 
                                        poId = @poId";

            var param = new DynamicParameters();
            param.Add("@sku", sku);
            param.Add("@poId", poId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);
            if (res == 0)
            {
                return false;
            }

            // default true to ensure no conflict will occur on error
            return true;
        }

        public async Task<bool> CreatePO(IDbConnection db, POModel po)
        {
            // define po status
            po.PoStatusId = (POStatus.CREATED).ToString();

            string strQry = @"insert into PO(poId, 
														refNumber, 
														refNumber2, 
														supplierId, 
														supplierName, 
														supplierAddress, 
														supplierContact, 
														supplierEmail, 
														carrierId, 
														carrierName, 
														carrierAddress, 
														carrierContact, 
														carrierEmail, 
														orderDate, 
														arrivalDate, 
														arrivalDate2, 
														poStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@poId, 
														@refNumber, 
														@refNumber2, 
														@supplierId, 
														@supplierName, 
														@supplierAddress, 
														@supplierContact, 
														@supplierEmail, 
														@carrierId, 
														@carrierName, 
														@carrierAddress, 
														@carrierContact, 
														@carrierEmail, 
														@orderDate, 
														@arrivalDate, 
														@arrivalDate2, 
														@poStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, po);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditADD(po, TranType.PO);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<POTranResultCode> UpdatePOMod(POModelMod po)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // check if PO primary reference number are unique
                if (!string.IsNullOrEmpty(po.POHeader.RefNumber))
                {
                    var poCount = await ReferenceNumExists(db, po.POHeader.RefNumber, po.POHeader.PoId);
                    if (poCount > 0)
                    {
                        return POTranResultCode.INVALIDREFNUMONE;
                    }
                }

                // check if PO secondary reference number are unique
                if (!string.IsNullOrEmpty(po.POHeader.RefNumber2))
                {
                    var poCount = await ReferenceNumExists(db, po.POHeader.RefNumber2, po.POHeader.PoId);
                    if (poCount > 0)
                    {
                        return POTranResultCode.INVALIDREFNUMTWO;
                    }
                }

                // update header
                var modHeader = await UpdatePO(db, po.POHeader, TranType.PO);

                if (modHeader)
                {
                    // update po user fields values
                    if (po.POUfields != null)
                    {
                        var uFieldsCreated = await POUFieldRepo.UpdatePOUFieldMOD(db, po.POHeader.PoId, po.POHeader.ModifiedBy, po.POUfields);
                        if (!uFieldsCreated)
                        {
                            return POTranResultCode.USRFIELDSAVEFAILED;
                        }
                    }

                    // update detail
                    if (po.PODetails != null && po.PODetails.Any())
                    {
                        var details = po.PODetails.ToList();

                        // get last po detail line number
                        var poDetailsFromDb = await PODetailRepo.LockPODetails(db, po.POHeader.PoId);
                        var lastPOLneId = poDetailsFromDb.OrderByDescending(x => x.PoLineId).Select(y => y.PoLineId).FirstOrDefault();
                        int lastLneNum = 0;

                        if (!string.IsNullOrEmpty(lastPOLneId))
                        {
                            lastLneNum = Convert.ToInt32(lastPOLneId.Substring(lastPOLneId.LastIndexOf('-') + 1));
                        }
                        else
                        {
                            lastLneNum = 0;
                        }

                        for (int i = 0; i < details.Count(); i++)
                        {
                            var detail = details[i];
                            bool dtlSaved = false;

                            if (detail.PoLineId == null)
                            {
                                // check if similar SKU exists under this PO
                                var skuExists = await SKUExistsInPO(db, detail.Sku, po.POHeader.PoId);
                                if (skuExists)
                                {
                                    return POTranResultCode.SKUCONFLICT;
                                }

                                // detail concidered as new
                                // set detail id, status and header po id
                                lastLneNum += 1;
                                detail.PoLineId = $"{po.POHeader.PoId}-{lastLneNum}";
                                detail.PoLineStatusId = (POLneStatus.CREATED).ToString();
                                detail.PoId = po.POHeader.PoId;

                                // create detail
                                dtlSaved = await PODetailRepo.CreatePODetailMod(db, detail);
                            }
                            else
                            {
                                // update existing details
                                var prevDetail = await PODetailRepo.GetPODetailByIdMod(db, detail.PoLineId);

                                if (prevDetail.PoLineStatusId == (POLneStatus.CREATED).ToString())
                                {
                                    if (prevDetail != detail)
                                    {
                                        dtlSaved = await PODetailRepo.UpdatePODetailMod(db, detail, TranType.PO);
                                    }
                                }
                            }

                            // return false if either of detail failed to save
                            if (!dtlSaved)
                            {
                                return POTranResultCode.POLINESAVEFAILED;
                            }
                        }
                    }
                    return POTranResultCode.SUCCESS;
                }
            }

            return POTranResultCode.FAILED;
        }

        public async Task<bool> UpdatePO(IDbConnection db, POModel po, TranType tranTyp)
        {
            string strQry = @"update PO set 
							                refNumber = @refNumber, 
							                refNumber2 = @refNumber2, 
							                supplierId = @supplierId, 
							                supplierName = @supplierName, 
							                supplierAddress = @supplierAddress, 
							                supplierContact = @supplierContact, 
							                supplierEmail = @supplierEmail, 
							                carrierId = @carrierId, 
							                carrierName = @carrierName, 
							                carrierAddress = @carrierAddress, 
							                carrierContact = @carrierContact, 
							                carrierEmail = @carrierEmail, 
							                orderDate = @orderDate, 
							                arrivalDate = @arrivalDate, 
							                arrivalDate2 = @arrivalDate2, 
							                poStatusId = @poStatusId, 
							                modifiedBy = @modifiedBy, 
							                remarks = @remarks where 
							                poId = @poId";

            int res = await db.ExecuteAsync(strQry, po);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditMOD(po, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeletePO(string poId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from PO where 
														poId = @poId";
                var param = new DynamicParameters();
                param.Add("@poId", poId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelPOResultCode> CancelPO(string poId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get PO details
                var poDetails = await PODetailRepo.LockPODetails(db, poId);

                if (poDetails == null || !poDetails.Any())
                {
                    return CancelPOResultCode.PODETAILLOCKFAILED;
                }

                // check if PO details is all in create status
                var mods = poDetails.Where(y => y.PoLineStatusId != (POLneStatus.CREATED).ToString());
                if (mods.Any())
                {
                    return CancelPOResultCode.PODETAILSSTATUSALTERED;
                }

                // lock PO header
                var po = await LockPO(db, poId);
                if (po == null)
                {
                    return CancelPOResultCode.POLOCKFAILED;
                }

                // check if PO header is in create status
                if (po.PoStatusId != (POStatus.CREATED).ToString())
                {
                    return CancelPOResultCode.POSTATUSALTERED;
                }

                // update PO status into canceled
                po.PoStatusId = (POStatus.CANCELED).ToString();
                var poAltered = await UpdatePO(db, po, TranType.PO);

                if (!poAltered)
                {
                    return CancelPOResultCode.POSTATUSUPDATEFAILED;
                }

                // update PO details staus
                int alteredDtlCnt = 0;
                foreach (var poDetail in poDetails)
                {
                    poDetail.PoLineStatusId = (POLneStatus.CLOSED).ToString();
                    var poDtlAltered = await PODetailRepo.UpdatePODetailMod(db, poDetail, TranType.CANCELRCV);

                    if (!poDtlAltered)
                    {
                        return CancelPOResultCode.PODETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == poDetails.Count())
                {
                    return CancelPOResultCode.SUCCESS;
                }
            }

            return CancelPOResultCode.FAILED;
        }

        public async Task<CancelPOResultCode> ForceCancelPO(string poId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get PO details
                var poDetails = await PODetailRepo.LockPODetails(db, poId);

                if (poDetails == null || !poDetails.Any())
                {
                    return CancelPOResultCode.PODETAILLOCKFAILED;
                }

                // check if PO details is valid for force close process
                var detailsValid = await ChkPODtlsCanFClose(poDetails);
                if (!detailsValid)
                {
                    return CancelPOResultCode.PODETAILSNOTVALID;
                }

                // check PO details if there's no pending putaway task
                var hasPendingPutaway = await PutawayTaskRepoSub.HasPendingPutawayTaskPO(db, poId);
                if (hasPendingPutaway)
                {
                    return CancelPOResultCode.HASPUTAWAYTASKPENDING;
                }

                // lock PO header
                var po = await LockPO(db, poId);
                if (po == null)
                {
                    return CancelPOResultCode.POLOCKFAILED;
                }

                // check if PO header is in partial receive status
                if (po.PoStatusId != (POStatus.PARTRCV).ToString())
                {
                    return CancelPOResultCode.POSTATUSNOTVALID;
                }

                // update PO status into forced closed
                po.PoStatusId = (POStatus.FRCCLOSED).ToString();
                var poAltered = await UpdatePO(db, po, TranType.PO);

                if (!poAltered)
                {
                    return CancelPOResultCode.POSTATUSUPDATEFAILED;
                }

                // update PO details status
                int alteredDtlCnt = 0;
                foreach (var poDetail in poDetails)
                {
                    poDetail.PoLineStatusId = (POLneStatus.FRCCLOSED).ToString();
                    var poDtlAltered = await PODetailRepo.UpdatePODetailMod(db, poDetail, TranType.CANCELRCV);

                    if (!poDtlAltered)
                    {
                        return CancelPOResultCode.PODETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == poDetails.Count())
                {
                    return CancelPOResultCode.SUCCESS;
                }
            }

            return CancelPOResultCode.FAILED;
        }

        private async Task<bool> ChkPODtlsCanFClose(IEnumerable<PODetailModel>? poDetails)
        {
            return await Task.Run(() =>
            {
                // check if PO contains force close-able details
                var dtlCreateCnt = poDetails.Where(x => x.PoLineStatusId == (POLneStatus.CREATED).ToString()).Count();
                var dtlPrtRcvCnt = poDetails.Where(x => x.PoLineStatusId == (POLneStatus.PRTRCV).ToString()).Count();
                var dtlFullRcvCnt = poDetails.Where(x => x.PoLineStatusId == (POLneStatus.FULLRCV).ToString()).Count();

                if (dtlPrtRcvCnt > 0)
                {
                    return true;
                }
                else if (dtlCreateCnt > 0 && dtlFullRcvCnt > 0)
                {
                    return true;
                }

                return false;
            });
        }

        private async Task<int> ReferenceNumExists(IDbConnection db, string refNum)
        {
            string strQry = @"select count(poId) from po where refNumber = @refNum or refNumber2 = @refNum";

            var param = new DynamicParameters();
            param.Add("@refNum", refNum);

            return await db.ExecuteScalarAsync<int>(strQry, param);
        }

        private async Task<int> ReferenceNumExists(IDbConnection db, string refNum, string poId)
        {
            string strQry = @"select count(poId) from po 
                                where (refNumber = @refNum 
                                or refNumber2 = @refNum) 
                                and poId <> @poId";

            var param = new DynamicParameters();
            param.Add("@refNum", refNum);
            param.Add("@poId", poId);

            return await db.ExecuteScalarAsync<int>(strQry, param);
        }
        public async Task<IEnumerable<POModel>> ExportPO()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "SELECT * FROM po";
                return await db.QueryAsync<POModel>(strQry, commandType: CommandType.Text);
            }
            //using (IDbConnection db = new MySqlConnection(ConnString))
            //{
            //    db.Open();

            //    var command = db.CreateCommand();
            //    command.CommandText = "SELECT * FROM po";

            //    using (var reader = command.ExecuteReader())
            //    {
            //        var pos = new List<POModel>();

            //        while (reader.Read())
            //        {
            //            var po = new POModel
            //            {
            //                PoId = reader.GetString(reader.GetOrdinal("poId")),
            //                OrderDate = reader.GetDateTime(reader.GetOrdinal("orderDate")),
            //                RefNumber = reader.GetString(reader.GetOrdinal("refNumber")),
            //                SupplierName = reader.GetString(reader.GetOrdinal("supplierName")),
            //                PoStatusId = reader.GetString(reader.GetOrdinal("poStatusId"))
            //                // Map other properties accordingly
            //            };

            //            pos.Add(po);
            //        }

            //        return pos;
            //    }
        }
        public void CreatePO(POModel pos)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string insQry = @"insert into PO(poId, 
														refNumber, 
														refNumber2, 
														supplierId, 
														supplierName, 
														supplierAddress, 
														supplierContact, 
														supplierEmail, 
														carrierId, 
														carrierName, 
														carrierAddress, 
														carrierContact, 
														carrierEmail, 
														orderDate, 
														arrivalDate, 
														arrivalDate2, 
														poStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@poId, 
														@refNumber, 
														@refNumber2, 
														@supplierId, 
														@supplierName, 
														@supplierAddress, 
														@supplierContact, 
														@supplierEmail, 
														@carrierId, 
														@carrierName, 
														@carrierAddress, 
														@carrierContact, 
														@carrierEmail, 
														@orderDate, 
														@arrivalDate, 
														@arrivalDate2, 
														@poStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

                db.Execute(insQry, pos);
            }
        }

        public Task<POTranResultCode> ImportPOData(IFormFile file)
        {
            throw new NotImplementedException();
        }

        //public IEnumerable<POModel> ImportPOData()
        //{
        //    using (IDbConnection db = new MySqlConnection(ConnString))
        //    {
        //        db.Open();
        //        string selQry = @"SELECT * FROM po";
        //        yield return db.ExecuteScalar<POModel>(selQry);
        //    }
        //}
    }
}
