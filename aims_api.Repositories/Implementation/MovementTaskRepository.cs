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
    public class MovementTaskRepository : IMovementTaskRepository
    {
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        IInvMoveRepository InvMoveRepo;
        IInvMoveDetailRepository InvMoveDetailRepo;
        IInventoryRepository InventoryRepo;
        IInventoryHistoryRepository InvHistoryRepo;
        ILotAttributeDetailRepository LotAttRepo;
        IPagingRepository PagingRepo;
        IAuditTrailRepository AuditTrailRepo;
        MovementTaskAudit AuditBuilder;
        public MovementTaskRepository(ITenantProvider tenantProvider,
                                    IAuditTrailRepository auditTrailRepo,
                                    IIdNumberRepository idNumberRepo,
                                    IInvMoveRepository invMoveRepo,
                                    IInvMoveDetailRepository invMoveDetailRepo,
                                    IInventoryRepository inventoryRepo,
                                    IInventoryHistoryRepository invHistoryRepo,
                                    ILotAttributeDetailRepository lotAttRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            InvMoveRepo = invMoveRepo;
            InvMoveDetailRepo = invMoveDetailRepo;
            InventoryRepo = inventoryRepo;
            InvHistoryRepo = invHistoryRepo;
            LotAttRepo = lotAttRepo;
            PagingRepo = new PagingRepository();
            AuditBuilder = new MovementTaskAudit();
        }

        public async Task<IEnumerable<MovementTaskModel>> GetMovementTaskPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from Receiving limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<MovementTaskModel>(strQry, param, commandType: CommandType.Text);
            }
        }
        public async Task<MovementTaskPagedMdl?> GetCancelableMv(int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetCancelableMvs`(@pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                var ret = await db.QueryAsync<InvMoveMovementTaskDetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "CALL `spCountCancelableMv`()";

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new MovementTaskPagedMdl()
                    {
                        Pagination = pageDetail,
                        Moves = ret
                    };
                }
            }

            return null;
        }

        public async Task<MovementTaskPagedMdl?> GetMovementTasksByInvMoveId(string invMoveId, int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetMovesByInvMoveId`(@invMoveId, @pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@invMoveId", invMoveId);
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                var ret = await db.QueryAsync<InvMoveMovementTaskDetailModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "CALL `spCountMovesByInvMoveId`(@invMoveId)";

                    var pgParam = new DynamicParameters();
                    pgParam.Add("@invMoveId", invMoveId);

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new MovementTaskPagedMdl()
                    {
                        Pagination = pageDetail,
                        Moves = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<InvMoveMovementTaskDetailModel>?> GetCancelableMvsById(string invMoveLineId, int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetCancelableMvsById`(@invMoveDetailId, @pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@invMoveDetailId", invMoveLineId);
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                return await db.QueryAsync<InvMoveMovementTaskDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvMoveMovementTaskDetailModel>?> GetCancelableMvsByInvMoveId(string invMoveId, int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetCancelableMvsByInvMoveId`(@invMoveId, @pageItem, @pgOffset)";

                int offset = (pageNum - 1) * pageItem;
                var param = new DynamicParameters();
                param.Add("@invMoveId", invMoveId);
                param.Add("@pageItem", pageItem);
                param.Add("@pgOffset", offset);

                return await db.QueryAsync<InvMoveMovementTaskDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<MovementTaskModel>> GetMovementTaskPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from MovementTask where 
														movementTaskId like @searchKey or 
														docLineId like @searchKey or 
														inventoryId like @searchKey or 
														seqNum like @searchKey or 
                                                        movementStatusId like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<MovementTaskModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<MovementTaskModel> GetMovementTaskById(string movementTaskId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from MovementTask where 
														movementTaskId = @movementTaskId";

                var param = new DynamicParameters();
                param.Add("@movementTaskId", movementTaskId);
                return await db.QuerySingleOrDefaultAsync<MovementTaskModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<MovementTaskModel> GetMovementTaskByIdMod(IDbConnection db, string movementTaskId)
        {
            string strQry = @"select * from MovementTask where 
														movementTaskId = @movementTaskId";

            var param = new DynamicParameters();
            param.Add("@movementTaskId", movementTaskId);

            return await db.QuerySingleOrDefaultAsync<MovementTaskModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> MovementTaskExists(string movementTaskId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select movementTaskId from MovementTask where 
														movementTaskId = @movementTaskId";

                var param = new DynamicParameters();
                param.Add("@movementTaskId", movementTaskId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        //public async Task<MovementTaskResultModel> MovementTask(MovementTaskModelMod data)
        //{
        //    // init return object
        //    var ret = new MovementTaskResultModel();
        //    ret.ResultCode = MovementTaskResultCode.FAILED;

        //    // check if required details is not null
        //    if (data.InvHead != null &&
        //        data.InvDetail != null)
        //    {
        //        // init objects
        //        var invHead = data.InvHead;
        //        var invDetail = data.InvDetail;

        //        using (IDbConnection db = new MySqlConnection(ConnString))
        //        {
        //            db.Open();

        //            // lock InvMoveDetail record
        //            var invMoveDetail = await InvMoveDetailRepo.LockInvMoveDetail(db, invHead.InvMoveLineId);

        //            // lock InvMove record
        //            var invMove = await InvMoveRepo.LockInvMove(db, invHead.InvMoveId);

        //            if (invMoveDetail != null &&
        //                invMoveDetail.InvMoveLineId == invHead.InvMoveLineId &&
        //                invMove.InvMoveId != null &&
        //                invMove.InvMoveId == invHead.InvMoveId)
        //            {
        //                // check if qty to move is valid
        //                if (invHead.QtyToMove < 1 && invMoveDetail.QtyTo < invHead.QtyToMove)
        //                {
        //                    ret.ResultCode = MovementTaskResultCode.INVALIDQTY;
        //                    return ret;
        //                }

        //                // check header if still in moveable status
        //                var hdrValid = await InvMoveRepo.InvMoveMovable(invHead.InvMoveId);
        //                if (!hdrValid ||
        //                    invMove.InvMoveStatusId == (InvMoveStatus.COMPLETED).ToString() ||
        //                    invMove.InvMoveStatusId == (InvMoveStatus.FRCCLOSED).ToString() ||
        //                    invMove.InvMoveStatusId == (InvMoveStatus.CANCELLED).ToString() ||
        //                    invMove.InvMoveStatusId == (InvMoveStatus.CLOSED).ToString())
        //                {
        //                    ret.ResultCode = MovementTaskResultCode.INVALIDINVMOVE;
        //                    return ret;
        //                }


        //                // check detail itself if still in movable status
        //                var dtlValid = await InvMoveDetailRepo.InvMoveDetailMovable(invHead.InvMoveLineId);
        //                if (!dtlValid)
        //                {
        //                    ret.ResultCode = MovementTaskResultCode.INVALIDINVMOVELANE;
        //                    return ret;
        //                }

        //                // check if invMove detail has movable qty left
        //                int qtyMoved = await GetInvMoveLineMvdQty(db, invMoveDetail.InvMoveLineId);
        //                int availableQty = invMoveDetail.QtyTo - qtyMoved;
        //                int leftOverQty = availableQty - invHead.QtyToMove;

        //                if (availableQty < 1 || availableQty < invHead.QtyToMove)
        //                {
        //                    ret.ResultCode = MovementTaskResultCode.INVALIDQTY;
        //                    return ret;
        //                }

        //                //// check if expiry date is valid
        //                //if (lotAtt.ManufactureDate > DateTime.Now)
        //                //{
        //                //    ret.ResultCode = ReceivingResultCode.LOTINVALIDDATE;
        //                //    return ret;
        //                //}

        //                // update product status if expired
        //                //if (lotAtt.ExpiryDate <= DateTime.Now &&
        //                //    lotAtt.ProductConditionId == (ProductCondition.GOOD).ToString())
        //                //{
        //                //    ret.ResultCode = ReceivingResultCode.INVALIDCONDITION;
        //                //    return ret;
        //                //}

        //                // create inventory detail section

        //                // get inventory next document number
        //                string? invId = await IdNumberRepo.GetNxtDocNum("INV", invHead.UserAccountId);

        //                // get lotattribute next document number (reuse lot id if similar detail exists)
        //                bool createLot = false;
        //                string? lotAttId = await LotAttRepo.GetLotAttributeIdWithSameDetail(lotAtt);
        //                if (string.IsNullOrEmpty(lotAttId))
        //                {
        //                    lotAttId = await IdNumberRepo.GetNxtDocNum("LOT", invHead.UserAccountId);
        //                    createLot = true;
        //                }

        //                // define lotatt detail's lot number
        //                lotAtt.LotAttributeId = lotAttId;

        //                // flag to print lotatt id label
        //                labelsToPrint.Add(new BCodeLabelToPrintModel()
        //                {
        //                    DocType = PrinterDocType.LotAttId,
        //                    DocTypeId = (PrinterDocType.LotAttId).ToString(),
        //                    Description = lotAtt.LotAttributeId,
        //                    BarcodeContent = lotAtt.LotAttributeId,
        //                    EPC = lotAtt.LotAttributeId,
        //                });

        //                // get receiving next document number
        //                string? receivingId = await IdNumberRepo.GetNxtDocNum("RCVING", invHead.UserAccountId);

        //                // get putaway task next document number
        //                string? putawayTaskId = await IdNumberRepo.GetNxtDocNum("PUTAWAY", invHead.UserAccountId);

        //                // get TID To if asterisk/* is provided
        //                string? tidTo = "*";
        //                if (invDetail.TrackIdTo == "*" || string.IsNullOrEmpty(invDetail.TrackIdTo))
        //                {
        //                    tidTo = await IdNumberRepo.GetNxtDocNum("TRACE", invHead.UserAccountId);
        //                    if (tidTo != "*")
        //                    {
        //                        invDetail.TrackIdTo = tidTo;

        //                        // flag to print track id label
        //                        labelsToPrint.Add(new BCodeLabelToPrintModel()
        //                        {
        //                            DocType = PrinterDocType.TrackId,
        //                            DocTypeId = (PrinterDocType.TrackId).ToString(),
        //                            Description = invDetail.TrackIdTo,
        //                            BarcodeContent = invDetail.TrackIdTo,
        //                            EPC = invDetail.TrackIdTo,
        //                        });
        //                    }
        //                    else
        //                    {
        //                        ret.ResultCode = ReceivingResultCode.FAILED;
        //                        return ret;
        //                    }
        //                }

        //                // check if TID is in correct format
        //                var tidPrefix = await IdNumberRepo.GetIdPrefix(db, (TranType.TRACE).ToString());

        //                if (string.IsNullOrEmpty(tidPrefix))
        //                {
        //                    ret.ResultCode = ReceivingResultCode.FAILEDGETDOCIDPREFIX;
        //                    return ret;
        //                }

        //                if (!string.IsNullOrEmpty(tidPrefix))
        //                {
        //                    if (!invDetail.TrackIdTo.Contains(tidPrefix))
        //                    {
        //                        ret.ResultCode = ReceivingResultCode.INVALIDTIDFORMAT;
        //                        return ret;
        //                    }
        //                }

        //                // check if TID has no any record yet
        //                var isCleanTID = await InvHistoryRepo.CheckTrackIdExists(db, invDetail.TrackIdTo);
        //                if (isCleanTID)
        //                {
        //                    ret.ResultCode = ReceivingResultCode.INVALIDTID;
        //                    return ret;
        //                }

        //                // get LPN To if asterisk/* is provided
        //                string? lpnTo = "*";
        //                if (invDetail.LpnTo == "*")
        //                {
        //                    lpnTo = await IdNumberRepo.GetNxtDocNum("LPN", invHead.UserAccountId);
        //                    if (lpnTo != "*")
        //                    {
        //                        invDetail.LpnTo = lpnTo;

        //                        // flag to print lpn id label
        //                        labelsToPrint.Add(new BCodeLabelToPrintModel()
        //                        {
        //                            DocType = PrinterDocType.LPNId,
        //                            DocTypeId = (PrinterDocType.LPNId).ToString(),
        //                            Description = invDetail.LpnTo,
        //                            BarcodeContent = invDetail.LpnTo,
        //                            EPC = invDetail.LpnTo
        //                        });
        //                    }
        //                    else
        //                    {

        //                        ret.ResultCode = ReceivingResultCode.FAILED;
        //                    }
        //                }

        //                // check if LPNTo is not in used by any location except INSTAGING
        //                if (!string.IsNullOrEmpty(invDetail.LpnTo))
        //                {
        //                    var isLPNUsed = await InvHistoryRepo.ChkLPNIsUsedInStorage(db, invDetail.LpnTo);
        //                    if (isLPNUsed)
        //                    {
        //                        ret.ResultCode = ReceivingResultCode.INVALIDLPNID;
        //                        return ret;
        //                    }

        //                    // check if LPN Id is in correct format
        //                    var lpnPrefix = await IdNumberRepo.GetIdPrefix(db, (TranType.LPN).ToString());
        //                    if (string.IsNullOrEmpty(lpnPrefix))
        //                    {
        //                        ret.ResultCode = ReceivingResultCode.FAILEDGETDOCIDPREFIX;
        //                        return ret;
        //                    }

        //                    if (!string.IsNullOrEmpty(lpnPrefix))
        //                    {
        //                        if (!invDetail.LpnTo.Contains(lpnPrefix))
        //                        {
        //                            ret.ResultCode = ReceivingResultCode.INVALIDLPNFORMAT;
        //                            return ret;
        //                        }
        //                    }
        //                }

        //                if (!string.IsNullOrEmpty(invId) &&
        //                    !string.IsNullOrEmpty(lotAttId) &&
        //                    !string.IsNullOrEmpty(receivingId) &&
        //                    !string.IsNullOrEmpty(putawayTaskId))
        //                {
        //                    // build inventory table data
        //                    var inv = new InventoryModel()
        //                    {
        //                        InventoryId = invId,
        //                        Sku = poDetail.Sku,
        //                        InventoryStatusId = (InvStatus.REFERRED).ToString()
        //                    };

        //                    // save header to inventory table
        //                    var invSaved = await InventoryRepo.CreateInventoryMod(db, inv, invHead.UserAccountId, TranType.RCVING);

        //                    if (invSaved)
        //                    {
        //                        // check record conflict on inventoryhistory table --skipped this validation (may not be needed)

        //                        // build inventory history detail defaults
        //                        invDetail.InventoryId = invId;
        //                        invDetail.SeqNum = 1;
        //                        invDetail.DocumentRefId = poDetail.PoLineId;
        //                        invDetail.QtyFrom = 0;
        //                        invDetail.QtyTo = invHead.QtyToReceive;
        //                        invDetail.LocationFrom = "*";
        //                        invDetail.TrackIdFrom = "*";
        //                        invDetail.LpnFrom = "*";
        //                        invDetail.LotAttributeId = lotAttId;
        //                        invDetail.TransactionTypeId = "RCVING";
        //                        invDetail.CreatedBy = invHead.UserAccountId;

        //                        // record lotattribute detail to lotattributedetail table if is new lotId
        //                        bool lotSaved;
        //                        if (createLot)
        //                        {
        //                            lotSaved = await LotAttRepo.CreateLotAttributeDetailMod(db, lotAtt, TranType.RCVING);
        //                        }
        //                        else
        //                        {
        //                            lotSaved = true;
        //                        }

        //                        if (lotSaved)
        //                        {
        //                            // record inventory detail to inventory history table
        //                            bool dtlSaved = await InvHistoryRepo.CreateInventoryHistoryMod(db, invDetail, TranType.RCVING);

        //                            if (dtlSaved)
        //                            {
        //                                // build receiving transaction detail
        //                                var rcvDetail = new ReceivingModel()
        //                                {
        //                                    ReceivingId = receivingId,
        //                                    DocLineId = poDetail.PoLineId,
        //                                    InventoryId = invId,
        //                                    SeqNum = 1,
        //                                    ReceivingStatusId = (ReceivingStatus.CREATED).ToString(),
        //                                    CreatedBy = invHead.UserAccountId,
        //                                    ModifiedBy = invHead.UserAccountId
        //                                };

        //                                // record receiving detail to receving table
        //                                bool rcvSaved = await CreateReceivingMod(db, rcvDetail, TranType.RCVING);

        //                                if (rcvSaved)
        //                                {
        //                                    // build putaway task data
        //                                    var putawayDtl = new PutawayTaskModel()
        //                                    {
        //                                        PutawayTaskId = putawayTaskId,
        //                                        ReceivingId = receivingId,
        //                                        InventoryId = invId,
        //                                        SeqNum = 1,
        //                                        PutawayStatusId = (PutawayStatus.CREATED).ToString(),
        //                                        CreatedBy = invHead.UserAccountId,
        //                                        ModifiedBy = invHead.UserAccountId
        //                                    };

        //                                    // record putaway task to putawaytask table
        //                                    bool putawaySaved = await PutawayTaskRepo.CreatePutawayTaskMod(db, putawayDtl, TranType.RCVING);

        //                                    if (putawaySaved)
        //                                    {
        //                                        // update InvMove detail status
        //                                        poDetail.ModifiedBy = invHead.UserAccountId;
        //                                        if (leftOverQty == 0)
        //                                        {
        //                                            poDetail.PoLineStatusId = (InvMoveLneStatus.FULLRCV).ToString();
        //                                        }
        //                                        else
        //                                        {
        //                                            poDetail.PoLineStatusId = (InvMoveLneStatus.PRTRCV).ToString();
        //                                        }

        //                                        // save updated InvMove detail record
        //                                        var poDtlUpdated = await InvMoveDetailRepo.UpdateInvMoveDetailMod(db, poDetail, TranType.RCVING);
        //                                        if (poDtlUpdated)
        //                                        {
        //                                            // update InvMove status
        //                                            // get InvMove updated status
        //                                            var poStatus = await InvMoveRepo.GetPoUpdatedStatus(db, po.PoId);

        //                                            if (!string.IsNullOrEmpty(poStatus))
        //                                            {
        //                                                // save update po record
        //                                                po.ModifiedBy = invHead.UserAccountId;
        //                                                po.PoStatusId = poStatus;

        //                                                var poUpdated = await InvMoveRepo.UpdateInvMove(db, po, TranType.RCVING);

        //                                                if (poUpdated)
        //                                                {
        //                                                    // record captured serial numbers if there's any
        //                                                    if (tags != null)
        //                                                    {
        //                                                        if (tags.Any())
        //                                                        {
        //                                                            // check if tags count meets to receive qty
        //                                                            if (tags.Count() > invHead.QtyToReceive)
        //                                                            {
        //                                                                ret.ResultCode = ReceivingResultCode.UNIQTAGSEXCEEDSQTYTORCV;
        //                                                                return ret;
        //                                                            }

        //                                                            // record unique tags
        //                                                            var resTagAppend = await UniqueTagsRepo.CreateUniqueTagsMod(db,
        //                                                                                                                        tags,
        //                                                                                                                        tidTo,
        //                                                                                                                        TranType.RCVING,
        //                                                                                                                        po.PoId,
        //                                                                                                                        poDetail.PoLineId,
        //                                                                                                                        invHead.UserAccountId);

        //                                                            // revert if recording of captured serials is failed
        //                                                            if (resTagAppend != RecordTagResultCode.SUCCESS)
        //                                                            {
        //                                                                ret.ResultCode = ReceivingResultCode.UNIQUETAGSAVEFIALED;
        //                                                                return ret;
        //                                                            }

        //                                                            // build EPC zpl ref to print data
        //                                                            var epcs = await PrintHelperRepo.BuildEPCZpls(tags);

        //                                                            // check if return tag sto print data is consistent in tags qty
        //                                                            if (!epcs.Any() || (epcs.Count() != tags.Count()))
        //                                                            {
        //                                                                ret.ResultCode = ReceivingResultCode.SNTOPRINTBUILDFAILED;
        //                                                                return ret;
        //                                                            }

        //                                                            // include EPC data on ZPL data to build
        //                                                            labelsToPrint.AddRange(epcs);
        //                                                        }
        //                                                    }

        //                                                    ret.ResultCode = ReceivingResultCode.SUCCESS;

        //                                                    // build zpl print codes for label printing
        //                                                    if (labelsToPrint.Any())
        //                                                    {
        //                                                        ret.LabelsToPrint = labelsToPrint;

        //                                                        var printData = await PrintHelperRepo.BuildZplDetails(labelsToPrint);
        //                                                        ret.ZplDetails = printData;

        //                                                        return ret;
        //                                                    }
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return ret;
        //}

        public async Task<int> GetInvMoveLineMvdQty(IDbConnection db, string invMoveDetailId)
        {
            string strQry = @"CALL `spGetInvMoveLineMovedQty`(@invMoveDetailId)";

            var param = new DynamicParameters();
            param.Add("@invMoveDetailId", invMoveDetailId);

            return await db.ExecuteScalarAsync<int>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> CreateMovementTask(MovementTaskModel movementTask)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into MovementTask(movementTaskId, 
														docLineId, 
														inventoryId, 
														seqNum, 
                                                        movementStatusId, 
														createdBy, 
														modifiedBy)
 												values(@movementTaskId, 
														@docLineId, 
														@inventoryId, 
														@seqNum, 
                                                        @movementStatusId, 
														@createdBy, 
														@modifiedBy)";

                int res = await db.ExecuteAsync(strQry, movementTask);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateMovementTaskMod(IDbConnection db, MovementTaskModel movementTask, TranType tranTyp)
        {
            string strQry = @"insert into MovementTask(movementTaskId, 
														docLineId, 
														inventoryId, 
														seqNum, 
                                                        movementStatusId, 
														createdBy, 
														modifiedBy)
 												values(@movementTaskId, 
														@docLineId, 
														@inventoryId, 
														@seqNum, 
                                                        @movementStatusId, 
														@createdBy, 
														@modifiedBy)";

            int res = await db.ExecuteAsync(strQry, movementTask);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildMovementTaskAuditADD(movementTask, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateMovementTask(MovementTaskModel movementTask)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update movementtask set 
														docLineId = @docLineId, 
														inventoryId = @inventoryId, 
														seqNum = @seqNum, 
                                                        movementStatusId = @movementStatusId, 
														modifiedBy = @modifiedBy where 
														movementTaskId = @movementTaskId";

                int res = await db.ExecuteAsync(strQry, movementTask);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<CancelMvResultCode> CancelMovementTask(string movementTaskId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // lock current movement detail
                var mvmntTaskDetail = await LockMovementTaskDetail(db, movementTaskId);
                if (mvmntTaskDetail == null)
                {
                    return CancelMvResultCode.MVLOCKFAIL; // cut process here if movement detail is not retreived
                }

                // lock current movement inv move detail
                var invMvDetail = await InvMoveDetailRepo.LockInvMoveDetail(db, mvmntTaskDetail.DocLineId);
                if (invMvDetail == null)
                {
                    return CancelMvResultCode.INVMOVEDETAILLOCKFAIL; // cut process here if po detail is not retreived
                }

                // lock current inv movement invmove header
                var invMoveHeader = await InvMoveRepo.LockInvMove(db, invMvDetail.InvMoveId);
                if (invMoveHeader == null)
                {
                    return CancelMvResultCode.INVMOVELOCKFAIL; // cut process here if po header is not retreived
                }

                // check if movement detail is still moveable
                if (mvmntTaskDetail.MovementStatusId != (MovementTaskStatus.CREATED).ToString())
                {
                    return CancelMvResultCode.MVINCONSISTENCY;
                }

                // check if inv move detail is in movement status
                if (invMvDetail.InvMoveLineStatusId == (InvMoveLneStatus.CREATED).ToString() ||
                    invMvDetail.InvMoveLineStatusId == (InvMoveLneStatus.FRCCLOSED).ToString() ||
                    invMvDetail.InvMoveLineStatusId == (InvMoveLneStatus.CLOSED).ToString())
                {
                    return CancelMvResultCode.INVMOVEDTLINCONSISTENCY;
                }

                // check if invmove is still not closed, cancelled or in create status
                if (invMoveHeader.InvMoveStatusId == (InvMoveStatus.CREATED).ToString() ||
                    invMoveHeader.InvMoveStatusId == (InvMoveStatus.CANCELLED).ToString() ||
                    invMoveHeader.InvMoveStatusId == (InvMoveStatus.FRCCLOSED).ToString() ||
                    invMoveHeader.InvMoveStatusId == (InvMoveStatus.CLOSED).ToString())
                {
                    return CancelMvResultCode.INVMOVEINCONSISTENCY;
                }

                // get most recent inventory history record
                var invHistDetail = await InvHistoryRepo.GetTopInvHistDetail(db, mvmntTaskDetail.InventoryId);

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
                        var invHistSave = await InvHistoryRepo.CreateInventoryHistoryMod(db, invHistDetail, TranType.CANCELMV);

                        if (invHistSave)
                        {
                            // update invtory detail status to closed
                            var invClosed = await InventoryRepo.SetInventoryStatus(db, invHistDetail.InventoryId, (InvStatus.CLOSED).ToString(), userAccountId, TranType.CANCELMV);

                            if (invClosed)
                            {
                                // update movement status to canceled 
                                var rcvClosed = await SetMovementTaskStatus(db, movementTaskId, (MovementTaskStatus.CANCELLED).ToString(), TranType.CANCELRCV, userAccountId);

                                if (rcvClosed)
                                {
                                    // get invmove detail updated line status
                                    var invMoveDtlUpdatedStatus = await InvMoveDetailRepo.GetInvMoveDtlCancelMvUpdatedStatus(db, invMvDetail.InvMoveLineId, movementTaskId);

                                    if (!string.IsNullOrEmpty(invMoveDtlUpdatedStatus))
                                    {
                                        // commit update invmove detail status
                                        invMvDetail.ModifiedBy = userAccountId;
                                        invMvDetail.InvMoveLineStatusId = invMoveDtlUpdatedStatus;
                                        var invMoveDtlUpdated = await InvMoveDetailRepo.UpdateInvMoveDetailMod(db, invMvDetail, TranType.CANCELMV);

                                        if (invMoveDtlUpdated)
                                        {
                                            // get update invmove status
                                            var invMoveUpdatedStatus = await InvMoveRepo.GetInvMoveUpdatedStatus(db, invMoveHeader.InvMoveId);

                                            if (!string.IsNullOrEmpty(invMoveUpdatedStatus))
                                            {
                                                // save update invmove record
                                                invMoveHeader.ModifiedBy = userAccountId;
                                                invMoveHeader.InvMoveStatusId = invMoveUpdatedStatus;

                                                var invMoveUpdated = await InvMoveRepo.UpdateInvMove(db, invMoveHeader, TranType.CANCELMV);

                                                if (invMoveUpdated)
                                                {
                                                    return CancelMvResultCode.SUCCESS;
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

            return CancelMvResultCode.FAILED;
        }

        public async Task<MovementTaskModel> LockMovementTaskDetail(IDbConnection db, string movementTaskId)
        {
            // hold current movement transaction
            string strQry = @"select * from movementtask where movementTaskId = @movementTaskId for update;";

            var param = new DynamicParameters();
            param.Add("@movementTaskId", movementTaskId);

            return await db.QuerySingleOrDefaultAsync<MovementTaskModel>(strQry, param);
        }

        public async Task<bool> SetMovementTaskStatus(string movementTaskId, string movementStatusId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update movementtask set movementStatusId = @movementStatusId 
                                    where movementTaskId = @movementTaskId;";

                var param = new DynamicParameters();
                param.Add("@movementStatusId", movementStatusId);
                param.Add("@movementTaskId", movementTaskId);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> SetMovementTaskStatus(IDbConnection db, string movementTaskId, string movementStatusId, TranType tranTyp, string userAccountId)
        {
            string strQry = @"update movementtask set movementStatusId = @movementStatusId, 
                                                    modifiedBy = @modifiedBy
                                    where movementTaskId = @movementTaskId;";

            var param = new DynamicParameters();
            param.Add("@movementStatusId", movementStatusId);
            param.Add("@modifiedBy", userAccountId);
            param.Add("@movementTaskId", movementTaskId);

            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                // get updated movement record
                var mvment = await GetMovementTaskByIdMod(db, movementTaskId);

                if (mvment != null)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(mvment, tranTyp);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> DeleteMovementTask(string movementTaskId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from MovementTask where 
														movementTaskId = @movementTaskId";
                var param = new DynamicParameters();
                param.Add("@movementTaskId", movementTaskId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<MovementTaskModel> LockMovementTaskDetailRefMulti(IDbConnection db, string invMoveLineId, string inventoryId)
        {
            // hold current movement transaction
            string strQry = @"select * 
                                from movementtask 
                                where docLineId = @invMoveLineId and 
                                        inventoryId = @inventoryId and 
                                        seqnum = 1 and 
                                        movementStatusId = 'CREATED' 
                                for update;";

            var param = new DynamicParameters();
            param.Add("@docLineId", invMoveLineId);
            param.Add("@inventoryId", inventoryId);

            return await db.QuerySingleOrDefaultAsync<MovementTaskModel>(strQry, param);
        }

        public async Task<bool> HasPendingMovementTask(IDbConnection db, string invMoveId)
        {
            string strQry = @"CALL `spCountInvMovePendingMovement`(@currInvMoveId);";

            var param = new DynamicParameters();
            param.Add("@currInvMoveId", invMoveId);

            int res = await db.ExecuteScalarAsync<int>(strQry, param);

            if (res == 0)
            {
                return false;
            }

            return true;
        }

        public Task<MovementTaskResultModel> ProceedMovementTask(CommitMovementTaskModel data)
        {
            throw new NotImplementedException();
        }

        public Task<MovementTaskResultCode> PartialMovement(IDbConnection db, MovementContainerModel data)
        {
            throw new NotImplementedException();
        }

        public Task<MovementTaskResultCode> FullMovementInvMove(IDbConnection db, MovementContainerModel data)
        {
            throw new NotImplementedException();
        }

        public Task<string?> DefineTranTypeByDocId(IDbConnection db, string docLineId)
        {
            throw new NotImplementedException();
        }
    }
}
