using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Repositories.Sub;
using aims_api.Utilities.Interface;
using Dapper;
using ExcelDataReader;
using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
            return await Task.Run(() =>
            {
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

        public async Task<IEnumerable<WhTransferModel>> GetExportWhTransfer()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "SELECT * FROM whtransfer";
                return await db.QueryAsync<WhTransferModel>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<WhTransCreateTranResult> CreateBulkWhTransfer(IFormFile file, string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream); //icopy sa path yung inupload
            }

            List<WhTransferModelMod> Parameters = new List<WhTransferModelMod>();

            if (file.FileName.ToLower().Contains(".csv"))
            {
                using (var reader = new StreamReader(path))
                {
                    // Read the header line from the CSV
                    string? headerLine = await reader.ReadLineAsync();

                    // Validate the header
                    if (!ValidateCsvHeader(headerLine))
                    {
                        return new WhTransCreateTranResult()
                        {
                            ResultCode = WhTransferTranResultCode.INVALIDHEADER
                        };
                    }
                }

                DataTable value = new DataTable();
                //Install Library : LumenWorksCsvReader 

                using (var csvReader = new CsvReader(new StreamReader(File.OpenRead(path)), true))
                {
                    value.Load(csvReader);
                };

                for (int i = 0; i < value.Rows.Count; i++)
                {
                    WhTransferModelMod rows = new WhTransferModelMod();
                    rows.whTransferHeader = new WhTransferModel();
                    rows.WhTransDetails = new List<WhTransferDetailModel>();
                    WhTransferDetailModel detail = new WhTransferDetailModel();

                    // get PO id number
                    var whTransId = await IdNumberRepo.GetNextIdNum("RCVTRANS");
                    rows.whTransferHeader.WhTransferId = whTransId;

                    rows.whTransferHeader.RefNumber = value.Rows[i][0] != null ? Convert.ToString(value.Rows[i][0]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.RefNumber))
                    {
                        return new WhTransCreateTranResult()
                        {
                            ResultCode = WhTransferTranResultCode.MISSINGREFNUMONE
                        };
                    }

                    rows.whTransferHeader.RefNumber2 = value.Rows[i][1] != null ? Convert.ToString(value.Rows[i][1]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.RefNumber2))
                    {
                        rows.whTransferHeader.RefNumber2 = null;
                    }

                    string? transferDateValue = value.Rows[i][2]?.ToString();
                    if (string.IsNullOrEmpty(transferDateValue))
                    {
                        return new WhTransCreateTranResult()
                        {
                            ResultCode = WhTransferTranResultCode.TRANSFERDATEISREQUIRED
                        };
                    }
                    else
                    {
                        DateTime transferDate;
                        if (!DateTime.TryParseExact(transferDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out transferDate))
                        {
                            transferDate = DateTime.MinValue;
                        }
                        rows.whTransferHeader.TransferDate = transferDate;
                    }

                    string? arrivalDateValue = value.Rows[i][3]?.ToString();
                    if (string.IsNullOrEmpty(arrivalDateValue))
                    {
                        rows.whTransferHeader.ArrivalDate = null;
                    }
                    else
                    {
                        DateTime arrivalDate;
                        if (!DateTime.TryParseExact(arrivalDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate))
                        {
                            arrivalDate = DateTime.MinValue;
                        }
                        rows.whTransferHeader.ArrivalDate = arrivalDate;
                    }

                    // check if expected arrival date is valid
                    if (rows.whTransferHeader.ArrivalDate != null)
                    {
                        if (rows.whTransferHeader.TransferDate > rows.whTransferHeader.ArrivalDate)
                        {
                            return new WhTransCreateTranResult()
                            {
                                ResultCode = WhTransferTranResultCode.INVALIDTRANSFERDATE
                            };
                        }
                    }

                    string? arrivalDate2Value = value.Rows[i][4]?.ToString();
                    if (string.IsNullOrEmpty(arrivalDate2Value))
                    {
                        rows.whTransferHeader.ArrivalDate2 = null;
                    }
                    else
                    {
                        DateTime arrivalDate2;
                        if (!DateTime.TryParseExact(arrivalDate2Value, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate2))
                        {
                            arrivalDate2 = DateTime.MinValue;
                        }
                        rows.whTransferHeader.ArrivalDate2 = arrivalDate2;
                    }

                    //check if expected arrival2 date is valid
                    if (rows.whTransferHeader.ArrivalDate2 != null)
                    {
                        if (rows.whTransferHeader.TransferDate > rows.whTransferHeader.ArrivalDate2)
                        {
                            return new WhTransCreateTranResult()
                            {
                                ResultCode = WhTransferTranResultCode.INVALIDTRANSFERDATE
                            };
                        }
                    }

                    rows.whTransferHeader.Remarks = value.Rows[i][7] != null ? Convert.ToString(value.Rows[i][7]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.Remarks))
                    {
                        rows.whTransferHeader.Remarks = null;
                    }
                    rows.whTransferHeader.WhFromId = value.Rows[i][8] != null ? Convert.ToString(value.Rows[i][8]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.WhFromId))
                    {
                        rows.whTransferHeader.WhFromId = null;
                    }
                    rows.whTransferHeader.WhFrom = value.Rows[i][9] != null ? Convert.ToString(value.Rows[i][9]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.WhFrom))
                    {
                        rows.whTransferHeader.WhFrom = null;
                    }
                    rows.whTransferHeader.WhFromAddress = value.Rows[i][10] != null ? Convert.ToString(value.Rows[i][10]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.WhFromAddress))
                    {
                        rows.whTransferHeader.WhFromAddress = null;
                    }
                    rows.whTransferHeader.WhFromContact = value.Rows[i][11] != null ? Convert.ToString(value.Rows[i][11]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.WhFromContact))
                    {
                        rows.whTransferHeader.WhFromContact = null;
                    }
                    rows.whTransferHeader.WhFromEmail = value.Rows[i][12] != null ? Convert.ToString(value.Rows[i][12]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.WhFromEmail))
                    {
                        rows.whTransferHeader.WhFromEmail = null;
                    }
                    rows.whTransferHeader.CarrierId = value.Rows[i][13] != null ? Convert.ToString(value.Rows[i][13]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.CarrierId))
                    {
                        rows.whTransferHeader.CarrierId = null;
                    }
                    rows.whTransferHeader.CarrierName = value.Rows[i][14] != null ? Convert.ToString(value.Rows[i][14]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.CarrierName))
                    {
                        rows.whTransferHeader.CarrierName = null;
                    }
                    rows.whTransferHeader.CarrierAddress = value.Rows[i][15] != null ? Convert.ToString(value.Rows[i][15]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.CarrierAddress))
                    {
                        rows.whTransferHeader.CarrierAddress = null;
                    }
                    rows.whTransferHeader.CarrierContact = value.Rows[i][16] != null ? Convert.ToString(value.Rows[i][16]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.CarrierContact))
                    {
                        rows.whTransferHeader.CarrierContact = null;
                    }
                    rows.whTransferHeader.CarrierEmail = value.Rows[i][17] != null ? Convert.ToString(value.Rows[i][17]) : null;
                    if (string.IsNullOrEmpty(rows.whTransferHeader.CarrierEmail))
                    {
                        rows.whTransferHeader.CarrierEmail = null;
                    }
                    DateTime currentDateTime = DateTime.Now;
                    rows.whTransferHeader.WhTransStatusId = WhTransferStatus.CREATED.ToString();
                    rows.whTransferHeader.WhTransferStatus = "Created";
                    rows.whTransferHeader.DateCreated = currentDateTime;
                    rows.whTransferHeader.DateModified = currentDateTime;
                    rows.whTransferHeader.CreatedBy = value.Rows[i][18] != null ? Convert.ToString(value.Rows[i][18]) : null;
                    rows.whTransferHeader.ModifiedBy = value.Rows[i][19] != null ? Convert.ToString(value.Rows[i][19]) : null;

                    // Populate PODetailModel
                    detail.Sku = value.Rows[i][5] != null ? Convert.ToString(value.Rows[i][5]) : null;
                    detail.ExpectedQty = Convert.ToInt32(value.Rows[i][6]);
                    detail.DateCreated = currentDateTime;
                    detail.DateModified = currentDateTime;
                    detail.CreatedBy = value.Rows[i][18] != null ? Convert.ToString(value.Rows[i][18]) : null;
                    detail.ModifiedBy = value.Rows[i][19] != null ? Convert.ToString(value.Rows[i][19]) : null;
                    detail.Remarks = value.Rows[i][7] != null ? Convert.ToString(value.Rows[i][7]) : null;

                    // Add the detail to the PODetails collection
                    ((List<WhTransferDetailModel>)rows.WhTransDetails).Add(detail);

                    Parameters.Add(rows);
                }

                if (Parameters.Count > 0)
                {
                    using (IDbConnection db = new MySqlConnection(ConnString))
                    {
                        db.Open();

                        foreach (WhTransferModelMod rows in Parameters)
                        {
                            if (rows.whTransferHeader != null)
                            {
                                var parameters = new
                                {
                                    whTransId = rows.whTransferHeader.WhTransferId,
                                    refNumber = rows.whTransferHeader.RefNumber,
                                    refNumber2 = rows.whTransferHeader.RefNumber2,
                                    transferDate = rows.whTransferHeader.TransferDate,
                                    arrivalDate = rows.whTransferHeader.ArrivalDate,
                                    arrivalDate2 = rows.whTransferHeader.ArrivalDate2,
                                    remarks = rows.whTransferHeader.Remarks,
                                    whFromId = rows.whTransferHeader.WhFromId,
                                    whFrom = rows.whTransferHeader.WhFrom,
                                    WhFromAddress = rows.whTransferHeader.WhFromAddress,
                                    whFromContact = rows.whTransferHeader.WhFromContact,
                                    whFromEmail = rows.whTransferHeader.WhFromEmail,
                                    carrierId = rows.whTransferHeader.CarrierId,
                                    carrierName = rows.whTransferHeader.CarrierName,
                                    carrierAddress = rows.whTransferHeader.CarrierAddress,
                                    carrierContact = rows.whTransferHeader.CarrierContact,
                                    carrierEmail = rows.whTransferHeader.CarrierEmail,
                                    poStatusId = rows.whTransferHeader.WhTransStatusId,
                                    poStatus = rows.whTransferHeader.WhTransferStatus,
                                    dateCreated = rows.whTransferHeader.DateCreated,
                                    dateModified = rows.whTransferHeader.DateModified,
                                    createdBy = rows.whTransferHeader.CreatedBy,
                                    modifyBy = rows.whTransferHeader.ModifiedBy
                                };

                                // check if WhTransfer primary reference number are unique
                                if (!string.IsNullOrEmpty(rows.whTransferHeader.RefNumber))
                                {
                                    var whTransCount = await ReferenceNumExists(db, rows.whTransferHeader.RefNumber);
                                    if (whTransCount > 0)
                                    {
                                        return new WhTransCreateTranResult()
                                        {
                                            ResultCode = WhTransferTranResultCode.INVALIDREFNUMONE
                                        };
                                    }
                                }

                                // check if WhTransfer secondary reference number are unique
                                if (!string.IsNullOrEmpty(rows.whTransferHeader.RefNumber2))
                                {
                                    var whTransCount = await ReferenceNumExists(db, rows.whTransferHeader.RefNumber2);
                                    if (whTransCount > 0)
                                    {
                                        return new WhTransCreateTranResult()
                                        {
                                            ResultCode = WhTransferTranResultCode.INVALIDREFNUMTWO
                                        };
                                    }
                                }

                                // create header
                                var headCreated = await CreateWhTransfer(db, rows.whTransferHeader);

                                if (headCreated)
                                {
                                    // init po user fields default data
                                    var initPOUFld = await WhTransUFieldRepo.InitWhTransferUField(db, rows.whTransferHeader.WhTransferId);
                                    if (!initPOUFld)
                                    {
                                        return new WhTransCreateTranResult()
                                        {
                                            ResultCode = WhTransferTranResultCode.USRFIELDSAVEFAILED
                                        };
                                    }

                                    // insert po user fields values
                                    if (rows.WhTransferUfields != null)
                                    {
                                        var uFieldsCreated = await WhTransUFieldRepo.UpdateWhTransferUField(db, rows.whTransferHeader.WhTransferId, rows.whTransferHeader.CreatedBy, rows.WhTransferUfields);
                                        if (!uFieldsCreated)
                                        {
                                            return new WhTransCreateTranResult()
                                            {
                                                ResultCode = WhTransferTranResultCode.USRFIELDSAVEFAILED
                                            };
                                        }
                                    }

                                    // create detail
                                    if (rows.WhTransDetails.Any())
                                    {
                                        var details = rows.WhTransDetails.ToList();

                                        for (int i = 0; i < details.Count(); i++)
                                        {
                                            var detail = details[i];

                                            // check if similar SKU exists under this PO
                                            var skuExists = await SKUExistsInWhTransfer(db, detail.Sku, rows.whTransferHeader.WhTransferId);
                                            if (skuExists)
                                            {
                                                return new WhTransCreateTranResult()
                                                {
                                                    ResultCode = WhTransferTranResultCode.SKUCONFLICT
                                                };
                                            }

                                            // set detail id, status and header po id
                                            detail.WhTransferLineId = $"{rows.whTransferHeader.WhTransferId}-{i + 1}";
                                            detail.WhTransLineStatusId = (WhTransferLneStatus.CREATED).ToString();
                                            detail.WhTransferId = rows.whTransferHeader.WhTransferId;

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
                                }
                            }
                        }

                        return new WhTransCreateTranResult()
                        {
                            ResultCode = WhTransferTranResultCode.SUCCESS,
                            WhTransferIds = Parameters.Select(p => p.whTransferHeader?.WhTransferId ?? "").ToArray()
                        };
                    }
                }
            }

            if (file.FileName.ToLower().Contains(".xlsx"))
            {
                DataSet dataSet;

                using (var stream = file.OpenReadStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        // Validate the XLSX header
                        if (await ValidateXlsxHeader(worksheet))
                        {
                            FileStream filestream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                            IExcelDataReader reader = ExcelReaderFactory.CreateReader(filestream);
                            dataSet = reader.AsDataSet(
                                new ExcelDataSetConfiguration()
                                {
                                    UseColumnDataType = false,
                                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                    {
                                        UseHeaderRow = true
                                    }

                                });

                            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                            {
                                WhTransferModelMod rows = new WhTransferModelMod();
                                rows.whTransferHeader = new WhTransferModel();
                                rows.WhTransDetails = new List<WhTransferDetailModel>();
                                WhTransferDetailModel detail = new WhTransferDetailModel();

                                // get PO id number
                                var whTransId = await IdNumberRepo.GetNextIdNum("RCVTRANS");
                                rows.whTransferHeader.WhTransferId = whTransId;

                                rows.whTransferHeader.RefNumber = dataSet.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[0]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.RefNumber))
                                {
                                    return new WhTransCreateTranResult()
                                    {
                                        ResultCode = WhTransferTranResultCode.MISSINGREFNUMONE
                                    };
                                }

                                rows.whTransferHeader.RefNumber2 = dataSet.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[1]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.RefNumber2))
                                {
                                    rows.whTransferHeader.RefNumber2 = null;
                                }

                                string? transferDateValue = dataSet.Tables[0].Rows[i].ItemArray[2]?.ToString();
                                if (string.IsNullOrEmpty(transferDateValue))
                                {
                                    return new WhTransCreateTranResult()
                                    {
                                        ResultCode = WhTransferTranResultCode.TRANSFERDATEISREQUIRED
                                    };
                                }
                                else
                                {
                                    DateTime transferDate;
                                    if (!DateTime.TryParseExact(transferDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out transferDate))
                                    {
                                        transferDate = DateTime.MinValue;
                                    }
                                    rows.whTransferHeader.TransferDate = transferDate;
                                }

                                string? arrivalDateValue = dataSet.Tables[0].Rows[i].ItemArray[3]?.ToString();
                                if (string.IsNullOrEmpty(arrivalDateValue))
                                {
                                    rows.whTransferHeader.ArrivalDate = null;
                                }
                                else
                                {
                                    DateTime arrivalDate;
                                    if (!DateTime.TryParseExact(arrivalDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate))
                                    {
                                        arrivalDate = DateTime.MinValue;
                                    }
                                    rows.whTransferHeader.ArrivalDate = arrivalDate;
                                }

                                // check if expected arrival date is valid
                                if (rows.whTransferHeader.ArrivalDate != null)
                                {
                                    if (rows.whTransferHeader.TransferDate > rows.whTransferHeader.ArrivalDate)
                                    {
                                        return new WhTransCreateTranResult()
                                        {
                                            ResultCode = WhTransferTranResultCode.INVALIDTRANSFERDATE
                                        };
                                    }
                                }
                                string? arrivalDate2Value = dataSet.Tables[0].Rows[i].ItemArray[4]?.ToString();
                                if (string.IsNullOrEmpty(arrivalDate2Value))
                                {
                                    rows.whTransferHeader.ArrivalDate2 = null;
                                }
                                else
                                {
                                    DateTime arrivalDate2;
                                    if (!DateTime.TryParseExact(arrivalDate2Value, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate2))
                                    {
                                        arrivalDate2 = DateTime.MinValue;
                                    }
                                    rows.whTransferHeader.ArrivalDate2 = arrivalDate2;
                                }

                                //check if expected arrival2 date is valid
                                if (rows.whTransferHeader.ArrivalDate2 != null)
                                {
                                    if (rows.whTransferHeader.TransferDate > rows.whTransferHeader.ArrivalDate2)
                                    {
                                        return new WhTransCreateTranResult()
                                        {
                                            ResultCode = WhTransferTranResultCode.INVALIDTRANSFERDATE
                                        };
                                    }
                                }

                                rows.whTransferHeader.Remarks = dataSet.Tables[0].Rows[i].ItemArray[7] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[7]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.Remarks))
                                {
                                    rows.whTransferHeader.Remarks = null;
                                }
                                rows.whTransferHeader.WhFromId = dataSet.Tables[0].Rows[i].ItemArray[8] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[8]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.WhFromId))
                                {
                                    rows.whTransferHeader.WhFromId = null;
                                }
                                rows.whTransferHeader.WhFrom = dataSet.Tables[0].Rows[i].ItemArray[9] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[9]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.WhFrom))
                                {
                                    rows.whTransferHeader.WhFrom = null;
                                }
                                rows.whTransferHeader.WhFromAddress = dataSet.Tables[0].Rows[i].ItemArray[10] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[10]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.WhFromAddress))
                                {
                                    rows.whTransferHeader.WhFromAddress = null;
                                }
                                rows.whTransferHeader.WhFromContact = dataSet.Tables[0].Rows[i].ItemArray[11] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[11]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.WhFromContact))
                                {
                                    rows.whTransferHeader.WhFromContact = null;
                                }
                                rows.whTransferHeader.WhFromEmail = dataSet.Tables[0].Rows[i].ItemArray[12] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[12]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.WhFromEmail))
                                {
                                    rows.whTransferHeader.WhFromEmail = null;
                                }
                                rows.whTransferHeader.CarrierId = dataSet.Tables[0].Rows[i].ItemArray[13] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[13]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.CarrierId))
                                {
                                    rows.whTransferHeader.CarrierId = null;
                                }
                                rows.whTransferHeader.CarrierName = dataSet.Tables[0].Rows[i].ItemArray[14] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[14]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.CarrierName))
                                {
                                    rows.whTransferHeader.CarrierName = null;
                                }
                                rows.whTransferHeader.CarrierAddress = dataSet.Tables[0].Rows[i].ItemArray[15] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[15]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.CarrierAddress))
                                {
                                    rows.whTransferHeader.CarrierAddress = null;
                                }
                                rows.whTransferHeader.CarrierContact = dataSet.Tables[0].Rows[i].ItemArray[16] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[16]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.CarrierContact))
                                {
                                    rows.whTransferHeader.CarrierContact = null;
                                }
                                rows.whTransferHeader.CarrierEmail = dataSet.Tables[0].Rows[i].ItemArray[17] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[17]) : null;
                                if (string.IsNullOrEmpty(rows.whTransferHeader.CarrierEmail))
                                {
                                    rows.whTransferHeader.CarrierEmail = null;
                                }
                                DateTime currentDateTime = DateTime.Now;
                                rows.whTransferHeader.WhTransStatusId = WhTransferStatus.CREATED.ToString();
                                rows.whTransferHeader.WhTransferStatus = "Created";
                                rows.whTransferHeader.DateCreated = currentDateTime;
                                rows.whTransferHeader.DateModified = currentDateTime;
                                rows.whTransferHeader.CreatedBy = dataSet.Tables[0].Rows[i].ItemArray[18] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[18]) : null;
                                rows.whTransferHeader.ModifiedBy = dataSet.Tables[0].Rows[i].ItemArray[19] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[19]) : null;

                                // Populate PODetailModel
                                detail.Sku = dataSet.Tables[0].Rows[i].ItemArray[5] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[5]) : null;
                                detail.ExpectedQty = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[6]);
                                detail.DateCreated = currentDateTime;
                                detail.DateModified = currentDateTime;
                                detail.CreatedBy = dataSet.Tables[0].Rows[i].ItemArray[18] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[18]) : null;
                                detail.ModifiedBy = dataSet.Tables[0].Rows[i].ItemArray[19] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[19]) : null;
                                detail.Remarks = dataSet.Tables[0].Rows[i].ItemArray[7] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[7]) : null;

                                // Add the detail to the PODetails collection
                                ((List<WhTransferDetailModel>)rows.WhTransDetails).Add(detail);

                                Parameters.Add(rows);
                            }

                            filestream.Close();

                            if (Parameters.Count > 0)
                            {
                                using (IDbConnection db = new MySqlConnection(ConnString))
                                {
                                    db.Open();

                                    foreach (WhTransferModelMod rows in Parameters)
                                    {
                                        if (rows.whTransferHeader != null)
                                        {
                                            var parameters = new
                                            {
                                                whTransId = rows.whTransferHeader.WhTransferId,
                                                refNumber = rows.whTransferHeader.RefNumber,
                                                refNumber2 = rows.whTransferHeader.RefNumber2,
                                                transferDate = rows.whTransferHeader.TransferDate,
                                                arrivalDate = rows.whTransferHeader.ArrivalDate,
                                                arrivalDate2 = rows.whTransferHeader.ArrivalDate2,
                                                remarks = rows.whTransferHeader.Remarks,
                                                whFromId = rows.whTransferHeader.WhFromId,
                                                whFrom = rows.whTransferHeader.WhFrom,
                                                WhFromAddress = rows.whTransferHeader.WhFromAddress,
                                                whFromContact = rows.whTransferHeader.WhFromContact,
                                                whFromEmail = rows.whTransferHeader.WhFromEmail,
                                                carrierId = rows.whTransferHeader.CarrierId,
                                                carrierName = rows.whTransferHeader.CarrierName,
                                                carrierAddress = rows.whTransferHeader.CarrierAddress,
                                                carrierContact = rows.whTransferHeader.CarrierContact,
                                                carrierEmail = rows.whTransferHeader.CarrierEmail,
                                                poStatusId = rows.whTransferHeader.WhTransStatusId,
                                                poStatus = rows.whTransferHeader.WhTransferStatus,
                                                dateCreated = rows.whTransferHeader.DateCreated,
                                                dateModified = rows.whTransferHeader.DateModified,
                                                createdBy = rows.whTransferHeader.CreatedBy,
                                                modifyBy = rows.whTransferHeader.ModifiedBy
                                            };

                                            // check if WhTransfer primary reference number are unique
                                            if (!string.IsNullOrEmpty(rows.whTransferHeader.RefNumber))
                                            {
                                                var poCount = await ReferenceNumExists(db, rows.whTransferHeader.RefNumber);
                                                if (poCount > 0)
                                                {
                                                    return new WhTransCreateTranResult()
                                                    {
                                                        ResultCode = WhTransferTranResultCode.INVALIDREFNUMONE
                                                    };
                                                }
                                            }

                                            // check if WhTransfer secondary reference number are unique
                                            if (!string.IsNullOrEmpty(rows.whTransferHeader.RefNumber2))
                                            {
                                                var whTransCount = await ReferenceNumExists(db, rows.whTransferHeader.RefNumber2);
                                                if (whTransCount > 0)
                                                {
                                                    return new WhTransCreateTranResult()
                                                    {
                                                        ResultCode = WhTransferTranResultCode.INVALIDREFNUMTWO
                                                    };
                                                }
                                            }

                                            // create header
                                            var headCreated = await CreateWhTransfer(db, rows.whTransferHeader);

                                            if (headCreated)
                                            {
                                                // init po user fields default data
                                                var initPOUFld = await WhTransUFieldRepo.InitWhTransferUField(db, rows.whTransferHeader.WhTransferId);
                                                if (!initPOUFld)
                                                {
                                                    return new WhTransCreateTranResult()
                                                    {
                                                        ResultCode = WhTransferTranResultCode.USRFIELDSAVEFAILED
                                                    };
                                                }

                                                // insert po user fields values
                                                if (rows.WhTransferUfields != null)
                                                {
                                                    var uFieldsCreated = await WhTransUFieldRepo.UpdateWhTransferUField(db, rows.whTransferHeader.WhTransferId, rows.whTransferHeader.CreatedBy, rows.WhTransferUfields);
                                                    if (!uFieldsCreated)
                                                    {
                                                        return new WhTransCreateTranResult()
                                                        {
                                                            ResultCode = WhTransferTranResultCode.USRFIELDSAVEFAILED
                                                        };
                                                    }
                                                }

                                                // create detail
                                                if (rows.WhTransDetails.Any())
                                                {
                                                    var details = rows.WhTransDetails.ToList();

                                                    for (int i = 0; i < details.Count(); i++)
                                                    {
                                                        var detail = details[i];

                                                        // check if similar SKU exists under this PO
                                                        var skuExists = await SKUExistsInWhTransfer(db, detail.Sku, rows.whTransferHeader.WhTransferId);
                                                        if (skuExists)
                                                        {
                                                            return new WhTransCreateTranResult()
                                                            {
                                                                ResultCode = WhTransferTranResultCode.SKUCONFLICT
                                                            };
                                                        }

                                                        // set detail id, status and header po id
                                                        detail.WhTransferLineId = $"{rows.whTransferHeader.WhTransferId}-{i + 1}";
                                                        detail.WhTransLineStatusId = (WhTransferLneStatus.CREATED).ToString();
                                                        detail.WhTransferId = rows.whTransferHeader.WhTransferId;

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
                                            }
                                        }
                                    }

                                    return new WhTransCreateTranResult()
                                    {
                                        ResultCode = WhTransferTranResultCode.SUCCESS,
                                        WhTransferIds = Parameters.Select(p => p.whTransferHeader?.WhTransferId ?? "").ToArray()
                                    };
                                }
                            }
                        }
                        else
                        {
                            return new WhTransCreateTranResult()
                            {
                                ResultCode = WhTransferTranResultCode.INVALIDHEADER
                            };
                        }
                    }
                }
            }

            return new WhTransCreateTranResult()
            {
                ResultCode = WhTransferTranResultCode.FAILED
            };
        }

        public bool ValidateCsvHeader(string headerLine)
        {
            string[] expectedHeaders = { "Reference Number", "2nd Reference Number", "Transfer Date",
                                         "Arrival Date", "Arrival Date 2", "SKU",
                                         "Expected Qty", "Remarks", "Warehouse From Id",
                                         "Warehouse From", "Warehouse From Address", "Warehouse From Contact",
                                         "Warehouse From Email", "Carrier Id", "Carrier Name",
                                         "Carrier Address", "Carrier Contact", "Carrier Email",
                                         "Created By", "Modified By"
                                       };

            string[] actualHeaders = headerLine.Split(',');

            if (actualHeaders.Length != expectedHeaders.Length)
            {
                return false;
            }

            for (int i = 0; i < actualHeaders.Length; i++)
            {
                if (actualHeaders[i].Trim() != expectedHeaders[i])
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> ValidateXlsxHeader(ExcelWorksheet worksheet)
        {
            string[] expectedHeaders = { "Reference Number", "2nd Reference Number", "Transfer Date",
                                         "Arrival Date", "Arrival Date 2", "SKU",
                                         "Expected Qty", "Remarks", "Warehouse From Id",
                                         "Warehouse From", "Warehouse From Address", "Warehouse From Contact",
                                         "Warehouse From Email", "Carrier Id", "Carrier Name",
                                         "Carrier Address", "Carrier Contact", "Carrier Email",
                                         "Created By", "Modified By"
                                       };

            int columnCount = await Task.Run(() => worksheet.Dimension.Columns);

            if (columnCount != expectedHeaders.Length)
            {
                return false;
            }

            for (int column = 1; column <= columnCount; column++)
            {
                string? headerCell = await Task.Run(() => worksheet.Cells[1, column].Value?.ToString());

                if (headerCell?.Trim() != expectedHeaders[column - 1])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
