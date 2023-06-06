using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Repositories.Sub;
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
    public class WhTransferRepository : IWhTransferRepository
    {
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        IWhTransferDetailRepository WhTransDetailsRepo;
        IWhTransUserFieldRepository WhTransUFieldRepo;
        IAuditTrailRepository AuditTrailRepo;
        PutawayTaskRepoSub PutawayTaskRepoSub;
        WhTransferAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public WhTransferRepository(ITenantProvider tenantProvider,
                                    IAuditTrailRepository auditTrailRepo,
                                    IIdNumberRepository idNumberRepo,
                                    IWhTransferDetailRepository whTransDetailsRepo,
                                    IWhTransUserFieldRepository whTransUFieldRepo,
                                    PutawayTaskRepoSub putawayTaskRepoSub)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            WhTransDetailsRepo = whTransDetailsRepo;
            WhTransUFieldRepo = whTransUFieldRepo;
            PutawayTaskRepoSub = putawayTaskRepoSub;
            PagingRepo = new PagingRepository();
            AuditBuilder = new WhTransferAudit();
        }

        public async Task<WhTransferPagedMdl?> GetWhTransferPaged(int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                int offset = (pageNum - 1) * pageItem;
                string strQry = @"select whtransfer.*, 
                                            postatus.poStatus whTransferStatus
                                    from whtransfer 
                                    inner join postatus on whtransfer.whTransStatusId = postatus.poStatusId 
                                    order by whTransferId 
                                    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<WhTransferModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetWhTransferPageDetail(db, pageNum, pageItem, ret.Count());

                    return new WhTransferPagedMdl()
                    {
                        Pagination = pageDetail,
                        WhTransfer = ret
                    };
                }
            }

            return null;
        }

        private async Task<Pagination?> GetWhTransferPageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = "select count(whTransferId) from whtransfer";
            return await PagingRepo.GetPageDetail(db, strQry, pageNum, pageItem, rowCount);
        }

        public async Task<WhTransferPagedMdl?> GetWhTransferFilteredPaged(WhTransferFilteredMdl filter, int pageNum, int pageItem)
        {
            string strQry = @"select whtransfer.*, 
                                        postatus.poStatus whTransferStatus
                                from whtransfer";
            string strFltr = " where ";
            var param = new DynamicParameters();

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(whTransferId) from whtransfer";

            if (!string.IsNullOrEmpty(filter.WhFromId))
            {
                strFltr += $"whFromId = @whFromId ";
                param.Add("@whFromId", filter.WhFromId);
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

            if (filter.TransferDate != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"transferDate = @transferDate ";
                param.Add("@transferDate", filter.TransferDate);
            }

            if (!string.IsNullOrEmpty(filter.WhTransStatusId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"whTransStatusId = @whTransStatusId ";
                param.Add("@whTransStatusId", filter.WhTransStatusId);
            }

            // build inner joins
            string strJoins = @" inner join postatus on whtransfer.whTransStatusId = poStatus.poStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by whTransferId";
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

                var ret = await db.QueryAsync<WhTransferModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new WhTransferPagedMdl()
                    {
                        Pagination = pageDetail,
                        WhTransfer = ret
                    };
                }
            }

            return null;
        }

        public async Task<WhTransferPagedMdl?> GetWhTransferForRcvPaged(int pageNum, int pageItem)
        {
            string strQry = @"select whtransfer.*, 
                                        postatus.poStatus whTransferStatus
                                from whtransfer";
            string strFltr = @" where whTransStatusId = @statsCreated or 
                                        whTransStatusId = @statsPartRcv ";

            var param = new DynamicParameters();
            param.Add("@statsCreated", (POStatus.CREATED).ToString());
            param.Add("@statsPartRcv", (POStatus.PARTRCV).ToString());

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(whTransferId) from whtransfer";

            // build inner joins
            string strJoins = @" inner join postatus on whtransfer.whTransStatusId = poStatus.poStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by whTransferId desc";
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

                var ret = await db.QueryAsync<WhTransferModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new WhTransferPagedMdl()
                    {
                        Pagination = pageDetail,
                        WhTransfer = ret
                    };
                }
            }

            return null;
        }

        public async Task<WhTransferPagedMdl?> GetWhTransSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select whtransfer.*, 
                                            postats.poStatus whTransferStatus 
                                    from whtransfer";

                string strFltr = @" where whTransferId like @searchKey or 
								            refNumber like @searchKey or 
								            refNumber2 like @searchKey or 
								            whFromId like @searchKey or 
								            whFrom like @searchKey or 
								            whFromAddress like @searchKey or 
								            whFromContact like @searchKey or 
								            whFromEmail like @searchKey or 
										    carrierId like @searchKey or 
										    carrierName like @searchKey or 
										    carrierAddress like @searchKey or 
										    carrierContact like @searchKey or 
										    carrierEmail like @searchKey or 
										    transferDate like @searchKey or 
								            arrivalDate like @searchKey or 
								            arrivalDate2 like @searchKey or 
								            whTransStatusId like @searchKey or 
								            dateCreated like @searchKey or 
								            dateModified like @searchKey or 
								            createdBy like @searchKey or 
								            modifiedBy like @searchKey or 
								            remarks like @searchKey or
                                            postats.poStatus like @searchKey";

                // build inner joins
                string strJoins = @" inner join postatus postats on whtransfer.whTransStatusId = poStats.poStatusId";

                strQry += $"{strJoins}{strFltr} order by whTransferId limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<WhTransferModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $"select count(whTransferId) from whtransfer {strJoins}{strFltr}";
                    var pgParam = new DynamicParameters();
                    pgParam.Add("@searchKey", $"%{searchKey}%");

                    var pageDeatil = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new WhTransferPagedMdl()
                    {
                        Pagination = pageDeatil,
                        WhTransfer = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<WhTransferModel>> GetWhTransferPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from WhTransfer where 
														whTransferId like @searchKey or 
														refNumber like @searchKey or 
														whFrom like @searchKey or 
														whFromAddress like @searchKey or 
														whFromContact like @searchKey or 
														whFromEmail like @searchKey or 
														arrivalDate like @searchKey or 
														arrivalDate2 like @searchKey or 
														whTransStatusId like @searchKey or 
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
                return await db.QueryAsync<WhTransferModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<WhTransferModel> GetWhTransferById(string whTransferId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT whtrans.*, 
			                                stats.poStatus whTransferStatus 
                                FROM whtransfer whtrans
                                INNER JOIN postatus stats 
	                                ON whtrans.whTransStatusId = stats.poStatusId 
                                WHERE whtrans.whTransferId = @whTransId;";

                var param = new DynamicParameters();
                param.Add("@whTransId", whTransferId);
                return await db.QuerySingleOrDefaultAsync<WhTransferModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> WhTransferExists(string whTransferId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select whTransferId from WhTransfer where 
														whTransferId = @whTransferId";

                var param = new DynamicParameters();
                param.Add("@whTransferId", whTransferId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> WhTransferReceivable(string whTransId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select count(whTransferId) from whtransfer where 
                                                        (whTransStatusId = 'CREATED' or 
                                                        whTransStatusId = 'PARTRCV') and 
														whTransferId = @whTransId";

                var param = new DynamicParameters();
                param.Add("@whTransId", whTransId);

                var res = await db.ExecuteScalarAsync<bool>(strQry, param);
                if (res)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<WhTransferModel> LockWhTransfer(IDbConnection db, string whTransId)
        {
            string strQry = @"SELECT * FROM whtransfer WHERE whTransferId = @whTransId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@whTransId", whTransId);

            return await db.QuerySingleOrDefaultAsync<WhTransferModel>(strQry, param);
        }

        public async Task<string?> GetWhTransUpdatedStatus(IDbConnection db, string whTransId)
        {
            string strQry = @"call `spGetWhTransUpdatedStatus`(@paramWhTransId);";

            var param = new DynamicParameters();
            param.Add("@paramWhTransId", whTransId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        public async Task<WhTransCreateTranResult> CreateWhTransferMod(WhTransferModelMod whTransfer)
        {
            // get wh transfer id number
            var whTransferId = await IdNumberRepo.GetNextIdNum("RCVTRANS");

            if (!string.IsNullOrEmpty(whTransferId) &&
                whTransfer.whTransferHeader != null &&
                whTransfer.WhTransDetails != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();
                    var tran = db.BeginTransaction();

                    // check if WH transfer primary reference number are unique
                    if (!string.IsNullOrEmpty(whTransfer.whTransferHeader.RefNumber))
                    {
                        var whTransCount = await ReferenceNumExists(db, whTransfer.whTransferHeader.RefNumber);
                        if (whTransCount > 0)
                        {
                            return new WhTransCreateTranResult()
                            {
                                ResultCode = WhTransferTranResultCode.INVALIDREFNUMONE
                            };
                        }
                    }

                    // check if returns secondary reference number are unique
                    if (!string.IsNullOrEmpty(whTransfer.whTransferHeader.RefNumber2))
                    {
                        var poCount = await ReferenceNumExists(db, whTransfer.whTransferHeader.RefNumber2);
                        if (poCount > 0)
                        {
                            return new WhTransCreateTranResult()
                            {
                                ResultCode = WhTransferTranResultCode.INVALIDREFNUMTWO
                            };
                        }
                    }

                    // create header
                    whTransfer.whTransferHeader.WhTransferId = whTransferId;
                    var headCreated = await CreateWhTransfer(db, whTransfer.whTransferHeader);

                    if (headCreated)
                    {
                        // init WH transfer user fields default data
                        var initPOUFld = await WhTransUFieldRepo.InitWhTransferUField(db, whTransferId);
                        if (!initPOUFld)
                        {
                            return new WhTransCreateTranResult()
                            {
                                ResultCode = WhTransferTranResultCode.USRFIELDSAVEFAILED
                            };
                        }

                        // insert WH transfer user fields values
                        if (whTransfer.WhTransferUfields != null)
                        {
                            var uFieldsCreated = await WhTransUFieldRepo.UpdateWhTransferUField(db, whTransferId, whTransfer.whTransferHeader.CreatedBy, whTransfer.WhTransferUfields);
                            if (!uFieldsCreated)
                            {
                                return new WhTransCreateTranResult()
                                {
                                    ResultCode = WhTransferTranResultCode.USRFIELDSAVEFAILED
                                };
                            }
                        }

                        // create detail
                        if (whTransfer.WhTransDetails.Any())
                        {
                            var details = whTransfer.WhTransDetails.ToList();

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];

                                // check if similar SKU exists under this wh transfer
                                var skuExists = await SKUExistsInWhTransfer(db, detail.Sku, whTransferId);
                                if (skuExists)
                                {
                                    return new WhTransCreateTranResult()
                                    {
                                        ResultCode = WhTransferTranResultCode.SKUCONFLICT
                                    };
                                }

                                // set detail id, status and header wh transfer id
                                detail.WhTransferLineId = $"{whTransferId}-{i + 1}";
                                detail.WhTransLineStatusId = (POLneStatus.CREATED).ToString();
                                detail.WhTransferId = whTransferId;

                                // create detail
                                bool dtlSaved = await WhTransDetailsRepo.CreateWhTransferDetailMod(db, detail);

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return new WhTransCreateTranResult()
                                    {
                                        ResultCode = WhTransferTranResultCode.WHTRANSLINESAVEFAILED
                                    };
                                }
                            }
                        }

                        tran.Commit();

                        return new WhTransCreateTranResult()
                        {
                            ResultCode = WhTransferTranResultCode.SUCCESS,
                            WhTransferId = whTransferId
                        };
                    }
                }
            }

            return new WhTransCreateTranResult()
            {
                ResultCode = WhTransferTranResultCode.FAILED
            };
        }

        private async Task<bool> SKUExistsInWhTransfer(IDbConnection db, string sku, string whTransId)
        {
            string strQry = @"select count(sku) from whtransferdetail 
                                where sku = @sku and 
                                        whTransferId = @whTransId";

            var param = new DynamicParameters();
            param.Add("@sku", sku);
            param.Add("@whTransId", whTransId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);
            if (res == 0)
            {
                return false;
            }

            // default true to ensure no conflict will occur on error
            return true;
        }

        public async Task<bool> CreateWhTransfer(IDbConnection db, WhTransferModel whTransfer)
        {
            // define returns status
            whTransfer.WhTransStatusId = (POStatus.CREATED).ToString();

            string strQry = @"insert into WhTransfer(whTransferId, 
													refNumber, 
													refNumber2, 
													whFromId, 
													whFrom, 
													whFromAddress, 
													whFromContact, 
													whFromEmail, 
											        carrierId, 
											        carrierName, 
											        carrierAddress, 
											        carrierContact, 
											        carrierEmail, 
											        transferDate, 
													arrivalDate, 
													arrivalDate2, 
													whTransStatusId, 
													createdBy, 
													modifiedBy, 
													remarks)
 											values(@whTransferId, 
													@refNumber, 
													@refNumber2, 
													@whFromId, 
													@whFrom, 
													@whFromAddress, 
													@whFromContact, 
													@whFromEmail, 
													@carrierId, 
													@carrierName, 
													@carrierAddress, 
													@carrierContact, 
													@carrierEmail, 
													@transferDate, 
													@arrivalDate, 
													@arrivalDate2, 
													@whTransStatusId, 
													@createdBy, 
													@modifiedBy, 
													@remarks)";

            int res = await db.ExecuteAsync(strQry, whTransfer);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildWhTransAuditADD(whTransfer, TranType.RCVTRANS);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<WhTransferTranResultCode> UpdateWhTransferMod(WhTransferModelMod whTransfer)
        {
            if (whTransfer.whTransferHeader != null &&
                whTransfer.WhTransDetails != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();
                    var tran = db.BeginTransaction();

                    // check if WH transfer primary reference number are unique
                    if (!string.IsNullOrEmpty(whTransfer.whTransferHeader.RefNumber))
                    {
                        var whTransCount = await ReferenceNumExists(db, whTransfer.whTransferHeader.RefNumber, whTransfer.whTransferHeader.WhTransferId);
                        if (whTransCount > 0)
                        {
                            return WhTransferTranResultCode.INVALIDREFNUMONE;
                        }
                    }

                    // check if returns secondary reference number are unique
                    if (!string.IsNullOrEmpty(whTransfer.whTransferHeader.RefNumber2))
                    {
                        var returnsCount = await ReferenceNumExists(db, whTransfer.whTransferHeader.RefNumber2, whTransfer.whTransferHeader.WhTransferId);
                        if (returnsCount > 0)
                        {
                            return WhTransferTranResultCode.INVALIDREFNUMTWO;
                        }
                    }

                    // update header
                    var modHeader = await UpdateWhTransfer(db, whTransfer.whTransferHeader, TranType.RCVTRANS);

                    if (modHeader)
                    {
                        // update returns user fields values
                        if (whTransfer.WhTransferUfields != null)
                        {
                            var uFieldsCreated = await WhTransUFieldRepo.UpdateWhTransUFieldMOD(db, whTransfer.whTransferHeader.WhTransferId, whTransfer.whTransferHeader.ModifiedBy, whTransfer.WhTransferUfields);
                            if (!uFieldsCreated)
                            {
                                return WhTransferTranResultCode.USRFIELDSAVEFAILED;
                            }
                        }

                        // update detail
                        if (whTransfer.WhTransDetails != null && whTransfer.WhTransDetails.Any())
                        {
                            var details = whTransfer.WhTransDetails.ToList();

                            // get last wh transfer detail line number
                            var whTransDetailsFromDb = await WhTransDetailsRepo.LockWhTransDetails(db, whTransfer.whTransferHeader.WhTransferId);
                            var lastWhtransLneId = whTransDetailsFromDb.OrderByDescending(x => x.WhTransferLineId).Select(y => y.WhTransferLineId).FirstOrDefault();
                            int lastLneNum = 0;

                            if (!string.IsNullOrEmpty(lastWhtransLneId))
                            {
                                lastLneNum = Convert.ToInt32(lastWhtransLneId.Substring(lastWhtransLneId.LastIndexOf('-') + 1));
                            }
                            else
                            {
                                lastLneNum = 0;
                            }

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];
                                bool dtlSaved = false;

                                if (detail.WhTransferLineId == null)
                                {
                                    // check if similar SKU exists under this WH transfer
                                    var skuExists = await SKUExistsInWhTransfer(db, detail.Sku, whTransfer.whTransferHeader.WhTransferId);
                                    if (skuExists)
                                    {
                                        return WhTransferTranResultCode.SKUCONFLICT;
                                    }

                                    // detail concidered as new
                                    // set detail id, status and header WH transfer id
                                    lastLneNum += 1;
                                    detail.WhTransferLineId = $"{whTransfer.whTransferHeader.WhTransferId}-{lastLneNum}";
                                    detail.WhTransLineStatusId = (POLneStatus.CREATED).ToString();
                                    detail.WhTransferId = whTransfer.whTransferHeader.WhTransferId;

                                    // create detail
                                    dtlSaved = await WhTransDetailsRepo.CreateWhTransferDetailMod(db, detail);
                                }
                                else
                                {
                                    // update existing details
                                    var prevDetail = await WhTransDetailsRepo.GetWhTransferDetailByIdmod(db, detail.WhTransferLineId);

                                    if (prevDetail.WhTransLineStatusId == (POLneStatus.CREATED).ToString())
                                    {
                                        if (prevDetail != detail)
                                        {
                                            dtlSaved = await WhTransDetailsRepo.UpdateWhTransDetailMod(db, detail, TranType.RCVRET);
                                        }
                                    }
                                }

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return WhTransferTranResultCode.WHTRANSLINESAVEFAILED;
                                }
                            }
                        }

                        tran.Commit();
                        return WhTransferTranResultCode.SUCCESS;
                    }
                }
            }

            return WhTransferTranResultCode.FAILED;
        }

        public async Task<bool> UpdateWhTransfer(IDbConnection db, WhTransferModel whTransfer, TranType tranType)
        {
            string strQry = @"update WhTransfer set 
							                        refNumber = @refNumber, 
							                        refNumber2 = @refNumber, 
							                        whFromId = @whFromId, 
							                        whFrom = @whFrom, 
							                        whFromAddress = @whFromAddress, 
							                        whFromContact = @whFromContact, 
							                        whFromEmail = @whFromEmail, 
							                        carrierId = @carrierId, 
							                        carrierName = @carrierName, 
							                        carrierAddress = @carrierAddress, 
							                        carrierContact = @carrierContact, 
							                        carrierEmail = @carrierEmail, 
							                        transferDate = @transferDate, 
							                        arrivalDate = @arrivalDate, 
							                        arrivalDate2 = @arrivalDate2, 
							                        whTransStatusId = @whTransStatusId, 
							                        modifiedBy = @modifiedBy, 
							                        remarks = @remarks where 
							                        whTransferId = @whTransferId";

            int res = await db.ExecuteAsync(strQry, whTransfer);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildWhTransAuditMOD(whTransfer, tranType);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteWhTransfer(string whTransferId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from WhTransfer where 
														whTransferId = @whTransferId";
                var param = new DynamicParameters();
                param.Add("@whTransferId", whTransferId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelWhTransResultCode> CancelWhTransfer(string whTransId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get wh transfer details
                var whTransDetails = await WhTransDetailsRepo.LockWhTransDetails(db, whTransId);

                if (whTransDetails == null || !whTransDetails.Any())
                {
                    return CancelWhTransResultCode.WHTRANSDETAILLOCKFAILED;
                }

                // check if wh transfer details is all in create status
                var mods = whTransDetails.Where(y => y.WhTransLineStatusId != (POLneStatus.CREATED).ToString());
                if (mods.Any())
                {
                    return CancelWhTransResultCode.WHTRANSDETAILSSTATUSALTERED;
                }

                // lock wh transfer header
                var whTransfer = await LockWhTransfer(db, whTransId);
                if (whTransfer == null)
                {
                    return CancelWhTransResultCode.WHTRANSLOCKFAILED;
                }

                // check if wh transfer header is in create status
                if (whTransfer.WhTransStatusId != (POStatus.CREATED).ToString())
                {
                    return CancelWhTransResultCode.WHTRANSFERSTATUSALTERED;
                }

                // update wh transfer status into canceled
                whTransfer.WhTransStatusId = (POStatus.CANCELED).ToString();
                var whTransAltered = await UpdateWhTransfer(db, whTransfer, TranType.RCVTRANS);

                if (!whTransAltered)
                {
                    return CancelWhTransResultCode.WHTRANSSTATUSUPDATEFAILED;
                }

                // update PO details staus
                int alteredDtlCnt = 0;
                foreach (var whTransDetail in whTransDetails)
                {
                    whTransDetail.WhTransLineStatusId = (POLneStatus.CLOSED).ToString();
                    var whTransDtlAltered = await WhTransDetailsRepo.UpdateWhTransDetailMod(db, whTransDetail, TranType.CANCELRCV);

                    if (!whTransDtlAltered)
                    {
                        return CancelWhTransResultCode.WHTRANSDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == whTransDetails.Count())
                {
                    return CancelWhTransResultCode.SUCCESS;
                }
            }

            return CancelWhTransResultCode.FAILED;
        }

        public async Task<CancelWhTransResultCode> ForceCancelWhTransfer(string whTransId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock and get returns details
                var whTransDetails = await WhTransDetailsRepo.LockWhTransDetails(db, whTransId);

                if (whTransDetails == null || !whTransDetails.Any())
                {
                    return CancelWhTransResultCode.WHTRANSDETAILLOCKFAILED;
                }

                // check if wh transfer details is valid for force close process
                var detailsValid = await ChkWhTransDtlsCanFClose(whTransDetails);
                if (!detailsValid)
                {
                    return CancelWhTransResultCode.WHTRANSDETAILSNOTVALID;
                }

                // check wh transfer details if there's no pending putaway task
                var hasPendingPutaway = await PutawayTaskRepoSub.HasPendingPutawayTaskRet(db, whTransId);
                if (hasPendingPutaway)
                {
                    return CancelWhTransResultCode.HASPUTAWAYTASKPENDING;
                }

                // lock wh transfer header
                var whTransfer = await LockWhTransfer(db, whTransId);
                if (whTransfer == null)
                {
                    return CancelWhTransResultCode.WHTRANSLOCKFAILED;
                }

                // check if wh transfer header is in partial receive status
                if (whTransfer.WhTransStatusId != (POStatus.PARTRCV).ToString())
                {
                    return CancelWhTransResultCode.WHTRANSSTATUSNOTVALID;
                }

                // update whw transfer status into forced closed
                whTransfer.WhTransStatusId = (POStatus.FRCCLOSED).ToString();
                var retAltered = await UpdateWhTransfer(db, whTransfer, TranType.RCVTRANS);

                if (!retAltered)
                {
                    return CancelWhTransResultCode.WHTRANSSTATUSUPDATEFAILED;
                }

                // update wh transfer details status
                int alteredDtlCnt = 0;
                foreach (var whTransDetail in whTransDetails)
                {
                    whTransDetail.WhTransLineStatusId = (POLneStatus.FRCCLOSED).ToString();
                    var whTransDtlAltered = await WhTransDetailsRepo.UpdateWhTransDetailMod(db, whTransDetail, TranType.CANCELRCV);

                    if (!whTransDtlAltered)
                    {
                        return CancelWhTransResultCode.WHTRANSDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == whTransDetails.Count())
                {
                    return CancelWhTransResultCode.SUCCESS;
                }
            }

            return CancelWhTransResultCode.FAILED;
        }

        private async Task<bool> ChkWhTransDtlsCanFClose(IEnumerable<WhTransferDetailModel>? whTransDetails)
        {
            return await Task.Run(() => {
                // check if wh transfer contains force close-able details
                var dtlCreateCnt = whTransDetails.Where(x => x.WhTransLineStatusId == (POLneStatus.CREATED).ToString()).Count();
                var dtlPrtRcvCnt = whTransDetails.Where(x => x.WhTransLineStatusId == (POLneStatus.PRTRCV).ToString()).Count();
                var dtlFullRcvCnt = whTransDetails.Where(x => x.WhTransLineStatusId == (POLneStatus.FULLRCV).ToString()).Count();

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
            string strQry = @"select count(whTransferId) from whtransfer where refNumber = @refNum or refNumber2 = @refNum";

            var param = new DynamicParameters();
            param.Add("@refNum", refNum);

            return await db.ExecuteScalarAsync<int>(strQry, param);
        }

        private async Task<int> ReferenceNumExists(IDbConnection db, string refNum, string whTransId)
        {
            string strQry = @"select count(whTransferId) from whtransfer 
                                where (refNumber = @refNum 
                                or refNumber2 = @refNum) 
                                and whTransferId <> @whTransId";

            var param = new DynamicParameters();
            param.Add("@refNum", refNum);
            param.Add("@whTransId", whTransId);

            return await db.ExecuteScalarAsync<int>(strQry, param);
        }

        public async Task<IEnumerable<string>> GetDistinctWhTransFrom()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT DISTINCT(whFrom) whFrom FROM whtransfer;";

                return await db.QueryAsync<string>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<WhTransferModel>> ExportWhTransfer()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "SELECT * FROM whtransfer";
                return await db.QueryAsync<WhTransferModel>(strQry, commandType: CommandType.Text);
            }
        }
    }
}
