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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class ReturnsRepository : IReturnsRepository
    {
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        IReturnsDetailRepository RetDetailRepo;
        IReturnsUserFieldRepository RetUFieldRepo;
        IAuditTrailRepository AuditTrailRepo;
        PutawayTaskRepoSub PutawayTaskRepoSub;
        ReturnsAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public ReturnsRepository(ITenantProvider tenantProvider,
                            IAuditTrailRepository auditTrailRepo,
                            IIdNumberRepository idNumberRepo,
                            IReturnsDetailRepository retDetailsRepo,
                            PutawayTaskRepoSub putawayTaskRepoSub,
                            IReturnsUserFieldRepository retUFieldRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            RetDetailRepo = retDetailsRepo;
            PutawayTaskRepoSub = putawayTaskRepoSub;
            PagingRepo = new PagingRepository();
            AuditBuilder = new ReturnsAudit();
            RetUFieldRepo = retUFieldRepo;
        }

        public async Task<ReturnsPagedMdl?> GetReturnsPaged(int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                int offset = (pageNum - 1) * pageItem;
                string strQry = @"select returns.*, 
                                            postatus.poStatus returnStatus
                                    from returns 
                                    inner join postatus on returns.returnsStatusId = postatus.poStatusId 
                                    order by returnsId 
                                    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<ReturnsModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetReturnsPageDetail(db, pageNum, pageItem, ret.Count());

                    return new ReturnsPagedMdl()
                    {
                        Pagination = pageDetail,
                        Returns = ret
                    };
                }
            }

            return null;
        }

        private async Task<Pagination?> GetReturnsPageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = "select count(returnsId) from returns";
            return await PagingRepo.GetPageDetail(db, strQry, pageNum, pageItem, rowCount);
        }

        public async Task<ReturnsPagedMdl?> GetReturnsFilteredPaged(ReturnsFilteredMdl filter, int pageNum, int pageItem)
        {
            string strQry = @"select returns.*, 
                                        postatus.poStatus returnStatus
                                from returns";
            string strFltr = " where ";
            var param = new DynamicParameters();

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(returnsId) from returns";

            if (!string.IsNullOrEmpty(filter.StoreId))
            {
                strFltr += $"storeId = @storeId ";
                param.Add("@storeId", filter.StoreId);
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

            if (filter.ReturnDate != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"returnDate = @returnDate ";
                param.Add("@returnDate", filter.ReturnDate);
            }

            if (!string.IsNullOrEmpty(filter.ReturnsStatusId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"returnsStatusId = @returnsStatusId ";
                param.Add("@returnsStatusId", filter.ReturnsStatusId);
            }

            // build inner joins
            string strJoins = @" inner join postatus on returns.returnsStatusId = poStatus.poStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by returnsId";
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

                var ret = await db.QueryAsync<ReturnsModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new ReturnsPagedMdl()
                    {
                        Pagination = pageDetail,
                        Returns = ret
                    };
                }
            }

            return null;
        }

        public async Task<ReturnsPagedMdl?> GetReturnsForRcvPaged(int pageNum, int pageItem)
        {
            string strQry = @"select returns.*, 
                                        postatus.poStatus returnStatus
                                from returns";
            string strFltr = @" where returnsStatusId = @statsCreated or 
                                        returnsStatusId = @statsPartRcv ";

            var param = new DynamicParameters();
            param.Add("@statsCreated", (POStatus.CREATED).ToString());
            param.Add("@statsPartRcv", (POStatus.PARTRCV).ToString());

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(returnsId) from returns";

            // build inner joins
            string strJoins = @" inner join postatus on returns.returnsStatusId = poStatus.poStatusId";

            // build order by and paging
            strQry += strJoins;
            strQry += strFltr + $" order by returnsId desc";
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

                var ret = await db.QueryAsync<ReturnsModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strJoins;
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new ReturnsPagedMdl()
                    {
                        Pagination = pageDetail,
                        Returns = ret
                    };
                }
            }

            return null;
        }

        public async Task<ReturnsPagedMdl?> GetReturnsSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select returns.*, 
                                            postats.poStatus returnStatus 
                                    from returns";

                string strFltr = @" where returnsId like @searchKey or 
							                refNumber like @searchKey or 
							                refNumber2 like @searchKey or 
							                storeId like @searchKey or 
							                storeFrom like @searchKey or 
							                storeAddress like @searchKey or 
							                storeContact like @searchKey or 
							                storeEmail like @searchKey or 
										    carrierId like @searchKey or 
										    carrierName like @searchKey or 
										    carrierAddress like @searchKey or 
										    carrierContact like @searchKey or 
										    carrierEmail like @searchKey or 
										    returnDate like @searchKey or 
							                arrivalDate like @searchKey or 
							                arrivalDate2 like @searchKey or 
							                returnsStatusId like @searchKey or 
							                dateCreated like @searchKey or 
							                dateModified like @searchKey or 
							                createdBy like @searchKey or 
							                modifiedBy like @searchKey or 
							                remarks like @searchKey or
                                            postats.poStatus like @searchKey";

                // build inner joins
                string strJoins = @" inner join postatus postats on returns.returnsStatusId = poStats.poStatusId";

                strQry += $"{strJoins}{strFltr} order by returnsId limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<ReturnsModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $"select count(returnsId) from returns {strJoins}{strFltr}";
                    var pgParam = new DynamicParameters();
                    pgParam.Add("@searchKey", $"%{searchKey}%");

                    var pageDeatil = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new ReturnsPagedMdl()
                    {
                        Pagination = pageDeatil,
                        Returns = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<ReturnsModel>> GetReturnsPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Returns where 
														returnsId like @searchKey or 
														refNumber like @searchKey or 
														refNumber2 like @searchKey or 
														storeFrom like @searchKey or 
														storeAddress like @searchKey or 
														storeContact like @searchKey or 
														storeEmail like @searchKey or 
														arrivalDate like @searchKey or 
														arrivalDate2 like @searchKey or 
														returnsStatusId like @searchKey or 
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
                return await db.QueryAsync<ReturnsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ReturnsModel> GetReturnsById(string returnsId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT ret.*, 
                                            stats.poStatus returnStatus 
                                    FROM `returns` ret
                                    INNER JOIN postatus stats 
	                                    ON ret.returnsStatusId = stats.poStatusId 
                                    WHERE ret.returnsId = @retId;";

                var param = new DynamicParameters();
                param.Add("@retId", returnsId);
                return await db.QuerySingleOrDefaultAsync<ReturnsModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> ReturnsExists(string returnsId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select returnsId from Returns where 
														returnsId = @returnsId";

                var param = new DynamicParameters();
                param.Add("@returnsId", returnsId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> ReturnsReceivable(string returnsId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select count(returnsId) from returns where 
                                                        (returnsStatusId = 'CREATED' or 
                                                        returnsStatusId = 'PARTRCV') and 
														returnsId = @returnsId";

                var param = new DynamicParameters();
                param.Add("@returnsId", returnsId);

                var res = await db.ExecuteScalarAsync<bool>(strQry, param);
                if (res)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<ReturnsModel> LockReturns(IDbConnection db, string returnsId)
        {
            string strQry = @"SELECT * FROM returns WHERE returnsId = @returnsId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@returnsId", returnsId);

            return await db.QuerySingleOrDefaultAsync<ReturnsModel>(strQry, param);
        }

        public async Task<string?> GetReturnsUpdatedStatus(IDbConnection db, string returnsId)
        {
            string strQry = @"call `spGetReturnsUpdatedStatus`(@paramReturnsId);";

            var param = new DynamicParameters();
            param.Add("@paramReturnsId", returnsId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        public async Task<ReturnsCreateTranResult> CreateReturnsMod(ReturnsModelMod returns)
        {
            // get returns id number
            var returnsId = await IdNumberRepo.GetNextIdNum("RCVRET");

            if (!string.IsNullOrEmpty(returnsId) &&
                returns.ReturnsHeader != null &&
                returns.ReturnsDetails != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // check if returns primary reference number are unique
                    if (!string.IsNullOrEmpty(returns.ReturnsHeader.RefNumber))
                    {
                        var poCount = await ReferenceNumExists(db, returns.ReturnsHeader.RefNumber);
                        if (poCount > 0)
                        {
                            return new ReturnsCreateTranResult()
                            {
                                ResultCode = ReturnsTranResultCode.INVALIDREFNUMONE
                            };
                        }
                    }

                    // check if returns secondary reference number are unique
                    if (!string.IsNullOrEmpty(returns.ReturnsHeader.RefNumber2))
                    {
                        var poCount = await ReferenceNumExists(db, returns.ReturnsHeader.RefNumber2);
                        if (poCount > 0)
                        {
                            return new ReturnsCreateTranResult()
                            {
                                ResultCode = ReturnsTranResultCode.INVALIDREFNUMTWO
                            };
                        }
                    }

                    // create header
                    returns.ReturnsHeader.ReturnsId = returnsId;
                    var headCreated = await CreateReturns(db, returns.ReturnsHeader);

                    if (headCreated)
                    {
                        // init returns user fields default data
                        var initPOUFld = await RetUFieldRepo.InitReturnsUField(db, returnsId);
                        if (!initPOUFld)
                        {
                            return new ReturnsCreateTranResult()
                            {
                                ResultCode = ReturnsTranResultCode.USRFIELDSAVEFAILED
                            };
                        }

                        // insert returns user fields values
                        if (returns.ReturnsUfields != null)
                        {
                            var uFieldsCreated = await RetUFieldRepo.UpdateReturnsUField(db, returnsId, returns.ReturnsHeader.CreatedBy, returns.ReturnsUfields);
                            if (!uFieldsCreated)
                            {
                                return new ReturnsCreateTranResult()
                                {
                                    ResultCode = ReturnsTranResultCode.USRFIELDSAVEFAILED
                                };
                            }
                        }

                        // create detail
                        if (returns.ReturnsDetails.Any())
                        {
                            var details = returns.ReturnsDetails.ToList();

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];

                                // check if similar SKU exists under this returns
                                var skuExists = await SKUExistsInReturns(db, detail.Sku, returnsId);
                                if (skuExists)
                                {
                                    return new ReturnsCreateTranResult()
                                    {
                                        ResultCode = ReturnsTranResultCode.SKUCONFLICT
                                    };
                                }

                                // set detail id, status and header returns id
                                detail.ReturnsLineId = $"{returnsId}-{i + 1}";
                                detail.ReturnsLineStatusId = (POLneStatus.CREATED).ToString();
                                detail.ReturnsId = returnsId;

                                // create detail
                                bool dtlSaved = await RetDetailRepo.CreateReturnsDetailMod(db, detail);

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return new ReturnsCreateTranResult()
                                    {
                                        ResultCode = ReturnsTranResultCode.RETLINESAVEFAILED
                                    };
                                }
                            }
                        }

                        return new ReturnsCreateTranResult()
                        {
                            ResultCode = ReturnsTranResultCode.SUCCESS,
                            ReturnsId = returnsId
                        };
                    }
                }
            }

            return new ReturnsCreateTranResult()
            {
                ResultCode = ReturnsTranResultCode.FAILED
            };
        }

        private async Task<bool> SKUExistsInReturns(IDbConnection db, string sku, string returnsId)
        {
            string strQry = @"select count(sku) from returnsdetail 
                                where sku = @sku and 
                                        returnsId = @returnsId";

            var param = new DynamicParameters();
            param.Add("@sku", sku);
            param.Add("@returnsId", returnsId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);
            if (res == 0)
            {
                return false;
            }

            // default true to ensure no conflict will occur on error
            return true;
        }

        public async Task<bool> CreateReturns(IDbConnection db, ReturnsModel returns)
        {
            // define returns status
            returns.ReturnsStatusId = (POStatus.CREATED).ToString();

            string strQry = @"insert into Returns(returnsId, 
														refNumber, 
														refNumber2, 
                                                        storeId, 
														storeFrom, 
														storeAddress, 
														storeContact, 
														storeEmail, 
														carrierId, 
														carrierName, 
														carrierAddress, 
														carrierContact, 
														carrierEmail, 
														returnDate, 
														arrivalDate, 
														arrivalDate2, 
														returnsStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@returnsId, 
														@refNumber, 
														@refNumber2, 
                                                        @storeId, 
														@storeFrom, 
														@storeAddress, 
														@storeContact, 
														@storeEmail, 
														@carrierId, 
														@carrierName, 
														@carrierAddress, 
														@carrierContact, 
														@carrierEmail, 
														@returnDate, 
														@arrivalDate, 
														@arrivalDate2, 
														@returnsStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, returns);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditADD(returns, TranType.RCVRET);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<ReturnsTranResultCode> UpdateReturnsMod(ReturnsModelMod returns)
        {
            if (returns.ReturnsHeader != null &&
                returns.ReturnsDetails != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // check if returns primary reference number are unique
                    if (!string.IsNullOrEmpty(returns.ReturnsHeader.RefNumber))
                    {
                        var returnsCount = await ReferenceNumExists(db, returns.ReturnsHeader.RefNumber, returns.ReturnsHeader.ReturnsId);
                        if (returnsCount > 0)
                        {
                            return ReturnsTranResultCode.INVALIDREFNUMONE;
                        }
                    }

                    // check if returns secondary reference number are unique
                    if (!string.IsNullOrEmpty(returns.ReturnsHeader.RefNumber2))
                    {
                        var returnsCount = await ReferenceNumExists(db, returns.ReturnsHeader.RefNumber2, returns.ReturnsHeader.ReturnsId);
                        if (returnsCount > 0)
                        {
                            return ReturnsTranResultCode.INVALIDREFNUMTWO;
                        }
                    }

                    // update header
                    var modHeader = await UpdateReturns(db, returns.ReturnsHeader, TranType.RCVRET);

                    if (modHeader)
                    {
                        // update returns user fields values
                        if (returns.ReturnsUfields != null)
                        {
                            var uFieldsCreated = await RetUFieldRepo.UpdateReturnsUFieldMOD(db, returns.ReturnsHeader.ReturnsId, returns.ReturnsHeader.ModifiedBy, returns.ReturnsUfields);
                            if (!uFieldsCreated)
                            {
                                return ReturnsTranResultCode.USRFIELDSAVEFAILED;
                            }
                        }

                        // update detail
                        if (returns.ReturnsDetails != null && returns.ReturnsDetails.Any())
                        {
                            var details = returns.ReturnsDetails.ToList();

                            // get last returns detail line number
                            var retDetailsFromDb = await RetDetailRepo.LockReturnsDetails(db, returns.ReturnsHeader.ReturnsId);
                            var lastRetLneId = retDetailsFromDb.OrderByDescending(x => x.ReturnsLineId).Select(y => y.ReturnsLineId).FirstOrDefault();
                            int lastLneNum = 0;

                            if (!string.IsNullOrEmpty(lastRetLneId))
                            {
                                lastLneNum = Convert.ToInt32(lastRetLneId.Substring(lastRetLneId.LastIndexOf('-') + 1));
                            }
                            else
                            {
                                lastLneNum = 0;
                            }

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];
                                bool dtlSaved = false;

                                if (detail.ReturnsLineId == null)
                                {
                                    // check if similar SKU exists under this returns
                                    var skuExists = await SKUExistsInReturns(db, detail.Sku, returns.ReturnsHeader.ReturnsId);
                                    if (skuExists)
                                    {
                                        return ReturnsTranResultCode.SKUCONFLICT;
                                    }

                                    // detail concidered as new
                                    // set detail id, status and header returns id
                                    lastLneNum += 1;
                                    detail.ReturnsLineId = $"{returns.ReturnsHeader.ReturnsId}-{lastLneNum}";
                                    detail.ReturnsLineStatusId = (POLneStatus.CREATED).ToString();
                                    detail.ReturnsId = returns.ReturnsHeader.ReturnsId;

                                    // create detail
                                    dtlSaved = await RetDetailRepo.CreateReturnsDetailMod(db, detail);
                                }
                                else
                                {
                                    // update existing details
                                    var prevDetail = await RetDetailRepo.GetReturnsDetailByIdMod(db, detail.ReturnsLineId);

                                    if (prevDetail.ReturnsLineStatusId == (POLneStatus.CREATED).ToString())
                                    {
                                        if (prevDetail != detail)
                                        {
                                            dtlSaved = await RetDetailRepo.UpdateReturnsDetailMod(db, detail, TranType.RCVRET);
                                        }
                                    }
                                }

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return ReturnsTranResultCode.RETLINESAVEFAILED;
                                }
                            }
                        }

                        return ReturnsTranResultCode.SUCCESS;
                    }
                }
            }

            return ReturnsTranResultCode.FAILED;
        }

        public async Task<bool> UpdateReturns(IDbConnection db, ReturnsModel returns, TranType tranTyp)
        {
            string strQry = @"update Returns set 
									            refNumber = @refNumber, 
									            refNumber2 = @refNumber2, 
									            storeId = @storeId, 
									            storeFrom = @storeFrom, 
									            storeAddress = @storeAddress, 
									            storeContact = @storeContact, 
									            storeEmail = @storeEmail, 
							                    carrierId = @carrierId, 
							                    carrierName = @carrierName, 
							                    carrierAddress = @carrierAddress, 
							                    carrierContact = @carrierContact, 
							                    carrierEmail = @carrierEmail, 
							                    returnDate = @returnDate, 
									            arrivalDate = @arrivalDate, 
									            arrivalDate2 = @arrivalDate2, 
									            returnsStatusId = @returnsStatusId, 
									            modifiedBy = @modifiedBy, 
									            remarks = @remarks where 
									            returnsId = @returnsId";

            int res = await db.ExecuteAsync(strQry, returns);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditMOD(returns, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteReturns(string returnsId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"delete from Returns where 
														returnsId = @returnsId";
                var param = new DynamicParameters();
                param.Add("@returnsId", returnsId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelRetResultCode> CancelReturns(string returnsId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                // lock and get returns details
                var retDetails = await RetDetailRepo.LockReturnsDetails(db, returnsId);

                if (retDetails == null || !retDetails.Any())
                {
                    return CancelRetResultCode.RETDETAILLOCKFAILED;
                }

                // check if returns details is all in create status
                var mods = retDetails.Where(y => y.ReturnsLineStatusId != (POLneStatus.CREATED).ToString());
                if (mods.Any())
                {
                    return CancelRetResultCode.RETDETAILSSTATUSALTERED;
                }

                // lock Preturns header
                var returns = await LockReturns(db, returnsId);
                if (returns == null)
                {
                    return CancelRetResultCode.RETLOCKFAILED;
                }

                // check if PO header is in create status
                if (returns.ReturnsStatusId != (POStatus.CREATED).ToString())
                {
                    return CancelRetResultCode.RETSTATUSALTERED;
                }

                // update PO status into canceled
                returns.ReturnsStatusId = (POStatus.CANCELED).ToString();
                var retAltered = await UpdateReturns(db, returns, TranType.RCVRET);

                if (!retAltered)
                {
                    return CancelRetResultCode.RETSTATUSUPDATEFAILED;
                }

                // update PO details staus
                int alteredDtlCnt = 0;
                foreach (var retDetail in retDetails)
                {
                    retDetail.ReturnsLineStatusId = (POLneStatus.CLOSED).ToString();
                    var retDtlAltered = await RetDetailRepo.UpdateReturnsDetailMod(db, retDetail, TranType.CANCELRCV);

                    if (!retDtlAltered)
                    {
                        return CancelRetResultCode.RETDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == retDetails.Count())
                {
                    tran.Commit();
                    return CancelRetResultCode.SUCCESS;
                }
            }

            return CancelRetResultCode.FAILED;
        }

        public async Task<CancelRetResultCode> ForceCancelReturns(string returnsId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                // lock and get returns details
                var retDetails = await RetDetailRepo.LockReturnsDetails(db, returnsId);

                if (retDetails == null || !retDetails.Any())
                {
                    return CancelRetResultCode.RETDETAILLOCKFAILED;
                }

                // check if Returns details is valid for force close process
                var detailsValid = await ChkRetDtlsCanFClose(retDetails);
                if (!detailsValid)
                {
                    return CancelRetResultCode.RETDETAILSNOTVALID;
                }

                // check Returns details if there's no pending putaway task
                var hasPendingPutaway = await PutawayTaskRepoSub.HasPendingPutawayTaskRet(db, returnsId);
                if (hasPendingPutaway)
                {
                    return CancelRetResultCode.HASPUTAWAYTASKPENDING;
                }

                // lock Returns header
                var returns = await LockReturns(db, returnsId);
                if (returns == null)
                {
                    return CancelRetResultCode.RETLOCKFAILED;
                }

                // check if Returns header is in partial receive status
                if (returns.ReturnsStatusId != (POStatus.PARTRCV).ToString())
                {
                    return CancelRetResultCode.RETSTATUSNOTVALID;
                }

                // update Returns status into forced closed
                returns.ReturnsStatusId = (POStatus.FRCCLOSED).ToString();
                var retAltered = await UpdateReturns(db, returns, TranType.RCVRET);

                if (!retAltered)
                {
                    return CancelRetResultCode.RETSTATUSUPDATEFAILED;
                }

                // update Returns details status
                int alteredDtlCnt = 0;
                foreach (var retDetail in retDetails)
                {
                    retDetail.ReturnsLineStatusId = (POLneStatus.FRCCLOSED).ToString();
                    var retDtlAltered = await RetDetailRepo.UpdateReturnsDetailMod(db, retDetail, TranType.CANCELRCV);

                    if (!retDtlAltered)
                    {
                        return CancelRetResultCode.RETDETAILSSTATUSUPDATEFAILED;
                    }

                    alteredDtlCnt += 1;
                }

                if (alteredDtlCnt == retDetails.Count())
                {
                    tran.Commit();
                    return CancelRetResultCode.SUCCESS;
                }
            }

            return CancelRetResultCode.FAILED;
        }

        private async Task<bool> ChkRetDtlsCanFClose(IEnumerable<ReturnsDetailModel>? retDetails)
        {
            return await Task.Run(() =>
            {
                // check if PO contains force close-able details
                var dtlCreateCnt = retDetails.Where(x => x.ReturnsLineStatusId == (POLneStatus.CREATED).ToString()).Count();
                var dtlPrtRcvCnt = retDetails.Where(x => x.ReturnsLineStatusId == (POLneStatus.PRTRCV).ToString()).Count();
                var dtlFullRcvCnt = retDetails.Where(x => x.ReturnsLineStatusId == (POLneStatus.FULLRCV).ToString()).Count();

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
            string strQry = @"select count(returnsId) from returns where refNumber = @refNum or refNumber2 = @refNum";

            var param = new DynamicParameters();
            param.Add("@refNum", refNum);

            return await db.ExecuteScalarAsync<int>(strQry, param);
        }

        private async Task<int> ReferenceNumExists(IDbConnection db, string refNum, string returnsId)
        {
            string strQry = @"select count(returnsId) from returns 
                                where (refNumber = @refNum 
                                or refNumber2 = @refNum) 
                                and returnsId <> @returnsId";

            var param = new DynamicParameters();
            param.Add("@refNum", refNum);
            param.Add("@returnsId", returnsId);

            return await db.ExecuteScalarAsync<int>(strQry, param);
        }

        public async Task<IEnumerable<string>> GetDistinctStoreFrom()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT DISTINCT(storeFrom) storeFrom FROM `returns`;";

                return await db.QueryAsync<string>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<ReturnsModel>> GetExportReturnsTransfer()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "SELECT * FROM returns";
                return await db.QueryAsync<ReturnsModel>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<ReturnsCreateTranResult> CreateBulkReturns(IFormFile file, string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream); //icopy sa path yung inupload
            }

            List<ReturnsModelMod> Parameters = new List<ReturnsModelMod>();

            if (file.FileName.ToLower().Contains(".csv"))
            {
                using (var reader = new StreamReader(path))
                {
                    // Read the header line from the CSV
                    string headerLine = await reader.ReadLineAsync();

                    // Validate the header
                    if (!ValidateCsvHeader(headerLine))
                    {
                        return new ReturnsCreateTranResult()
                        {
                            ResultCode = ReturnsTranResultCode.INVALIDHEADER
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
                    ReturnsModelMod rows = new ReturnsModelMod();
                    rows.ReturnsHeader = new ReturnsModel();
                    rows.ReturnsDetails = new List<ReturnsDetailModel>();
                    ReturnsDetailModel detail = new ReturnsDetailModel();

                    // get PO id number
                    var retId = await IdNumberRepo.GetNextIdNum("RCVRET");
                    rows.ReturnsHeader.ReturnsId = retId;

                    rows.ReturnsHeader.RefNumber = value.Rows[i][0] != null ? Convert.ToString(value.Rows[i][0]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.RefNumber))
                    {
                        return new ReturnsCreateTranResult()
                        {
                            ResultCode = ReturnsTranResultCode.MISSINGREFNUMONE
                        };
                    }

                    rows.ReturnsHeader.RefNumber2 = value.Rows[i][1] != null ? Convert.ToString(value.Rows[i][1]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.RefNumber2))
                    {
                        rows.ReturnsHeader.RefNumber2 = null;
                    }

                    string? returnDateValue = value.Rows[i][2]?.ToString();
                    if (string.IsNullOrEmpty(returnDateValue))
                    {
                        return new ReturnsCreateTranResult()
                        {
                            ResultCode = ReturnsTranResultCode.RETURNDATEISREQUIRED
                        };
                    }
                    else
                    {
                        DateTime returnDate;
                        if (!DateTime.TryParseExact(returnDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out returnDate))
                        {
                            returnDate = DateTime.MinValue;
                        }
                        rows.ReturnsHeader.ReturnDate = returnDate;
                    }

                    string? arrivalDateValue = value.Rows[i][3]?.ToString();
                    if (string.IsNullOrEmpty(arrivalDateValue))
                    {
                        rows.ReturnsHeader.ArrivalDate = null;
                    }
                    else
                    {
                        DateTime arrivalDate;
                        if (!DateTime.TryParseExact(arrivalDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate))
                        {
                            arrivalDate = DateTime.MinValue;
                        }
                        rows.ReturnsHeader.ArrivalDate = arrivalDate;
                    }

                    // check if expected arrival date is valid
                    if (rows.ReturnsHeader.ArrivalDate != null)
                    {
                        if (rows.ReturnsHeader.ReturnDate > rows.ReturnsHeader.ArrivalDate)
                        {
                            return new ReturnsCreateTranResult()
                            {
                                ResultCode = ReturnsTranResultCode.INVALIDRETURNDATE
                            };
                        }
                    }

                    string? arrivalDate2Value = value.Rows[i][4]?.ToString();
                    if (string.IsNullOrEmpty(arrivalDate2Value))
                    {
                        rows.ReturnsHeader.ArrivalDate2 = null;
                    }
                    else
                    {
                        DateTime arrivalDate2;
                        if (!DateTime.TryParseExact(arrivalDate2Value, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate2))
                        {
                            arrivalDate2 = DateTime.MinValue;
                        }
                        rows.ReturnsHeader.ArrivalDate2 = arrivalDate2;
                    }

                    //check if expected arrival2 date is valid
                    if (rows.ReturnsHeader.ArrivalDate2 != null)
                    {
                        if (rows.ReturnsHeader.ReturnDate > rows.ReturnsHeader.ArrivalDate2)
                        {
                            return new ReturnsCreateTranResult()
                            {
                                ResultCode = ReturnsTranResultCode.INVALIDRETURNDATE
                            };
                        }
                    }

                    rows.ReturnsHeader.Remarks = value.Rows[i][7] != null ? Convert.ToString(value.Rows[i][7]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.Remarks))
                    {
                        rows.ReturnsHeader.Remarks = null;
                    }
                    rows.ReturnsHeader.StoreId = value.Rows[i][8] != null ? Convert.ToString(value.Rows[i][8]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.StoreId))
                    {
                        rows.ReturnsHeader.StoreId = null;
                    }
                    rows.ReturnsHeader.StoreFrom = value.Rows[i][9] != null ? Convert.ToString(value.Rows[i][9]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.StoreFrom))
                    {
                        rows.ReturnsHeader.StoreFrom = null;
                    }
                    rows.ReturnsHeader.StoreAddress = value.Rows[i][10] != null ? Convert.ToString(value.Rows[i][10]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.StoreAddress))
                    {
                        rows.ReturnsHeader.StoreAddress = null;
                    }
                    rows.ReturnsHeader.StoreContact = value.Rows[i][11] != null ? Convert.ToString(value.Rows[i][11]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.StoreContact))
                    {
                        rows.ReturnsHeader.StoreContact = null;
                    }
                    rows.ReturnsHeader.StoreEmail = value.Rows[i][12] != null ? Convert.ToString(value.Rows[i][12]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.StoreEmail))
                    {
                        rows.ReturnsHeader.StoreEmail = null;
                    }
                    rows.ReturnsHeader.CarrierId = value.Rows[i][13] != null ? Convert.ToString(value.Rows[i][13]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.CarrierId))
                    {
                        rows.ReturnsHeader.CarrierId = null;
                    }
                    rows.ReturnsHeader.CarrierName = value.Rows[i][14] != null ? Convert.ToString(value.Rows[i][14]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.CarrierName))
                    {
                        rows.ReturnsHeader.CarrierName = null;
                    }
                    rows.ReturnsHeader.CarrierAddress = value.Rows[i][15] != null ? Convert.ToString(value.Rows[i][15]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.CarrierAddress))
                    {
                        rows.ReturnsHeader.CarrierAddress = null;
                    }
                    rows.ReturnsHeader.CarrierContact = value.Rows[i][16] != null ? Convert.ToString(value.Rows[i][16]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.CarrierContact))
                    {
                        rows.ReturnsHeader.CarrierContact = null;
                    }
                    rows.ReturnsHeader.CarrierEmail = value.Rows[i][17] != null ? Convert.ToString(value.Rows[i][17]) : null;
                    if (string.IsNullOrEmpty(rows.ReturnsHeader.CarrierEmail))
                    {
                        rows.ReturnsHeader.CarrierEmail = null;
                    }
                    DateTime currentDateTime = DateTime.Now;
                    rows.ReturnsHeader.ReturnsStatusId = ReturnStatus.CREATED.ToString();
                    rows.ReturnsHeader.ReturnStatus = "Created";
                    rows.ReturnsHeader.DateCreated = currentDateTime;
                    rows.ReturnsHeader.DateModified = currentDateTime;
                    rows.ReturnsHeader.CreatedBy = value.Rows[i][18] != null ? Convert.ToString(value.Rows[i][18]) : null;
                    rows.ReturnsHeader.ModifiedBy = value.Rows[i][19] != null ? Convert.ToString(value.Rows[i][19]) : null;

                    // Populate PODetailModel
                    detail.Sku = value.Rows[i][5] != null ? Convert.ToString(value.Rows[i][5]) : null;
                    detail.ExpectedQty = Convert.ToInt32(value.Rows[i][6]);
                    detail.DateCreated = currentDateTime;
                    detail.DateModified = currentDateTime;
                    detail.CreatedBy = value.Rows[i][18] != null ? Convert.ToString(value.Rows[i][18]) : null;
                    detail.ModifiedBy = value.Rows[i][19] != null ? Convert.ToString(value.Rows[i][19]) : null;
                    detail.Remarks = value.Rows[i][7] != null ? Convert.ToString(value.Rows[i][7]) : null;

                    // Add the detail to the PODetails collection
                    ((List<ReturnsDetailModel>)rows.ReturnsDetails).Add(detail);

                    Parameters.Add(rows);
                }

                if (Parameters.Count > 0)
                {
                    using (IDbConnection db = new MySqlConnection(ConnString))
                    {
                        db.Open();

                        foreach (ReturnsModelMod rows in Parameters)
                        {
                            var parameters = new
                            {
                                poId = rows.ReturnsHeader.ReturnsId,
                                refNumber = rows.ReturnsHeader.RefNumber,
                                refNumber2 = rows.ReturnsHeader.RefNumber2,
                                orderDate = rows.ReturnsHeader.ReturnDate,
                                arrivalDate = rows.ReturnsHeader.ArrivalDate,
                                arrivalDate2 = rows.ReturnsHeader.ArrivalDate2,
                                remarks = rows.ReturnsHeader.Remarks,
                                storeId = rows.ReturnsHeader.StoreId,
                                storeFrom = rows.ReturnsHeader.StoreFrom,
                                storeAddress = rows.ReturnsHeader.StoreAddress,
                                storeContact = rows.ReturnsHeader.StoreContact,
                                storeEmail = rows.ReturnsHeader.StoreEmail,
                                carrierId = rows.ReturnsHeader.CarrierId,
                                carrierName = rows.ReturnsHeader.CarrierName,
                                carrierAddress = rows.ReturnsHeader.CarrierAddress,
                                carrierContact = rows.ReturnsHeader.CarrierContact,
                                carrierEmail = rows.ReturnsHeader.CarrierEmail,
                                returnStatusId = rows.ReturnsHeader.ReturnsStatusId,
                                returnStatus = rows.ReturnsHeader.ReturnStatus,
                                dateCreated = rows.ReturnsHeader.DateCreated,
                                dateModified = rows.ReturnsHeader.DateModified,
                                createdBy = rows.ReturnsHeader.CreatedBy,
                                modifyBy = rows.ReturnsHeader.ModifiedBy
                            };

                            // check if PO primary reference number are unique
                            if (!string.IsNullOrEmpty(rows.ReturnsHeader.RefNumber))
                            {
                                var poCount = await ReferenceNumExists(db, rows.ReturnsHeader.RefNumber);
                                if (poCount > 0)
                                {
                                    return new ReturnsCreateTranResult()
                                    {
                                        ResultCode = ReturnsTranResultCode.INVALIDREFNUMONE
                                    };
                                }
                            }

                            // check if PO secondary reference number are unique
                            if (!string.IsNullOrEmpty(rows.ReturnsHeader.RefNumber2))
                            {
                                var poCount = await ReferenceNumExists(db, rows.ReturnsHeader.RefNumber2);
                                if (poCount > 0)
                                {
                                    return new ReturnsCreateTranResult()
                                    {
                                        ResultCode = ReturnsTranResultCode.INVALIDREFNUMTWO
                                    };
                                }
                            }

                            // create header
                            var headCreated = await CreateReturns(db, rows.ReturnsHeader);

                            if (headCreated)
                            {
                                // init po user fields default data
                                var initPOUFld = await RetUFieldRepo.InitReturnsUField(db, rows.ReturnsHeader.ReturnsId);
                                if (!initPOUFld)
                                {
                                    return new ReturnsCreateTranResult()
                                    {
                                        ResultCode = ReturnsTranResultCode.USRFIELDSAVEFAILED
                                    };
                                }

                                // insert po user fields values
                                if (rows.ReturnsUfields != null)
                                {
                                    var uFieldsCreated = await RetUFieldRepo.UpdateReturnsUField(db, rows.ReturnsHeader.ReturnsId, rows.ReturnsHeader.CreatedBy, rows.ReturnsUfields);
                                    if (!uFieldsCreated)
                                    {
                                        return new ReturnsCreateTranResult()
                                        {
                                            ResultCode = ReturnsTranResultCode.USRFIELDSAVEFAILED
                                        };
                                    }
                                }

                                // create detail
                                if (rows.ReturnsDetails.Any())
                                {
                                    var details = rows.ReturnsDetails.ToList();

                                    for (int i = 0; i < details.Count(); i++)
                                    {
                                        var detail = details[i];

                                        // check if similar SKU exists under this PO
                                        var skuExists = await SKUExistsInReturns(db, detail.Sku, rows.ReturnsHeader.ReturnsId);
                                        if (skuExists)
                                        {
                                            return new ReturnsCreateTranResult()
                                            {
                                                ResultCode = ReturnsTranResultCode.SKUCONFLICT
                                            };
                                        }

                                        // set detail id, status and header po id
                                        detail.ReturnsLineId = $"{rows.ReturnsHeader.ReturnsId}-{i + 1}";
                                        detail.ReturnsLineStatusId = (RetLneStatus.CREATED).ToString();
                                        detail.ReturnsId = rows.ReturnsHeader.ReturnsId;

                                        // create detail
                                        bool dtlSaved = await RetDetailRepo.CreateReturnsDetailMod(db, detail);

                                        // return false if either of detail failed to save
                                        if (!dtlSaved)
                                        {
                                            return new ReturnsCreateTranResult()
                                            {
                                                ResultCode = ReturnsTranResultCode.RETLINESAVEFAILED
                                            };
                                        }
                                    }
                                }
                            }
                        }

                        return new ReturnsCreateTranResult()
                        {
                            ResultCode = ReturnsTranResultCode.SUCCESS,
                            ReturnsIds = Parameters.Select(p => p.ReturnsHeader.ReturnsId).ToArray()
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
                                ReturnsModelMod rows = new ReturnsModelMod();
                                rows.ReturnsHeader = new ReturnsModel();
                                rows.ReturnsDetails = new List<ReturnsDetailModel>();
                                ReturnsDetailModel detail = new ReturnsDetailModel();

                                // get PO id number
                                var retId = await IdNumberRepo.GetNextIdNum("RCVRET");
                                rows.ReturnsHeader.ReturnsId = retId;

                                rows.ReturnsHeader.RefNumber = dataSet.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[0]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.RefNumber))
                                {
                                    return new ReturnsCreateTranResult()
                                    {
                                        ResultCode = ReturnsTranResultCode.MISSINGREFNUMONE
                                    };
                                }

                                rows.ReturnsHeader.RefNumber2 = dataSet.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[1]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.RefNumber2))
                                {
                                    rows.ReturnsHeader.RefNumber2 = null;
                                }

                                string? returnDateValue = dataSet.Tables[0].Rows[i].ItemArray[2]?.ToString();
                                if (string.IsNullOrEmpty(returnDateValue))
                                {
                                    return new ReturnsCreateTranResult()
                                    {
                                        ResultCode = ReturnsTranResultCode.RETURNDATEISREQUIRED
                                    };
                                }
                                else
                                {
                                    DateTime returnDate;
                                    if (!DateTime.TryParseExact(returnDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out returnDate))
                                    {
                                        returnDate = DateTime.MinValue;
                                    }
                                    rows.ReturnsHeader.ReturnDate = returnDate;
                                }

                                string? arrivalDateValue = dataSet.Tables[0].Rows[i].ItemArray[3]?.ToString();
                                if (string.IsNullOrEmpty(arrivalDateValue))
                                {
                                    rows.ReturnsHeader.ArrivalDate = null;
                                }
                                else
                                {
                                    DateTime arrivalDate;
                                    if (!DateTime.TryParseExact(arrivalDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate))
                                    {
                                        arrivalDate = DateTime.MinValue;
                                    }
                                    rows.ReturnsHeader.ArrivalDate = arrivalDate;
                                }

                                // check if expected arrival date is valid
                                if (rows.ReturnsHeader.ArrivalDate != null)
                                {
                                    if (rows.ReturnsHeader.ReturnDate > rows.ReturnsHeader.ArrivalDate)
                                    {
                                        return new ReturnsCreateTranResult()
                                        {
                                            ResultCode = ReturnsTranResultCode.INVALIDRETURNDATE
                                        };
                                    }
                                }
                                string? arrivalDate2Value = dataSet.Tables[0].Rows[i].ItemArray[4]?.ToString();
                                if (string.IsNullOrEmpty(arrivalDate2Value))
                                {
                                    rows.ReturnsHeader.ArrivalDate2 = null;
                                }
                                else
                                {
                                    DateTime arrivalDate2;
                                    if (!DateTime.TryParseExact(arrivalDate2Value, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate2))
                                    {
                                        arrivalDate2 = DateTime.MinValue;
                                    }
                                    rows.ReturnsHeader.ArrivalDate2 = arrivalDate2;
                                }

                                //check if expected arrival2 date is valid
                                if (rows.ReturnsHeader.ArrivalDate2 != null)
                                {
                                    if (rows.ReturnsHeader.ReturnDate > rows.ReturnsHeader.ArrivalDate2)
                                    {
                                        return new ReturnsCreateTranResult()
                                        {
                                            ResultCode = ReturnsTranResultCode.INVALIDRETURNDATE
                                        };
                                    }
                                }

                                rows.ReturnsHeader.Remarks = dataSet.Tables[0].Rows[i].ItemArray[7] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[7]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.Remarks))
                                {
                                    rows.ReturnsHeader.Remarks = null;
                                }
                                rows.ReturnsHeader.StoreId = dataSet.Tables[0].Rows[i].ItemArray[8] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[8]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.StoreId))
                                {
                                    rows.ReturnsHeader.StoreId = null;
                                }
                                rows.ReturnsHeader.StoreFrom = dataSet.Tables[0].Rows[i].ItemArray[9] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[9]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.StoreFrom))
                                {
                                    rows.ReturnsHeader.StoreFrom = null;
                                }
                                rows.ReturnsHeader.StoreAddress = dataSet.Tables[0].Rows[i].ItemArray[10] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[10]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.StoreAddress))
                                {
                                    rows.ReturnsHeader.StoreAddress = null;
                                }
                                rows.ReturnsHeader.StoreContact = dataSet.Tables[0].Rows[i].ItemArray[11] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[11]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.StoreContact))
                                {
                                    rows.ReturnsHeader.StoreContact = null;
                                }
                                rows.ReturnsHeader.StoreEmail = dataSet.Tables[0].Rows[i].ItemArray[12] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[12]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.StoreEmail))
                                {
                                    rows.ReturnsHeader.StoreEmail = null;
                                }
                                rows.ReturnsHeader.CarrierId = dataSet.Tables[0].Rows[i].ItemArray[13] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[13]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.CarrierId))
                                {
                                    rows.ReturnsHeader.CarrierId = null;
                                }
                                rows.ReturnsHeader.CarrierName = dataSet.Tables[0].Rows[i].ItemArray[14] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[14]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.CarrierName))
                                {
                                    rows.ReturnsHeader.CarrierName = null;
                                }
                                rows.ReturnsHeader.CarrierAddress = dataSet.Tables[0].Rows[i].ItemArray[15] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[15]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.CarrierAddress))
                                {
                                    rows.ReturnsHeader.CarrierAddress = null;
                                }
                                rows.ReturnsHeader.CarrierContact = dataSet.Tables[0].Rows[i].ItemArray[16] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[16]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.CarrierContact))
                                {
                                    rows.ReturnsHeader.CarrierContact = null;
                                }
                                rows.ReturnsHeader.CarrierEmail = dataSet.Tables[0].Rows[i].ItemArray[17] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[17]) : null;
                                if (string.IsNullOrEmpty(rows.ReturnsHeader.CarrierEmail))
                                {
                                    rows.ReturnsHeader.CarrierEmail = null;
                                }
                                DateTime currentDateTime = DateTime.Now;
                                rows.ReturnsHeader.ReturnsStatusId = POStatus.CREATED.ToString();
                                rows.ReturnsHeader.ReturnStatus = "Created";
                                rows.ReturnsHeader.DateCreated = currentDateTime;
                                rows.ReturnsHeader.DateModified = currentDateTime;
                                rows.ReturnsHeader.CreatedBy = dataSet.Tables[0].Rows[i].ItemArray[18] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[18]) : null;
                                rows.ReturnsHeader.ModifiedBy = dataSet.Tables[0].Rows[i].ItemArray[19] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[19]) : null;

                                // Populate PODetailModel
                                detail.Sku = dataSet.Tables[0].Rows[i].ItemArray[5] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[5]) : null;
                                detail.ExpectedQty = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[6]);
                                detail.DateCreated = currentDateTime;
                                detail.DateModified = currentDateTime;
                                detail.CreatedBy = dataSet.Tables[0].Rows[i].ItemArray[18] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[18]) : null;
                                detail.ModifiedBy = dataSet.Tables[0].Rows[i].ItemArray[19] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[19]) : null;
                                detail.Remarks = dataSet.Tables[0].Rows[i].ItemArray[7] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[7]) : null;

                                // Add the detail to the PODetails collection
                                ((List<ReturnsDetailModel>)rows.ReturnsDetails).Add(detail);

                                Parameters.Add(rows);
                            }

                            filestream.Close();

                            if (Parameters.Count > 0)
                            {
                                using (IDbConnection db = new MySqlConnection(ConnString))
                                {
                                    db.Open();

                                    foreach (ReturnsModelMod rows in Parameters)
                                    {
                                        var parameters = new
                                        {
                                            returnId = rows.ReturnsHeader.ReturnsId,
                                            refNumber = rows.ReturnsHeader.RefNumber,
                                            refNumber2 = rows.ReturnsHeader.RefNumber2,
                                            returnDate = rows.ReturnsHeader.ReturnDate,
                                            arrivalDate = rows.ReturnsHeader.ArrivalDate,
                                            arrivalDate2 = rows.ReturnsHeader.ArrivalDate2,
                                            remarks = rows.ReturnsHeader.Remarks,
                                            storeId = rows.ReturnsHeader.StoreId,
                                            storeFrom = rows.ReturnsHeader.StoreFrom,
                                            storeAddress = rows.ReturnsHeader.StoreAddress,
                                            storeContact = rows.ReturnsHeader.StoreContact,
                                            storeEmail = rows.ReturnsHeader.StoreEmail,
                                            carrierId = rows.ReturnsHeader.CarrierId,
                                            carrierName = rows.ReturnsHeader.CarrierName,
                                            carrierAddress = rows.ReturnsHeader.CarrierAddress,
                                            carrierContact = rows.ReturnsHeader.CarrierContact,
                                            carrierEmail = rows.ReturnsHeader.CarrierEmail,
                                            returnStatusId = rows.ReturnsHeader.ReturnsStatusId,
                                            returnStatus = rows.ReturnsHeader.ReturnStatus,
                                            dateCreated = rows.ReturnsHeader.DateCreated,
                                            dateModified = rows.ReturnsHeader.DateModified,
                                            createdBy = rows.ReturnsHeader.CreatedBy,
                                            modifyBy = rows.ReturnsHeader.ModifiedBy
                                        };

                                        // check if Return primary reference number are unique
                                        if (!string.IsNullOrEmpty(rows.ReturnsHeader.RefNumber))
                                        {
                                            var returnCount = await ReferenceNumExists(db, rows.ReturnsHeader.RefNumber);
                                            if (returnCount > 0)
                                            {
                                                return new ReturnsCreateTranResult()
                                                {
                                                    ResultCode = ReturnsTranResultCode.INVALIDREFNUMONE
                                                };
                                            }
                                        }

                                        // check if Return secondary reference number are unique
                                        if (!string.IsNullOrEmpty(rows.ReturnsHeader.RefNumber2))
                                        {
                                            var returnCount = await ReferenceNumExists(db, rows.ReturnsHeader.RefNumber2);
                                            if (returnCount > 0)
                                            {
                                                return new ReturnsCreateTranResult()
                                                {
                                                    ResultCode = ReturnsTranResultCode.INVALIDREFNUMTWO
                                                };
                                            }
                                        }

                                        // create header
                                        var headCreated = await CreateReturns(db, rows.ReturnsHeader);

                                        if (headCreated)
                                        {
                                            // init po user fields default data
                                            var initPOUFld = await RetUFieldRepo.InitReturnsUField(db, rows.ReturnsHeader.ReturnsId);
                                            if (!initPOUFld)
                                            {
                                                return new ReturnsCreateTranResult()
                                                {
                                                    ResultCode = ReturnsTranResultCode.USRFIELDSAVEFAILED
                                                };
                                            }

                                            // insert po user fields values
                                            if (rows.ReturnsUfields != null)
                                            {
                                                var uFieldsCreated = await RetUFieldRepo.UpdateReturnsUField(db, rows.ReturnsHeader.ReturnsId, rows.ReturnsHeader.CreatedBy, rows.ReturnsUfields);
                                                if (!uFieldsCreated)
                                                {
                                                    return new ReturnsCreateTranResult()
                                                    {
                                                        ResultCode = ReturnsTranResultCode.USRFIELDSAVEFAILED
                                                    };
                                                }
                                            }

                                            // create detail
                                            if (rows.ReturnsDetails.Any())
                                            {
                                                var details = rows.ReturnsDetails.ToList();

                                                for (int i = 0; i < details.Count(); i++)
                                                {
                                                    var detail = details[i];

                                                    // check if similar SKU exists under this PO
                                                    var skuExists = await SKUExistsInReturns(db, detail.Sku, rows.ReturnsHeader.ReturnsId);
                                                    if (skuExists)
                                                    {
                                                        return new ReturnsCreateTranResult()
                                                        {
                                                            ResultCode = ReturnsTranResultCode.SKUCONFLICT
                                                        };
                                                    }

                                                    // set detail id, status and header po id
                                                    detail.ReturnsLineId = $"{rows.ReturnsHeader.ReturnsId}-{i + 1}";
                                                    detail.ReturnsLineStatusId = (RetLneStatus.CREATED).ToString();
                                                    detail.ReturnsId = rows.ReturnsHeader.ReturnsId;

                                                    // create detail
                                                    bool dtlSaved = await RetDetailRepo.CreateReturnsDetailMod(db, detail);

                                                    // return false if either of detail failed to save
                                                    if (!dtlSaved)
                                                    {
                                                        return new ReturnsCreateTranResult()
                                                        {
                                                            ResultCode = ReturnsTranResultCode.RETLINESAVEFAILED
                                                        };
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    return new ReturnsCreateTranResult()
                                    {
                                        ResultCode = ReturnsTranResultCode.SUCCESS,
                                        ReturnsIds = Parameters.Select(p => p.ReturnsHeader.ReturnsId).ToArray()
                                    };
                                }
                            }
                        }
                        else
                        {
                            return new ReturnsCreateTranResult()
                            {
                                ResultCode = ReturnsTranResultCode.INVALIDHEADER
                            };
                        }
                    }
                }
            }

            return new ReturnsCreateTranResult()
            {
                ResultCode = ReturnsTranResultCode.FAILED
            };
        }

        public bool ValidateCsvHeader(string headerLine)
        {
            string[] expectedHeaders = { "Reference Number", "2nd Reference Number", "Return Date",
                                         "Arrival Date", "Arrival Date 2", "SKU",
                                         "Expected Qty", "Remarks", "Store Id",
                                         "Store From", "Store Address", "Store Contact",
                                         "Store Email", "Carrier Id", "Carrier Name",
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
            string[] expectedHeaders = { "Reference Number", "2nd Reference Number", "Return Date",
                                         "Arrival Date", "Arrival Date 2", "SKU",
                                         "Expected Qty", "Remarks", "Store Id",
                                         "Store From", "Store Address", "Store Contact",
                                         "Store Email", "Carrier Id", "Carrier Name",
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
