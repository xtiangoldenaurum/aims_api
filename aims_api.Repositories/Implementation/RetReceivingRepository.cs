using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using aims_api.Utilities.Interface;
using aims_printsvc.Enums;
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
    public class RetReceivingRepository : IRetReceivingRepository
    {
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        IReturnsRepository ReturnsRepo;
        IReturnsDetailRepository RetDetailRepo;
        IInventoryRepository InventoryRepo;
        IInventoryHistoryRepository InvHistoryRepo;
        ILotAttributeDetailRepository LotAttRepo;
        IPutawayTaskRepository PutawayTaskRepo;
        IPagingRepository PagingRepo;
        IAuditTrailRepository AuditTrailRepo;
        ReceivingAudit AuditBuilder;
        IPrintHelperRepository PrintHelperRepo;
        IUniqueTagsRepository UniqueTagsRepo;

        // constructor to init prerequisit services/repos
        public RetReceivingRepository(ITenantProvider tenantProvider, 
                                    IAuditTrailRepository auditTrailRepo, 
                                    IIdNumberRepository idNumberRepo, 
                                    IReturnsRepository returnsRepo, 
                                    IReturnsDetailRepository retDetailRepo, 
                                    IInventoryRepository inventoryRepo, 
                                    IInventoryHistoryRepository invHistoryRepo, 
                                    ILotAttributeDetailRepository lotAttRepo, 
                                    IPutawayTaskRepository putawayTaskRepo, 
                                    IPrintHelperRepository printHelperRepo,
                                    IUniqueTagsRepository uniqueTagsRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            ReturnsRepo = returnsRepo;
            RetDetailRepo = retDetailRepo;
            InventoryRepo = inventoryRepo;
            InvHistoryRepo = invHistoryRepo;
            LotAttRepo = lotAttRepo;
            PutawayTaskRepo = putawayTaskRepo;
            PrintHelperRepo = printHelperRepo;
            UniqueTagsRepo = uniqueTagsRepo;
            PagingRepo = new PagingRepository();
            AuditBuilder = new ReceivingAudit();
        }

        public async Task<RetReceivePagedMdl?> GetReceivesByReturnsId(string returnsId, int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetReceivesByRetId`(@retId, @pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@retId", returnsId);
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                var ret = await db.QueryAsync<RetReceivedDetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "CALL `spCountReceivesByRetId`(@retId)";

                    var pgParam = new DynamicParameters();
                    pgParam.Add("@retId", returnsId);

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new RetReceivePagedMdl()
                    {
                        Pagination = pageDetail,
                        Receives = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<RetReceivedDetailModel>?> GetCancelableRcvsById(string retLineId, int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetCancelableRcvsByRetLneId`(@retDetailId, @pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@retDetailId", retLineId);
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                return await db.QueryAsync<RetReceivedDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<RetReceivedDetailModel>?> GetCancelableRcvsByRetId(string returnsId, int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetCancelableRcvsByRetId`(@retId, @pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@retId", returnsId);
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                return await db.QueryAsync<RetReceivedDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<ReceivingModel>> GetReceivingPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Receiving where 
														receivingId like @searchKey or 
														docLineId like @searchKey or 
														inventoryId like @searchKey or 
														seqNum like @searchKey or 
                                                        receivingStatusId like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ReceivingModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ReceivingModel> GetReceivingById(string receivingId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Receiving where 
														receivingId = @receivingId";

                var param = new DynamicParameters();
                param.Add("@receivingId", receivingId);
                return await db.QuerySingleOrDefaultAsync<ReceivingModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ReceivingModel> GetReceivingByIdMod(IDbConnection db, string receivingId)
        {
            string strQry = @"select * from Receiving where 
														receivingId = @receivingId";

            var param = new DynamicParameters();
            param.Add("@receivingId", receivingId);

            return await db.QuerySingleOrDefaultAsync<ReceivingModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> ReceivingExists(string receivingId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select receivingId from Receiving where 
														receivingId = @receivingId";

                var param = new DynamicParameters();
                param.Add("@receivingId", receivingId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<ReceivingResultModel> Receiving(RetReceivingModelMod data)
        {
            // init return object
            var ret = new ReceivingResultModel();
            ret.ResultCode = ReceivingResultCode.FAILED;

            // check if required details is not null
            if (data.InvHead != null &&
                data.InvDetail != null &&
                data.LotAtt != null)
            {
                // init to be printed zpl doctypes holder
                var labelsToPrint = new List<BCodeLabelToPrintModel>();

                // init objects
                var invHead = data.InvHead;
                var invDetail = data.InvDetail;
                var lotAtt = data.LotAtt;
                var tags = data.UniqTags;

                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // lock Returns Detail record
                    var retDetail = await RetDetailRepo.LockRetDetail(db, invHead.ReturnsLineId);

                    // lock returns record
                    var returns = await ReturnsRepo.LockReturns(db, invHead.ReturnsId);

                    if (retDetail != null &&
                        retDetail.ReturnsLineId == invHead.ReturnsLineId &&
                        returns.ReturnsId != null &&
                        returns.ReturnsId == invHead.ReturnsId)
                    {
                        // check if qty to receive is valid
                        if (invHead.QtyToReceive < 1 && retDetail.ExpectedQty < invHead.QtyToReceive)
                        {
                            ret.ResultCode = ReceivingResultCode.INVALIDQTY;
                            return ret;
                        }

                        // check header if still in receivable status
                        var hdrValid = await ReturnsRepo.ReturnsReceivable(invHead.ReturnsId);
                        if (!hdrValid ||
                            returns.ReturnsStatusId == (POStatus.FULLRCV).ToString() ||
                            returns.ReturnsStatusId == (POStatus.FRCCLOSED).ToString() ||
                            returns.ReturnsStatusId == (POStatus.CANCELED).ToString() ||
                            returns.ReturnsStatusId == (POStatus.CLOSED).ToString())
                        {
                            ret.ResultCode = ReceivingResultCode.INVALIDPO;
                            return ret;
                        }


                        // check detail itself if still in receivable status
                        var dtlValid = await RetDetailRepo.RetDetailReceivable(invHead.ReturnsLineId);
                        if (!dtlValid)
                        {
                            ret.ResultCode = ReceivingResultCode.INVALIDPOLANE;
                            return ret;
                        }

                        // check if po detail has receivable qty left
                        int qtyReceived = await GetDocLineRcvdQty(db, retDetail.ReturnsLineId);
                        int availableQty = retDetail.ExpectedQty - qtyReceived;
                        int leftOverQty = availableQty - invHead.QtyToReceive;

                        if (availableQty < 1 || availableQty < invHead.QtyToReceive)
                        {
                            ret.ResultCode = ReceivingResultCode.INVALIDQTY;
                            return ret;
                        }

                        // check if expiry date is valid
                        if (lotAtt.ManufactureDate > DateTime.Now)
                        {
                            ret.ResultCode = ReceivingResultCode.LOTINVALIDDATE;
                            return ret;
                        }

                        // update product status if expired
                        if (lotAtt.ExpiryDate <= DateTime.Now && 
                            lotAtt.ProductConditionId == (ProductCondition.GOOD).ToString())
                        {
                            ret.ResultCode = ReceivingResultCode.INVALIDCONDITION;
                            return ret;
                        }

                        // create inventory detail section

                        // get inventory next document number
                        string? invId = await IdNumberRepo.GetNxtDocNum("INV", invHead.UserAccountId);

                        // get lotattribute next document number (reuse lot id if similar detail exists)
                        bool createLot = false;
                        string? lotAttId = await LotAttRepo.GetLotAttributeIdWithSameDetail(lotAtt);
                        if (string.IsNullOrEmpty(lotAttId))
                        {
                            lotAttId = await IdNumberRepo.GetNxtDocNum("LOT", invHead.UserAccountId);
                            createLot = true;
                        }

                        // define lotatt detail's lot number
                        lotAtt.LotAttributeId = lotAttId;

                        // flag to print lotatt id label
                        labelsToPrint.Add(new BCodeLabelToPrintModel() { 
                            DocType = PrinterDocType.LotAttId,
                            DocTypeId = (PrinterDocType.LotAttId).ToString(),
                            Description = lotAtt.LotAttributeId,
                            BarcodeContent = lotAtt.LotAttributeId,
                            EPC = lotAtt.LotAttributeId,
                        });

                        // get receiving next document number
                        string? receivingId = await IdNumberRepo.GetNxtDocNum("RCVING", invHead.UserAccountId);

                        // get putaway task next document number
                        string? putawayTaskId = await IdNumberRepo.GetNxtDocNum("PUTAWAY", invHead.UserAccountId);

                        // get TID To if asterisk/* is provided
                        string? tidTo = "*";
                        if (invDetail.TrackIdTo == "*" || string.IsNullOrEmpty(invDetail.TrackIdTo))
                        {
                            tidTo = await IdNumberRepo.GetNxtDocNum("TRACE", invHead.UserAccountId);
                            if (tidTo != "*")
                            {
                                invDetail.TrackIdTo = tidTo;

                                // flag to print track id label
                                labelsToPrint.Add(new BCodeLabelToPrintModel()
                                {
                                    DocType = PrinterDocType.TrackId,
                                    DocTypeId = (PrinterDocType.TrackId).ToString(),
                                    Description = invDetail.TrackIdTo,
                                    BarcodeContent = invDetail.TrackIdTo,
                                    EPC = invDetail.TrackIdTo,
                                });
                            }
                            else
                            {
                                ret.ResultCode = ReceivingResultCode.FAILED;
                                return ret;
                            }
                        }

                        // check if TID is in correct format
                        var tidPrefix = await IdNumberRepo.GetIdPrefix(db, (TranType.TRACE).ToString());
                        if (!string.IsNullOrEmpty(tidPrefix))
                        {
                            if (!invDetail.TrackIdTo.Contains(tidPrefix))
                            {
                                ret.ResultCode = ReceivingResultCode.INVALIDTIDFORMAT;
                                return ret;
                            }
                        }

                        // check if TID has no any record yet
                        var isCleanTID = await InvHistoryRepo.CheckTrackIdExists(db, invDetail.TrackIdTo);
                        if (isCleanTID)
                        {
                            ret.ResultCode = ReceivingResultCode.INVALIDTID;
                            return ret;
                        }

                        // get LPN To if asterisk/* is provided
                        string? lpnTo = "*";
                        if (invDetail.LpnTo == "*")
                        {
                            lpnTo = await IdNumberRepo.GetNxtDocNum("LPN", invHead.UserAccountId);
                            if (lpnTo != "*")
                            {
                                invDetail.LpnTo = lpnTo;

                                // flag to print lpn id label
                                labelsToPrint.Add(new BCodeLabelToPrintModel()
                                {
                                    DocType = PrinterDocType.LPNId,
                                    DocTypeId = (PrinterDocType.LPNId).ToString(),
                                    Description = invDetail.LpnTo,
                                    BarcodeContent = invDetail.LpnTo,
                                    EPC = invDetail.LpnTo
                                });
                            }
                            else
                            {
                                
                                ret.ResultCode = ReceivingResultCode.FAILED;
                            }
                        }

                        // check if LPNTo is not in used by any location except INSTAGING
                        if (!string.IsNullOrEmpty(invDetail.LpnTo))
                        {
                            var isLPNUsed = await InvHistoryRepo.ChkLPNIsUsedInStorage(db, invDetail.LpnTo);
                            if (isLPNUsed)
                            {
                                ret.ResultCode = ReceivingResultCode.INVALIDLPNID;
                                return ret;
                            }

                            // check if LPN Id is in correct format
                            var lpnPrefix = await IdNumberRepo.GetIdPrefix(db, (TranType.LPN).ToString());
                            if (!string.IsNullOrEmpty(lpnPrefix))
                            {
                                if (!invDetail.LpnTo.Contains(lpnPrefix))
                                {
                                    ret.ResultCode = ReceivingResultCode.INVALIDLPNFORMAT;
                                    return ret;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(invId) &&
                            !string.IsNullOrEmpty(lotAttId) &&
                            !string.IsNullOrEmpty(receivingId) &&
                            !string.IsNullOrEmpty(putawayTaskId))
                        {
                            // build inventory table data
                            var inv = new InventoryModel()
                            {
                                InventoryId = invId,
                                Sku = retDetail.Sku,
                                InventoryStatusId = (InvStatus.REFERRED).ToString()
                            };

                            // save header to inventory table
                            var invSaved = await InventoryRepo.CreateInventoryMod(db, inv, invHead.UserAccountId, TranType.RCVING);

                            if (invSaved)
                            {
                                // check record conflict on inventoryhistory table --skipped this validation (may not be needed)

                                // build inventory history detail defaults
                                invDetail.InventoryId = invId;
                                invDetail.SeqNum = 1;
                                invDetail.DocumentRefId = retDetail.ReturnsLineId;
                                invDetail.QtyFrom = 0;
                                invDetail.QtyTo = invHead.QtyToReceive;
                                invDetail.LocationFrom = "*";
                                invDetail.TrackIdFrom = "*";
                                invDetail.LpnFrom = "*";
                                invDetail.LotAttributeId = lotAttId;
                                invDetail.TransactionTypeId = "RCVING";
                                invDetail.CreatedBy = invHead.UserAccountId;

                                // record lotattribute detail to lotattributedetail table if is new lotId
                                bool lotSaved;
                                if (createLot)
                                {
                                    lotSaved = await LotAttRepo.CreateLotAttributeDetailMod(db, lotAtt, TranType.RCVING);
                                }
                                else
                                {
                                    lotSaved = true;
                                }

                                if (lotSaved)
                                {
                                    // record inventory detail to inventory history table
                                    bool dtlSaved = await InvHistoryRepo.CreateInventoryHistoryMod(db, invDetail, TranType.RCVING);

                                    if (dtlSaved)
                                    {
                                        // build receiving transaction detail
                                        var rcvDetail = new ReceivingModel()
                                        {
                                            ReceivingId = receivingId,
                                            DocLineId = retDetail.ReturnsLineId,
                                            InventoryId = invId,
                                            SeqNum = 1,
                                            ReceivingStatusId = (ReceivingStatus.CREATED).ToString(),
                                            CreatedBy = invHead.UserAccountId,
                                            ModifiedBy = invHead.UserAccountId
                                        };

                                        // record receiving detail to receving table
                                        bool rcvSaved = await CreateReceivingMod(db, rcvDetail, TranType.RCVING);

                                        if (rcvSaved)
                                        {
                                            // build putaway task data
                                            var putawayDtl = new PutawayTaskModel()
                                            {
                                                PutawayTaskId = putawayTaskId,
                                                ReceivingId = receivingId,
                                                InventoryId = invId,
                                                SeqNum = 1,
                                                PutawayStatusId = (PutawayStatus.CREATED).ToString(),
                                                CreatedBy = invHead.UserAccountId,
                                                ModifiedBy = invHead.UserAccountId
                                            };

                                            // record putaway task to putawaytask table
                                            bool putawaySaved = await PutawayTaskRepo.CreatePutawayTaskMod(db, putawayDtl, TranType.RCVING);

                                            if (putawaySaved)
                                            {
                                                // update return detail status
                                                retDetail.ModifiedBy = invHead.UserAccountId;
                                                if (leftOverQty == 0)
                                                {
                                                    retDetail.ReturnsLineStatusId = (POLneStatus.FULLRCV).ToString();
                                                }
                                                else
                                                {
                                                    retDetail.ReturnsLineStatusId = (POLneStatus.PRTRCV).ToString();
                                                }

                                                // save updated Returns detail record
                                                var retDtlUpdated = await RetDetailRepo.UpdateReturnsDetailMod(db, retDetail, TranType.RCVING);
                                                if (retDtlUpdated)
                                                {
                                                    // update Returns status
                                                    // get Returns updated status
                                                    var retStatus = await ReturnsRepo.GetReturnsUpdatedStatus(db, returns.ReturnsId);

                                                    if (!string.IsNullOrEmpty(retStatus))
                                                    {
                                                        // save update returns record
                                                        returns.ModifiedBy = invHead.UserAccountId;
                                                        returns.ReturnsStatusId = retStatus;

                                                        var retUpdated = await ReturnsRepo.UpdateReturns(db, returns, TranType.RCVING);

                                                        if (retUpdated)
                                                        {
                                                            // record captured serial numbers if there's any
                                                            if (tags != null)
                                                            {
                                                                if (tags.Any())
                                                                {
                                                                    // check if tags count meets to receive qty
                                                                    if (tags.Count() > invHead.QtyToReceive)
                                                                    {
                                                                        ret.ResultCode = ReceivingResultCode.UNIQTAGSEXCEEDSQTYTORCV;
                                                                        return ret;
                                                                    }

                                                                    // record unique tags
                                                                    var resTagAppend = await UniqueTagsRepo.CreateUniqueTagsMod(db,
                                                                                                                                tags,
                                                                                                                                tidTo,
                                                                                                                                TranType.RCVING,
                                                                                                                                returns.ReturnsId,
                                                                                                                                retDetail.ReturnsLineId,
                                                                                                                                invHead.UserAccountId);

                                                                    // revert if recording of captured serials is failed
                                                                    if (resTagAppend != RecordTagResultCode.SUCCESS)
                                                                    {
                                                                        ret.ResultCode = ReceivingResultCode.UNIQUETAGSAVEFIALED;
                                                                        return ret;
                                                                    }

                                                                    // build EPC zpl ref to print data
                                                                    var epcs = await PrintHelperRepo.BuildEPCZpls(tags);

                                                                    // check if return tags to print data is consistent in tags qty
                                                                    if (!epcs.Any() || (epcs.Count() != tags.Count()))
                                                                    {
                                                                        ret.ResultCode = ReceivingResultCode.SNTOPRINTBUILDFAILED;
                                                                        return ret;
                                                                    }

                                                                    // include EPC data on ZPL data to build
                                                                    labelsToPrint.AddRange(epcs);
                                                                }
                                                            }

                                                            // let commit and disregard possible error on zpl code generation

                                                            ret.ResultCode = ReceivingResultCode.SUCCESS;

                                                            // build zpl print codes for label printing
                                                            if (labelsToPrint.Any())
                                                            {
                                                                ret.LabelsToPrint = labelsToPrint;

                                                                var printData = await PrintHelperRepo.BuildZplDetails(labelsToPrint);
                                                                ret.ZplDetails = printData;

                                                                return ret;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        public async Task<int> GetDocLineRcvdQty(IDbConnection db, string docDetailId)
        {
            string strQry = @"CALL `spGetDocLineReceivedQty`(@docDetailId)";

            var param = new DynamicParameters();
            param.Add("@docDetailId", docDetailId);

            return await db.ExecuteScalarAsync<int>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> CreateReceivingMod(IDbConnection db, ReceivingModel receiving, TranType tranTyp)
        {
            string strQry = @"insert into Receiving(receivingId, 
														docLineId, 
														inventoryId, 
														seqNum, 
                                                        receivingStatusId, 
														createdBy, 
														modifiedBy)
 												values(@receivingId, 
														@docLineId, 
														@inventoryId, 
														@seqNum, 
                                                        @receivingStatusId, 
														@createdBy, 
														@modifiedBy)";

            int res = await db.ExecuteAsync(strQry, receiving);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildReceivingAuditADD(receiving, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelRcvResultCode> CancelReceived(string receivingId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock current receive detail
                var rcvDetail = await LockReceiveDetail(db, receivingId);
                if (rcvDetail == null)
                {
                    return CancelRcvResultCode.RCVLOCKFAIL; // cut process here if po detail is not retreived
                }

                // lock current receive detail putaway task
                var putawayDtl = await PutawayTaskRepo.LockPutawayTaskDtl(db, receivingId);
                if (putawayDtl == null)
                {
                    return CancelRcvResultCode.PUTAWAYLOCKFAIL; // cut process here if po detail is not retreived
                }

                // lock current receive returns detail
                var retDetail = await RetDetailRepo.LockRetDetail(db, rcvDetail.DocLineId);
                if (retDetail == null)
                {
                    return CancelRcvResultCode.RETDETAILLOCKFAIL; // cut process here if po detail is not retreived
                }

                // lock current receive returns header
                var retHeader = await ReturnsRepo.LockReturns(db, retDetail.ReturnsId);
                if (retHeader == null)
                {
                    return CancelRcvResultCode.RETLOCKFAIL; // cut process here if po header is not retreived
                }

                // check if receive detail is still receivable
                if (rcvDetail.ReceivingStatusId != (ReceivingStatus.CREATED).ToString())
                {
                    return CancelRcvResultCode.RCVINCONSISTENCY;
                }

                // check if putaway detail is still untouched
                if (putawayDtl.PutawayStatusId == (PutawayStatus.CANCELED).ToString() ||
                    putawayDtl.PutawayStatusId == (PutawayStatus.CLOSED).ToString())
                {
                    return CancelRcvResultCode.PUTAWAYINCONSISTENCY;
                }

                // check if returns detail is in receive status
                if (retDetail.ReturnsLineStatusId == (POLneStatus.CREATED).ToString() ||
                    retDetail.ReturnsLineStatusId == (POLneStatus.FRCCLOSED).ToString() ||
                    retDetail.ReturnsLineStatusId == (POLneStatus.CLOSED).ToString())
                {
                    return CancelRcvResultCode.RETDTLINCONSISTENCY;
                }

                // check if returns is still not closed, canceled or in create status
                if (retHeader.ReturnsStatusId == (POStatus.CREATED).ToString() ||
                    retHeader.ReturnsStatusId == (POStatus.CANCELED).ToString() ||
                    retHeader.ReturnsStatusId == (POStatus.FRCCLOSED).ToString() ||
                    retHeader.ReturnsStatusId == (POStatus.CLOSED).ToString())
                {
                    return CancelRcvResultCode.RETINCONSISTENCY;
                }

                // get most recent inventory history record
                var invHistDetail = await InvHistoryRepo.GetTopInvHistDetail(db, rcvDetail.InventoryId);

                if (invHistDetail != null)
                {
                    // lock inventory history detail
                    var tmpInvHist = await InvHistoryRepo.LockInvHistDetail(db, invHistDetail.InventoryId, invHistDetail.SeqNum);

                    if (tmpInvHist != null) // && invHistDetail == tmpInvHist) **skipped validation if invHist and lockedInvHist is the same**
                    {
                        // update inventoryhistory detail
                        invHistDetail.QtyFrom = invHistDetail.QtyTo;
                        invHistDetail.QtyTo = 0;
                        invHistDetail.SeqNum += 1;

                        // record new inventory history detail
                        var invHistSave = await InvHistoryRepo.CreateInventoryHistoryMod(db, invHistDetail, TranType.CANCELRCV);

                        if (invHistSave)
                        {
                            // update invtory detail status to closed
                            var invClosed = await InventoryRepo.SetInventoryStatus(db, invHistDetail.InventoryId, (InvStatus.CLOSED).ToString(), userAccountId, TranType.CANCELRCV);

                            if (invClosed)
                            {
                                // update putaway task status to canceled
                                var putawayClosed = await PutawayTaskRepo.SetPutawayStatus(db, putawayDtl.PutawayTaskId, (PutawayStatus.CANCELED).ToString(), userAccountId, TranType.CANCELRCV);

                                if (putawayClosed)
                                {
                                    // update receiving status to canceled 
                                    var rcvClosed = await SetReceivingStatus(db, receivingId, (ReceivingStatus.CANCELED).ToString(), TranType.CANCELRCV, userAccountId);

                                    if (rcvClosed)
                                    {
                                        // get returns detail updated line status
                                        var poDtlUpdatedStatus = await RetDetailRepo.GetRetDtlCancelRcvUpdatedStatus(db, retDetail.ReturnsLineId, receivingId);

                                        if (!string.IsNullOrEmpty(poDtlUpdatedStatus))
                                        {
                                            // commit update returns detail status
                                            retDetail.ModifiedBy = userAccountId;
                                            retDetail.ReturnsLineStatusId = poDtlUpdatedStatus;
                                            var retDtlUpdated = await RetDetailRepo.UpdateReturnsDetailMod(db, retDetail, TranType.CANCELRCV);

                                            if (retDtlUpdated)
                                            {
                                                // get updated returns status
                                                var retUpdatedStatus = await ReturnsRepo.GetReturnsUpdatedStatus(db, retHeader.ReturnsId);

                                                if (!string.IsNullOrEmpty(retUpdatedStatus))
                                                {
                                                    // save updated returns record
                                                    retHeader.ModifiedBy = userAccountId;
                                                    retHeader.ReturnsStatusId = retUpdatedStatus;

                                                    var retUpdated = await ReturnsRepo.UpdateReturns(db, retHeader, TranType.CANCELRCV);

                                                    if (retUpdated)
                                                    {
                                                        // revert recorded unique tags under this receives' tran track id
                                                        var delUniqTags = await UniqueTagsRepo.DeleteUniqueTagsMod(db, invHistDetail.TrackIdTo, userAccountId);
                                                        if (delUniqTags != CancelRcvResultCode.SUCCESS)
                                                        {
                                                            return delUniqTags;
                                                        }
                                                        
                                                        return CancelRcvResultCode.SUCCESS;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return CancelRcvResultCode.FAILED;
        }

        public async Task<ReceivingModel> LockReceiveDetail(IDbConnection db, string receivingId)
        {
            // hold currentr eceive transaction
            string strQry = @"select * from receiving where receivingId = @receivingId for update;";

            var param = new DynamicParameters();
            param.Add("@receivingId", receivingId);

            return await db.QuerySingleOrDefaultAsync<ReceivingModel>(strQry, param);
        }

        public async Task<bool> SetReceivingStatus(string receivingId, string receivingStatus)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update receiving set receivingStatusId = @receivingStatusId 
                                    where receivingId = @receivingId;";

                var param = new DynamicParameters();
                param.Add("@receivingStatus", receivingStatus);
                param.Add("@receivingId", receivingId);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> SetReceivingStatus(IDbConnection db, string receivingId, string receivingStatus, TranType tranTyp, string userAccountId)
        {
            string strQry = @"update receiving set receivingStatusId = @receivingStatusId, 
                                                    modifiedBy = @modifiedBy
                                    where receivingId = @receivingId;";

            var param = new DynamicParameters();
            param.Add("@receivingStatusId", receivingStatus);
            param.Add("@modifiedBy", userAccountId);
            param.Add("@receivingId", receivingId);

            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                // get updated receiving record
                var rcving = await GetReceivingByIdMod(db, receivingId);

                if (rcving != null)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(rcving, tranTyp);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
