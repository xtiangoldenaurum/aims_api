using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Repositories.Sub;
using aims_api.Utilities.Interface;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class PutawayTaskRepository : IPutawayTaskRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        IInventoryHistoryRepository InvHistoryRepo;
        IInventoryRepository InventoryRepo;
        PutawayTaskAudit AuditBuilder;
        IProductRepository ProductRepo;
        ILotAttributeDetailRepository LotAttRepo;
        IPODetailRepository PODetailRepo;
        IReturnsDetailRepository RetDetailRepo;
        IWhTransferDetailRepository WhTransDetailRepo;
        IIdNumberRepository IdNumberRepo;
        PORepoSub PORepo;
        ReturnsRepoSub ReturnsRepo;
        WhTransferRepoSub WhTransferRepo;
        ReceivingRepoSub ReceivingRepo;
        ILocationRepository LocationRepo;

        public PutawayTaskRepository(ITenantProvider tenantProvider,
                                        IAuditTrailRepository auditTrailRepo,
                                        IInventoryHistoryRepository invHistoryRepo,
                                        IInventoryRepository inventoryRepo,
                                        IProductRepository productRepo,
                                        ILotAttributeDetailRepository lotAttRepo,
                                        IPODetailRepository poDetailRepo,
                                        IReturnsDetailRepository retDetailRepo,
                                        IWhTransferDetailRepository whTransDetailRepo,
                                        IIdNumberRepository idNumberRepo,
                                        PORepoSub poRepo,
                                        ReturnsRepoSub returnsRepo,
                                        WhTransferRepoSub whTransferRepo,
                                        ReceivingRepoSub receivingRepo,
                                        ILocationRepository locationRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            IdNumberRepo = idNumberRepo;
            AuditTrailRepo = auditTrailRepo;
            InvHistoryRepo = invHistoryRepo;
            InventoryRepo = inventoryRepo;
            ProductRepo = productRepo;
            LotAttRepo = lotAttRepo;
            PODetailRepo = poDetailRepo;
            RetDetailRepo = retDetailRepo;
            WhTransDetailRepo = whTransDetailRepo;
            PORepo = poRepo;
            ReturnsRepo = returnsRepo;
            WhTransferRepo = whTransferRepo;
            ReceivingRepo = receivingRepo;
            LocationRepo = locationRepo;
            AuditBuilder = new PutawayTaskAudit();
        }

        public async Task<IEnumerable<PutawayTaskModel>> GetPutawayTaskPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from PutawayTask limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<PutawayTaskModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<PutawayTaskModel>> GetPutawayTaskPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from PutawayTask where 
														putawayTaskId like @searchKey or 
														receivingId like @searchKey or 
														inventoryId like @searchKey or 
														seqNum like @searchKey or 
                                                        putawayStatusId like @putawayStatusId or
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or
														modifiedBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<PutawayTaskModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<PutawayTaskModel> GetPutawayTaskById(string putawayTaskId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from PutawayTask where 
														putawayTaskId = @putawayTaskId";

                var param = new DynamicParameters();
                param.Add("@putawayTaskId", putawayTaskId);
                return await db.QuerySingleOrDefaultAsync<PutawayTaskModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<PutawayTaskModel> GetPutawayTaskByIdMod(IDbConnection db, string putawayTaskId)
        {
            string strQry = @"select * from PutawayTask where 
														putawayTaskId = @putawayTaskId";

            var param = new DynamicParameters();
            param.Add("@putawayTaskId", putawayTaskId);

            return await db.QuerySingleOrDefaultAsync<PutawayTaskModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> PutawayTaskExists(string putawayTaskId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select putawayTaskId from PutawayTask where 
														putawayTaskId = @putawayTaskId";

                var param = new DynamicParameters();
                param.Add("@putawayTaskId", putawayTaskId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<PutawayTaskModel> LockPutawayTaskDtl(IDbConnection db, string receivingId)
        {
            // hold currentr eceive transaction
            string strQry = @"select * from putawaytask where receivingId = @receivingId and seqNum = 1 for update;";

            var param = new DynamicParameters();
            param.Add("@receivingId", receivingId);

            return await db.QuerySingleOrDefaultAsync<PutawayTaskModel>(strQry, param);
        }

        public async Task<bool> CreatePutawayTask(PutawayTaskModel putawayTask)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into PutawayTask(putawayTaskId, 
														receivingId, 
														inventoryId, 
														seqNum, 
                                                        putawayStatusId, 
														createdBy,
                                                        modifiedBy)
 												values(@putawayTaskId, 
														@receivingId, 
														@inventoryId, 
														@seqNum, 
                                                        @putawayStatusId, 
														@createdBy, 
                                                        @modifiedBy)";

                int res = await db.ExecuteAsync(strQry, putawayTask);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreatePutawayTaskMod(IDbConnection db, PutawayTaskModel putawayTask, TranType tranTyp)
        {
            string strQry = @"insert into PutawayTask(putawayTaskId, 
														receivingId, 
														inventoryId, 
														seqNum, 
                                                        putawayStatusId, 
														createdBy, 
                                                        modifiedBy)
 												values(@putawayTaskId, 
														@receivingId, 
														@inventoryId, 
														@seqNum, 
                                                        @putawayStatusId, 
														@createdBy, 
                                                        @modifiedBy)";

            int res = await db.ExecuteAsync(strQry, putawayTask);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildPutawayAuditADD(putawayTask, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdatePutawayTask(PutawayTaskModel putawayTask)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update PutawayTask set 
														receivingId = @receivingId, 
														inventoryId = @inventoryId, 
														seqNum = @seqNum, 
                                                        putawayStatusId = @putawayStatusId where 
														putawayTaskId = @putawayTaskId";

                int res = await db.ExecuteAsync(strQry, putawayTask);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> SetPutawayStatus(string putawayTaskId, string putawaytStatusId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update putawaytask set putawaytStatusId = @putawaytStatusId, 
                                    modifiedBy = @modifiedBy 
                                    where putawayTaskId = @putawayTaskId;";

                var param = new DynamicParameters();
                param.Add("@putawaytStatusId", putawaytStatusId);
                param.Add("@modifiedBy", userAccountId);
                param.Add("@putawayTaskId", putawayTaskId);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> SetPutawayStatus(IDbConnection db, string putawayTaskId, string putawayStatusId, string userAccountId, TranType tranTyp)
        {
            string strQry = @"update putawaytask set putawayStatusId = @putawayStatusId, 
                                    modifiedBy = @modifiedBy 
                                    where putawayTaskId = @putawayTaskId;";

            var param = new DynamicParameters();
            param.Add("@putawayStatusId", putawayStatusId);
            param.Add("@modifiedBy", userAccountId);
            param.Add("@putawayTaskId", putawayTaskId);

            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                // get updated putaway task
                var putawayTask = await GetPutawayTaskByIdMod(db, putawayTaskId);

                if (putawayTask != null)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(putawayTask, tranTyp);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeletePutawayTask(string putawayTaskId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from PutawayTask where 
														putawayTaskId = @putawayTaskId";
                var param = new DynamicParameters();
                param.Add("@putawayTaskId", putawayTaskId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> HasPendingPutawayTaskPO(IDbConnection db, string poId)
        {
            string strQry = @"CALL `spCountPOPendingPutaway`(@currPOId);";

            var param = new DynamicParameters();
            param.Add("@currPOId", poId);

            int res = await db.ExecuteScalarAsync<int>(strQry, param);

            if (res == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> HasPendingPutawayTaskRet(IDbConnection db, string returnsId)
        {
            string strQry = @"CALL `spCountRetPendingPutaway`(@currRetId);";

            var param = new DynamicParameters();
            param.Add("@currRetId", returnsId);

            int res = await db.ExecuteScalarAsync<int>(strQry, param);

            if (res == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<PutawayResultModel> PutawayQryTIDDetails(string trackId, string userAccountId)
        {
            // init return object
            var ret = new PutawayResultModel()
            {
                ResultCode = PutawayResultCode.FAILED,
            };

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // get inventory history detail by scanned track id and lock
                var invDetail = await InvHistoryRepo.GetInvHistoryByTrackId(db, trackId);
                if (invDetail == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKINVHIST;
                    return ret;
                }

                // get and lock inventory header
                var invHead = await InventoryRepo.LockInventoryByInvId(db, invDetail.InventoryId);
                if (invHead == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKINVHEAD;
                    return ret;
                }

                // get and lock linked receiving tran
                // get actual receiving service from Func<> service (bypassed circular dependency)
                // var receivingRepo = ReceivingRepo();
                var rcvDetail = await ReceivingRepo.LockReceiveDetailRefMulti(db, invDetail.DocumentRefId, invDetail.InventoryId);
                if (rcvDetail == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKRCVDETAILS;
                    return ret;
                }

                // get and lock putaway task
                var putawayDtl = await LockPutawayTaskDtl(db, rcvDetail.ReceivingId);
                if (putawayDtl == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKPTask;
                    return ret;
                }

                // check if putawat task is still in create status // ignore warning here
                if (putawayDtl.PutawayStatusId != (PutawayStatus.CREATED).ToString())
                {
                    ret.ResultCode = PutawayResultCode.INVALIDPUTAWAYTASKSTATUS;
                    return ret;
                }

                // get product lot attributes
                var lotAtt = await LotAttRepo.GetLotAttributeDetailById(invDetail.LotAttributeId);
                if (lotAtt == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOGETLOTATT;
                    return ret;
                }

                // process details to be displayed on handheld putaway window 1

                // get sku product description // ignore warning here
                var prodDetails = await ProductRepo.GetProductById(invHead.Sku);
                if (prodDetails == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOGETPRODDETAILS;
                    return ret;
                }

                // build details for putaway window 1 display
                var putawayWinOne = new PutawayWinOneModel()
                {
                    TargetTrackId = invDetail.TrackIdTo,
                    Sku = invHead.Sku,
                    Barcode = prodDetails.Barcode,
                    Barcode2 = prodDetails.Barcode2,
                    Barcode3 = prodDetails.Barcode3,
                    Barcode4 = prodDetails.Barcode4,
                    ProductName = prodDetails.ProductName,
                    CurrentQty = invDetail.QtyTo,
                    CurrentLocation = invDetail.LocationTo,
                    CurrentLPN = invDetail.LpnTo,
                    ManufactureDate = lotAtt.ManufactureDate,
                    ExpiryDate = lotAtt.ExpiryDate,
                    WarehousingDate = lotAtt.WarehousingDate,
                    ProductConditionId = lotAtt.ProductConditionId, 
                    UomDisplay = invDetail.UomDisplay
                };

                // details for putaway window 2 is null by default

                var data = new PutawayTaskProcModel()
                {
                    PutawayWinOne = putawayWinOne
                };

                // append success result code and data to ret object
                ret.ResultCode = PutawayResultCode.SUCCESS;
                ret.Data = data;
            }

            return ret;
        }

        // this function repeats validations from "PutawayQryTIDDetails" function to ensure data consistency
        public async Task<PutawayResultModel> CommitPutaway(PutawayTaskProcModel data)
        {
            // init return object
            var ret = new PutawayResultModel()
            {
                ResultCode = PutawayResultCode.FAILED,
            };

            // split data
            var winOneData = data.PutawayWinOne;
            var winTwoData = data.PutawayWinTwo;

            if (winOneData != null && winTwoData != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // check if current and target location is not the same
                    if (winOneData.CurrentLocation == winTwoData.PutawayLocation)
                    {
                        ret.ResultCode = PutawayResultCode.TARGETLOCCONFLICT;
                        return ret;
                    }

                    // define if putaway if partial or full
                    bool isPartial;
                    var qtyLeft = winOneData.CurrentQty - winTwoData.PutawayQty;

                    if (qtyLeft == 0)
                    {
                        isPartial = false;
                    }
                    else if (qtyLeft > 0)
                    {
                        isPartial = true;
                    }
                    else
                    {
                        // exit process due to putaway qty exceeds original
                        ret.ResultCode = PutawayResultCode.QTYEXCEEDS;
                        return ret;
                    }

                    // get inventory history detail by scanned track id and lock
                    var invDetail = await InvHistoryRepo.GetInvHistoryByTrackId(db, winOneData.TargetTrackId);
                    if (invDetail == null)
                    {
                        ret.ResultCode = PutawayResultCode.FAILEDTOGETINVHIST;
                        return ret;
                    }

                    var invDtlLock = await InvHistoryRepo.LockInvHistDetail(db, invDetail.InventoryId, invDetail.SeqNum);
                    if (invDtlLock == null)
                    {
                        ret.ResultCode = PutawayResultCode.FAILEDTOLOCKINVHIST;
                        return ret;
                    }

                    // get and lock inventory header
                    var invHead = await InventoryRepo.LockInventoryByInvId(db, invDetail.InventoryId);
                    if (invHead == null)
                    {
                        ret.ResultCode = PutawayResultCode.FAILEDTOLOCKINVHEAD;
                        return ret;
                    }

                    // get and lock linked receiving tran
                    // get actual receiving service from Func<> service (bypassed circular dependency)
                    //var receivingRepo = ReceivingRepo();
                    var rcvDetail = await ReceivingRepo.LockReceiveDetailRefMulti(db, invDetail.DocumentRefId, invDetail.InventoryId);
                    if (rcvDetail == null)
                    {
                        ret.ResultCode = PutawayResultCode.FAILEDTOLOCKRCVDETAILS;
                        return ret;
                    }

                    // get and lock putaway task
                    var putawayDtl = await LockPutawayTaskDtl(db, rcvDetail.ReceivingId);
                    if (putawayDtl == null)
                    {
                        ret.ResultCode = PutawayResultCode.FAILEDTOLOCKPTask;
                        return ret;
                    }

                    // check if putaway task is still in create status // ignore warning here
                    if (putawayDtl.PutawayStatusId != (PutawayStatus.CREATED).ToString())
                    {
                        ret.ResultCode = PutawayResultCode.INVALIDPUTAWAYTASKSTATUS;
                        return ret;
                    }

                    // get product lot attributes
                    var lotAtt = await LotAttRepo.GetLotAttributeDetailById(invDetail.LotAttributeId);
                    if (lotAtt == null)
                    {
                        ret.ResultCode = PutawayResultCode.FAILEDTOGETLOTATT;
                        return ret;
                    }

                    // get linked PO detail then lock

                    // process details to be displayed on handheld putaway window 1

                    // get sku product description // ignore warning here
                    var prodDetails = await ProductRepo.GetProductById(invHead.Sku);
                    if (prodDetails == null)
                    {
                        ret.ResultCode = PutawayResultCode.FAILEDTOGETPRODDETAILS;
                        return ret;
                    }

                    // build details for putaway window 1 display
                    var putawayWinOne = new PutawayWinOneModel()
                    {
                        TargetTrackId = invDetail.TrackIdTo,
                        Sku = invHead.Sku,
                        Barcode = prodDetails.Barcode,
                        Barcode2 = prodDetails.Barcode2,
                        Barcode3 = prodDetails.Barcode3,
                        Barcode4 = prodDetails.Barcode4,
                        ProductName = prodDetails.ProductName,
                        CurrentQty = invDetail.QtyTo,
                        CurrentLocation = invDetail.LocationTo,
                        CurrentLPN = invDetail.LpnTo,
                        ManufactureDate = lotAtt.ManufactureDate,
                        ExpiryDate = lotAtt.ExpiryDate,
                        WarehousingDate = lotAtt.WarehousingDate,
                        ProductConditionId = lotAtt.ProductConditionId,
                        UomDisplay = invDetail.UomDisplay
                    };

                    // compare putaway prev. and current putaway win data
                    if (!winOneData.Equals(putawayWinOne))
                    {
                        ret.ResultCode = PutawayResultCode.RECORDINCONSISTENCY;
                        return ret;
                    }

                    // check if LPNTo is not in used by any location except INSTAGING
                    if (!string.IsNullOrEmpty(winTwoData.PutawayLPN))
                    {
                        var isLPNUsed = await InvHistoryRepo.ChkLPNIsUsedInStorage(db, winTwoData.PutawayLPN);
                        if (isLPNUsed)
                        {
                            ret.ResultCode = PutawayResultCode.LPNISALREADYUSED;
                            return ret;
                        }

                        // check if LPN Id is in correct format
                        var lpnPrefix = await IdNumberRepo.GetIdPrefix(db, (TranType.LPN).ToString());
                        if (!string.IsNullOrEmpty(lpnPrefix))
                        {
                            if (!winTwoData.PutawayLPN.Contains(lpnPrefix))
                            {
                                ret.ResultCode = PutawayResultCode.INVALIDLPNFORMAT;
                                return ret;
                            }
                        }
                    }

                    // check if pallet is same on current receiving pallet
                    if (!string.IsNullOrEmpty(winTwoData.PutawayLPN))
                    {
                        if (winOneData.CurrentLPN == winTwoData.PutawayLPN)
                        {
                            ret.ResultCode = PutawayResultCode.CANNOTUSESAMELPN;
                            return ret;
                        }
                    }

                    // re-validate target putaway location
                    var targetLocChk = await LocationRepo.DefineTargetLocByLocId(winTwoData.PutawayLocation);
                    if (targetLocChk.ResultCode != PutawayResultCode.SUCCESS)
                    {
                        ret.ResultCode = targetLocChk.ResultCode;
                        return ret;
                    }

                    // location LPN insist condition checking
                    if (targetLocChk.TargetLoc != null)
                    {
                        var insistedLPNTO = targetLocChk.TargetLoc.LPNTo;
                        if (!string.IsNullOrEmpty(insistedLPNTO))
                        {
                            if (insistedLPNTO != winTwoData.PutawayLPN)
                            {
                                ret.ResultCode = PutawayResultCode.INVALIDLPNTO;
                                return ret;
                            }
                        }
                    }

                    // define if its either PO, Returns or Transfer type putaway
                    var docTypeOrigin = await DefineTranTypeByDocId(db, invDetail.DocumentRefId);
                    if (string.IsNullOrEmpty(docTypeOrigin) || docTypeOrigin == "ERR")
                    {
                        ret.ResultCode = PutawayResultCode.FAILEDTODEFINEDOCORIGINTYPE;
                        return ret;
                    }

                    // build putaway details container object
                    var container = new PutawayContainerModel()
                    {
                        Sku = invHead.Sku,
                        ReceivingId = rcvDetail.ReceivingId,
                        PutawayTaskId = putawayDtl.PutawayTaskId,
                        UserAccountId = data.UserAccountId,
                        InvHistory = invDetail,
                        WinData = data,
                        LotAtt = lotAtt
                    };

                    // init final result holder
                    var tempResult = PutawayResultCode.FAILED;

                    if (!isPartial)
                    {
                        // proceed to PO full putaway
                        tempResult = await ProceedFullPutawayByDoc(db, container, docTypeOrigin);
                    }
                    else
                    {
                        // lock parent documents first
                        var docLockRes = await PartialPutawayDocLocker(db, docTypeOrigin, invDetail.DocumentRefId);
                        if (docLockRes != PutawayResultCode.SUCCESS)
                        {
                            ret.ResultCode = docLockRes;
                            return ret;
                        }

                        // proceed partial putaway
                        tempResult = await PartialPutaway(db, container);
                    }

                    ret.ResultCode = tempResult;

                    // check tempresult status
                    if (tempResult == PutawayResultCode.SUCCESS)
                    {
                        // append updated data on return object
                        ret.Data = data;
                    }
                }
            }

            return ret;
        }

        public async Task<PutawayResultCode> FullPutawayPO(IDbConnection db, PutawayContainerModel data)
        {
            // build inventory history next sequence
            if (data.InvHistory != null
                && data.WinData != null
                && data.WinData.PutawayWinOne != null
                && data.WinData.PutawayWinTwo != null
                && data.LotAtt != null)
            {
                // init data
                var putawayWinOne = data.WinData.PutawayWinOne;
                var putawayWinTwo = data.WinData.PutawayWinTwo;
                var invHistory = data.InvHistory;

                // build inventory history succeeding sequnce record
                invHistory.SeqNum += 1;
                invHistory.QtyFrom = putawayWinOne.CurrentQty;
                invHistory.QtyTo = putawayWinTwo.PutawayQty;
                invHistory.LpnFrom = invHistory.LpnTo;
                invHistory.LpnTo = putawayWinTwo.PutawayLPN;
                invHistory.LocationFrom = invHistory.LocationFrom;
                invHistory.LocationTo = putawayWinTwo.PutawayLocation;
                invHistory.TransactionTypeId = (TranType.PUTAWAY).ToString();
                invHistory.CreatedBy = data.UserAccountId;

                // get and lock PO linked detail
                var poDtlLockRes = await LockPODetail(db, invHistory.DocumentRefId);
                if (poDtlLockRes.ResultCode != PutawayResultCode.SUCCESS ||
                    poDtlLockRes.PODetail == null)
                {
                    return poDtlLockRes.ResultCode;
                }

                // set PO Detail
                var poDetail = poDtlLockRes.PODetail;

                // get and lock linked PO
                var poHeadLockRes = await LockPOHeader(db, poDetail.PoId);
                if (poHeadLockRes.ResultCode != PutawayResultCode.SUCCESS ||
                    poHeadLockRes.POHeader == null)
                {
                    return poHeadLockRes.ResultCode;
                }

                // set PO header
                var poHeader = poHeadLockRes.POHeader;

                // insert new sequence into inventory history table
                var invHistSaved = await InvHistoryRepo.CreateInventoryHistoryMod(db, invHistory, TranType.PUTAWAY);
                if (!invHistSaved)
                {
                    return PutawayResultCode.INVHISTCOMMITFAILED;
                }

                // update inventory header status into available if product condition is good
                if (data.LotAtt.ProductConditionId == (ProductCondition.GOOD).ToString())
                {
                    var invStatusUpdated = await InventoryRepo.SetInventoryStatus(db,
                                                                                    invHistory.InventoryId,
                                                                                    (InvStatus.AVAILABLE).ToString(),
                                                                                    data.UserAccountId,
                                                                                    TranType.PUTAWAY);
                    if (!invStatusUpdated)
                    {
                        return PutawayResultCode.INVSTATUSUPDATEFAILED;
                    }
                }

                // close receiving status
                var rcvStatusUpdate = await ReceivingRepo.SetReceivingStatus(db,
                                                                                data.ReceivingId,
                                                                                (ReceivingStatus.CLOSED).ToString(),
                                                                                TranType.PUTAWAY,
                                                                                data.UserAccountId);
                if (!rcvStatusUpdate)
                {
                    return PutawayResultCode.RCVSTATUSUPDATEFAILED;
                }

                // close putaway task status
                var putawayStatusUpdt = await SetPutawayStatus(db,
                                                                            data.PutawayTaskId,
                                                                            (PutawayStatus.CLOSED).ToString(),
                                                                            data.UserAccountId,
                                                                            TranType.PUTAWAY);
                if (!putawayStatusUpdt)
                {
                    return PutawayResultCode.PUTAWAYSTATUSUPDATEFAILED;
                }

                // get update PO header status id
                var updatePOStatus = await PORepo.GetPoUpdatedStatus(db, poHeader.PoId);
                if (string.IsNullOrEmpty(updatePOStatus))
                {
                    return PutawayResultCode.FAILEDGETUPDATEPOSTATUS;
                }

                // check if PO is complete base on its updated PO status id
                if (updatePOStatus == (POStatus.FULLRCV).ToString())
                {
                    // check PO details if there's no pending putaway task
                    var hasPutawayTask = await HasPendingPutawayTaskPO(db, poHeader.PoId);

                    // close PO and PO details status if there's no pending receive and putaway
                    if (!hasPutawayTask)
                    {
                        // update PO status
                        poHeader.PoStatusId = (POStatus.CLOSED).ToString();
                        poHeader.ModifiedBy = data.UserAccountId;

                        var poClosed = await PORepo.UpdatePO(db, poHeader, TranType.PUTAWAY);
                        if (!poClosed)
                        {
                            return PutawayResultCode.FAILEDTOUPDATEPOSTATUS;
                        }

                        // retreive all PO Details to close each
                        var poDetails = await PODetailRepo.LockPODetails(db, poHeader.PoId);
                        if (!poDetails.Any())
                        {
                            return PutawayResultCode.FAILEDTOUPDATEPODTLSTATUS;
                        }

                        // update each PO detail status
                        foreach (var poDtl in poDetails)
                        {
                            poDtl.PoLineStatusId = (POLneStatus.CLOSED).ToString();
                            poDtl.ModifiedBy = (data.UserAccountId).ToString();

                            var poDtlClosed = await PODetailRepo.UpdatePODetailMod(db, poDtl, TranType.PUTAWAY);

                            if (!poDtlClosed)
                            {
                                return PutawayResultCode.FAILEDTOUPDATEPODTLSTATUS;
                            }
                        }

                        return PutawayResultCode.SUCCESS;
                    }
                }

                // simply update PO header status into its updated one
                poHeader.PoStatusId = updatePOStatus;
                poHeader.ModifiedBy = data.UserAccountId;

                var poStatusUpdated = await PORepo.UpdatePO(db, poHeader, TranType.PUTAWAY);
                if (!poStatusUpdated)
                {
                    return PutawayResultCode.FAILEDTOUPDATEPOSTATUS;
                }
                else
                {
                    return PutawayResultCode.SUCCESS;
                }
            }

            return PutawayResultCode.FAILED;
        }

        public async Task<PutawayResultCode> FullPutawayReturns(IDbConnection db, PutawayContainerModel data)
        {
            // build inventory history next sequence
            if (data.InvHistory != null
                && data.WinData != null
                && data.WinData.PutawayWinOne != null
                && data.WinData.PutawayWinTwo != null
                && data.LotAtt != null)
            {
                // init data
                var putawayWinOne = data.WinData.PutawayWinOne;
                var putawayWinTwo = data.WinData.PutawayWinTwo;
                var invHistory = data.InvHistory;

                // build inventory history succeeding sequnce record
                invHistory.SeqNum += 1;
                invHistory.QtyFrom = putawayWinOne.CurrentQty;
                invHistory.QtyTo = putawayWinTwo.PutawayQty;
                invHistory.LpnFrom = invHistory.LpnTo;
                invHistory.LpnTo = putawayWinTwo.PutawayLPN;
                invHistory.LocationFrom = invHistory.LocationFrom;
                invHistory.LocationTo = putawayWinTwo.PutawayLocation;
                invHistory.TransactionTypeId = (TranType.PUTAWAY).ToString();
                invHistory.CreatedBy = data.UserAccountId;

                // get and lock returns linked detail
                var retDtlLockRes = await LockReturnsDetail(db, invHistory.DocumentRefId);
                if (retDtlLockRes.ResultCode != PutawayResultCode.SUCCESS ||
                    retDtlLockRes.ReturnsDetail == null)
                {
                    return retDtlLockRes.ResultCode;
                }

                // set returns Detail
                var retDetail = retDtlLockRes.ReturnsDetail;

                // get and lock linked returns
                var retHeadLockRes = await LockReturnsHeader(db, retDetail.ReturnsId);
                if (retHeadLockRes.ResultCode != PutawayResultCode.SUCCESS ||
                    retHeadLockRes.ReturnsHeader == null)
                {
                    return retHeadLockRes.ResultCode;
                }

                // set returns header
                var retHeader = retHeadLockRes.ReturnsHeader;

                // insert new sequence into inventory history table
                var invHistSaved = await InvHistoryRepo.CreateInventoryHistoryMod(db, invHistory, TranType.PUTAWAY);
                if (!invHistSaved)
                {
                    return PutawayResultCode.INVHISTCOMMITFAILED;
                }

                // update inventory header status into available if product condition is good
                if (data.LotAtt.ProductConditionId == (ProductCondition.GOOD).ToString())
                {
                    var invStatusUpdated = await InventoryRepo.SetInventoryStatus(db,
                                                                                    invHistory.InventoryId,
                                                                                    (InvStatus.AVAILABLE).ToString(),
                                                                                    data.UserAccountId,
                                                                                    TranType.PUTAWAY);
                    if (!invStatusUpdated)
                    {
                        return PutawayResultCode.INVSTATUSUPDATEFAILED;
                    }
                }

                // close receiving status
                var rcvStatusUpdate = await ReceivingRepo.SetReceivingStatus(db,
                                                                                data.ReceivingId,
                                                                                (ReceivingStatus.CLOSED).ToString(),
                                                                                TranType.PUTAWAY,
                                                                                data.UserAccountId);
                if (!rcvStatusUpdate)
                {
                    return PutawayResultCode.RCVSTATUSUPDATEFAILED;
                }

                // close putaway task status
                var putawayStatusUpdt = await SetPutawayStatus(db,
                                                                            data.PutawayTaskId,
                                                                            (PutawayStatus.CLOSED).ToString(),
                                                                            data.UserAccountId,
                                                                            TranType.PUTAWAY);
                if (!putawayStatusUpdt)
                {
                    return PutawayResultCode.PUTAWAYSTATUSUPDATEFAILED;
                }

                // get update Returns header status id
                var updateRetStatus = await ReturnsRepo.GetReturnsUpdatedStatus(db, retHeader.ReturnsId);
                if (string.IsNullOrEmpty(updateRetStatus))
                {
                    return PutawayResultCode.FAILEDGETUPDATERETSTATUS;
                }

                // check if Returns is complete base on its updated Returns status id
                if (updateRetStatus == (POStatus.FULLRCV).ToString())
                {
                    // check returns details if there's no pending putaway task
                    var hasPutawayTask = await HasPendingPutawayTaskRet(db, retHeader.ReturnsId);

                    // close returns and returns details status if there's no pending receive and putaway
                    if (!hasPutawayTask)
                    {
                        // update Returns status
                        retHeader.ReturnsStatusId = (POStatus.CLOSED).ToString();
                        retHeader.ModifiedBy = data.UserAccountId;

                        var retClosed = await ReturnsRepo.UpdateReturns(db, retHeader, TranType.PUTAWAY);
                        if (!retClosed)
                        {
                            return PutawayResultCode.FAILEDTOUPDATERETSTATUS;
                        }

                        // retreive all returns Details to close each
                        var retDetails = await RetDetailRepo.LockReturnsDetails(db, retHeader.ReturnsId);
                        if (!retDetails.Any())
                        {
                            return PutawayResultCode.FAILEDTOUPDATERETDTLSTATUS;
                        }

                        // update each returns detail status
                        foreach (var retDtl in retDetails)
                        {
                            retDtl.ReturnsLineStatusId = (POLneStatus.CLOSED).ToString();
                            retDtl.ModifiedBy = (data.UserAccountId).ToString();

                            var retDtlClosed = await RetDetailRepo.UpdateReturnsDetailMod(db, retDtl, TranType.PUTAWAY);

                            if (!retDtlClosed)
                            {
                                return PutawayResultCode.FAILEDTOUPDATERETDTLSTATUS;
                            }
                        }

                        return PutawayResultCode.SUCCESS;
                    }
                }

                // simply update returns header status into its updated one
                retHeader.ReturnsStatusId = updateRetStatus;
                retHeader.ModifiedBy = data.UserAccountId;

                var retStatusUpdated = await ReturnsRepo.UpdateReturns(db, retHeader, TranType.PUTAWAY);
                if (!retStatusUpdated)
                {
                    return PutawayResultCode.FAILEDTOUPDATERETSTATUS;
                }
                else
                {
                    return PutawayResultCode.SUCCESS;
                }
            }

            return PutawayResultCode.FAILED;
        }

        public async Task<PutawayResultCode> FullPutawayWhTrans(IDbConnection db, PutawayContainerModel data)
        {
            // build inventory history next sequence
            if (data.InvHistory != null
                && data.WinData != null
                && data.WinData.PutawayWinOne != null
                && data.WinData.PutawayWinTwo != null
                && data.LotAtt != null)
            {
                // init data
                var putawayWinOne = data.WinData.PutawayWinOne;
                var putawayWinTwo = data.WinData.PutawayWinTwo;
                var invHistory = data.InvHistory;

                // build inventory history succeeding sequnce record
                invHistory.SeqNum += 1;
                invHistory.QtyFrom = putawayWinOne.CurrentQty;
                invHistory.QtyTo = putawayWinTwo.PutawayQty;
                invHistory.LpnFrom = invHistory.LpnTo;
                invHistory.LpnTo = putawayWinTwo.PutawayLPN;
                invHistory.LocationFrom = invHistory.LocationFrom;
                invHistory.LocationTo = putawayWinTwo.PutawayLocation;
                invHistory.TransactionTypeId = (TranType.PUTAWAY).ToString();
                invHistory.CreatedBy = data.UserAccountId;

                // get and lock wh transfer linked detail
                var whTransDtlLockRes = await LockWhTransDetail(db, invHistory.DocumentRefId);
                if (whTransDtlLockRes.ResultCode != PutawayResultCode.SUCCESS ||
                    whTransDtlLockRes.WhTransDetail == null)
                {
                    return whTransDtlLockRes.ResultCode;
                }

                // set wh transfer Detail
                var whTransDetail = whTransDtlLockRes.WhTransDetail;

                // get and lock linked wh transfer
                var whTransHeadLockRes = await LockWhTransHeader(db, whTransDetail.WhTransferId);
                if (whTransHeadLockRes.ResultCode != PutawayResultCode.SUCCESS ||
                    whTransHeadLockRes.WhTransferHeader == null)
                {
                    return whTransHeadLockRes.ResultCode;
                }

                // set wh rtansfer header
                var whTransferHeader = whTransHeadLockRes.WhTransferHeader;

                // insert new sequence into inventory history table
                var invHistSaved = await InvHistoryRepo.CreateInventoryHistoryMod(db, invHistory, TranType.PUTAWAY);
                if (!invHistSaved)
                {
                    return PutawayResultCode.INVHISTCOMMITFAILED;
                }

                // update inventory header status into available if product condition is good
                if (data.LotAtt.ProductConditionId == (ProductCondition.GOOD).ToString())
                {
                    var invStatusUpdated = await InventoryRepo.SetInventoryStatus(db,
                                                                                    invHistory.InventoryId,
                                                                                    (InvStatus.AVAILABLE).ToString(),
                                                                                    data.UserAccountId,
                                                                                    TranType.PUTAWAY);
                    if (!invStatusUpdated)
                    {
                        return PutawayResultCode.INVSTATUSUPDATEFAILED;
                    }
                }

                // close receiving status
                var rcvStatusUpdate = await ReceivingRepo.SetReceivingStatus(db,
                                                                                data.ReceivingId,
                                                                                (ReceivingStatus.CLOSED).ToString(),
                                                                                TranType.PUTAWAY,
                                                                                data.UserAccountId);
                if (!rcvStatusUpdate)
                {
                    return PutawayResultCode.RCVSTATUSUPDATEFAILED;
                }

                // close putaway task status
                var putawayStatusUpdt = await SetPutawayStatus(db,
                                                                            data.PutawayTaskId,
                                                                            (PutawayStatus.CLOSED).ToString(),
                                                                            data.UserAccountId,
                                                                            TranType.PUTAWAY);
                if (!putawayStatusUpdt)
                {
                    return PutawayResultCode.PUTAWAYSTATUSUPDATEFAILED;
                }

                // get updated wh transfer header status id
                var updateWhTransStatus = await WhTransferRepo.GetReturnsUpdatedStatus(db, whTransferHeader.WhTransferId);
                if (string.IsNullOrEmpty(updateWhTransStatus))
                {
                    return PutawayResultCode.FAILEDGETUPDATEWHTRANSSTATUS;
                }

                // check if wh transfer is complete base on its updated wh transfer status id
                if (updateWhTransStatus == (POStatus.FULLRCV).ToString())
                {
                    // check wh transfer details if there's no pending putaway task
                    var hasPutawayTask = await HasPendingPutawayTaskRet(db, whTransferHeader.WhTransferId);

                    // close wh transfer and wh transfer details status if there's no pending receive and putaway
                    if (!hasPutawayTask)
                    {
                        // update wh transfer status
                        whTransferHeader.WhTransStatusId = (POStatus.CLOSED).ToString();
                        whTransferHeader.ModifiedBy = data.UserAccountId;

                        var whTransClosed = await WhTransferRepo.UpdateWhTransfer(db, whTransferHeader, TranType.PUTAWAY);
                        if (!whTransClosed)
                        {
                            return PutawayResultCode.FAILEDTOUPDATEWHTRANSSTATUS;
                        }

                        // retreive all wh transfer Details to close each
                        var whTransDetails = await WhTransDetailRepo.LockWhTransDetails(db, whTransferHeader.WhTransferId);
                        if (!whTransDetails.Any())
                        {
                            return PutawayResultCode.FAILEDTOUPDATEWHTRANSDTLSTATUS;
                        }

                        // update each wh transfer detail status
                        foreach (var whTransDtl in whTransDetails)
                        {
                            whTransDtl.WhTransLineStatusId = (POLneStatus.CLOSED).ToString();
                            whTransDtl.ModifiedBy = (data.UserAccountId).ToString();

                            var whTransDtlClosed = await WhTransDetailRepo.UpdateWhTransDetailMod(db, whTransDtl, TranType.PUTAWAY);

                            if (!whTransDtlClosed)
                            {
                                return PutawayResultCode.FAILEDTOUPDATEWHTRANSDTLSTATUS;
                            }
                        }

                        return PutawayResultCode.SUCCESS;
                    }
                }

                // simply update returns header status into its updated one
                whTransferHeader.WhTransStatusId = updateWhTransStatus;
                whTransferHeader.ModifiedBy = data.UserAccountId;

                var whTransStatusUpdated = await WhTransferRepo.UpdateWhTransfer(db, whTransferHeader, TranType.PUTAWAY);
                if (!whTransStatusUpdated)
                {
                    return PutawayResultCode.FAILEDTOUPDATEWHTRANSSTATUS;
                }
                else
                {
                    return PutawayResultCode.SUCCESS;
                }
            }

            return PutawayResultCode.FAILED;
        }

        public async Task<PutawayResultCode> PartialPutaway(IDbConnection db, PutawayContainerModel data)
        {
            // build inventory history next sequence
            if (data.InvHistory != null
                && data.WinData != null
                && data.WinData.PutawayWinOne != null
                && data.WinData.PutawayWinTwo != null
                && data.LotAtt != null)
            {
                // init data
                // var putawayWinOne = data.WinData.PutawayWinOne;
                var putawayWinTwo = data.WinData.PutawayWinTwo;
                var invHistory = data.InvHistory;

                // reduce original inventory history product qty by creating its successding  sequence record
                invHistory.SeqNum += 1;
                invHistory.QtyFrom = invHistory.QtyTo;
                invHistory.QtyTo = invHistory.QtyTo - putawayWinTwo.PutawayQty;
                invHistory.LpnFrom = invHistory.LpnTo;
                invHistory.CreatedBy = data.UserAccountId;

                // insert reduced sequence into inventory history table
                var invHistSaved = await InvHistoryRepo.CreateInventoryHistoryMod(db, invHistory, TranType.PUTAWAY);
                if (!invHistSaved)
                {
                    return PutawayResultCode.INVHISTREDUCEFAILED;
                }

                // start of auto generation of inv, invhist, receiving and putaway trans for partial putaway prerequisite

                // get inventory next document number
                string? invId = await IdNumberRepo.GetNxtDocNum("INV", data.UserAccountId);

                // get receiving next document number
                string? receivingId = await IdNumberRepo.GetNxtDocNum("RCVING", data.UserAccountId);

                // get putaway task next document number
                string? putawayTaskId = await IdNumberRepo.GetNxtDocNum("PUTAWAY", data.UserAccountId);

                // get track id nect document number
                string? tidTo = await IdNumberRepo.GetNxtDocNum("TRACE", data.UserAccountId);

                // proceed if required document numbers are not empty
                if (!string.IsNullOrEmpty(invId)
                    && !string.IsNullOrEmpty(receivingId)
                    && !string.IsNullOrEmpty(putawayTaskId)
                    && !string.IsNullOrEmpty(tidTo))
                {
                    // create inventory header table data
                    var inv = new InventoryModel()
                    {
                        InventoryId = invId,
                        Sku = data.Sku,
                        InventoryStatusId = (InvStatus.REFERRED).ToString()
                    };

                    // save header to inventory table
                    var invSaved = await InventoryRepo.CreateInventoryMod(db, inv, data.UserAccountId, TranType.PUTAWAY);
                    if (!invSaved)
                    {
                        return PutawayResultCode.FAILEDTOCREATEINVHEADER;
                    }

                    // create inventory history data for partial putaway qty
                    var invDetail = new InventoryHistoryModel()
                    {
                        InventoryId = invId,
                        SeqNum = 1,
                        DocumentRefId = invHistory.DocumentRefId,
                        QtyFrom = 0,
                        QtyTo = putawayWinTwo.PutawayQty,
                        LocationFrom = invHistory.LocationTo,
                        LocationTo = putawayWinTwo.PutawayLocation,
                        TrackIdFrom = invHistory.TrackIdTo,
                        TrackIdTo = tidTo,
                        LpnFrom = invHistory.LpnTo,
                        LpnTo = putawayWinTwo.PutawayLPN,
                        LotAttributeId = invHistory.LotAttributeId,
                        TransactionTypeId = (TranType.PUTAWAY).ToString(),
                        CreatedBy = data.UserAccountId
                    };

                    // record inventory detail to inventory history table
                    bool dtlSaved = await InvHistoryRepo.CreateInventoryHistoryMod(db, invDetail, TranType.PUTAWAY);
                    if (!dtlSaved)
                    {
                        return PutawayResultCode.FAILEDTOCREATEINVDETAIL;
                    }

                    // create receiving transaction detail
                    var rcvDetail = new ReceivingModel()
                    {
                        ReceivingId = receivingId,
                        DocLineId = invHistory.DocumentRefId,
                        InventoryId = invId,
                        SeqNum = 1,
                        ReceivingStatusId = (ReceivingStatus.CREATED).ToString(),
                        CreatedBy = data.UserAccountId,
                        ModifiedBy = data.UserAccountId
                    };

                    // record receiving detail to receving table
                    bool rcvSaved = await ReceivingRepo.CreateReceivingMod(db, rcvDetail, TranType.PUTAWAY);
                    if (!rcvSaved)
                    {
                        return PutawayResultCode.FAILEDTOCREATERCVDETAIL;
                    }

                    // build putaway task data
                    var putawayDtl = new PutawayTaskModel()
                    {
                        PutawayTaskId = putawayTaskId,
                        ReceivingId = receivingId,
                        InventoryId = invId,
                        SeqNum = 1,
                        PutawayStatusId = (PutawayStatus.CREATED).ToString(),
                        CreatedBy = data.UserAccountId,
                        ModifiedBy = data.UserAccountId
                    };

                    // record putaway task to putawaytask table
                    bool putawaySaved = await CreatePutawayTaskMod(db, putawayDtl, TranType.PUTAWAY);
                    if (!putawaySaved)
                    {
                        return PutawayResultCode.FAILEDTOCREATEPUTAWAY;
                    }

                    // can skip get and lock process of generated transactions as it is in hold by current database connection tran

                    // commit partial putaway changes
                    var refIds = new PartialPutawayRefIdModel()
                    {
                        InventoryId = invId,
                        ReceivingId = receivingId,
                        PutawayTaskId = putawayTaskId,
                        TargetTrackId = tidTo,
                        ProductConditionId = data.LotAtt.ProductConditionId,
                        UserAccountId = data.UserAccountId
                    };

                    var res = await CommitPartialPutaway(db, refIds, data.WinData);

                    return res;

                }

            }

            return PutawayResultCode.FAILED;
        }

        public async Task<PutawayResultCode> CommitPartialPutaway(IDbConnection db, PartialPutawayRefIdModel refIds, PutawayTaskProcModel winData)
        {
            if (refIds != null && winData != null)
            {
                // init ref data
                // var putawayWinOne = winData.PutawayWinOne;
                var putawayWinTwo = winData.PutawayWinTwo;

                // create succeeeding seqnum of inventory history
                var invDetail = await InvHistoryRepo.GetInvHistoryByTrackId(db, refIds.TargetTrackId);
                if (invDetail == null)
                {
                    return PutawayResultCode.FAILEDTOGETINVHIST;
                }

                if(putawayWinTwo != null)
                {
                    // build inventory history succeeding sequnce record
                    invDetail.SeqNum += 1;
                    invDetail.QtyFrom = invDetail.QtyTo;
                    invDetail.QtyTo = putawayWinTwo.PutawayQty;
                    invDetail.LpnFrom = invDetail.LpnTo;
                    invDetail.LpnTo = putawayWinTwo.PutawayLPN;
                    invDetail.LocationFrom = invDetail.LocationTo;
                    invDetail.LocationTo = putawayWinTwo.PutawayLocation;
                    invDetail.TransactionTypeId = (TranType.PUTAWAY).ToString();
                    invDetail.CreatedBy = refIds.UserAccountId;
                }

                // insert new sequence into inventory history table
                var invHistSaved = await InvHistoryRepo.CreateInventoryHistoryMod(db, invDetail, TranType.PUTAWAY);
                if (!invHistSaved)
                {
                    return PutawayResultCode.INVHISTCOMMITFAILED;
                }

                // update inventory header status into available if product condition is good
                if (refIds.ProductConditionId == (ProductCondition.GOOD).ToString())
                {
                    var invStatusUpdated = await InventoryRepo.SetInventoryStatus(db,
                                                                                    refIds.InventoryId,
                                                                                    (InvStatus.AVAILABLE).ToString(),
                                                                                    refIds.UserAccountId,
                                                                                    TranType.PUTAWAY);
                    if (!invStatusUpdated)
                    {
                        return PutawayResultCode.INVSTATUSUPDATEFAILED;
                    }
                }

                // close receiving status
                var rcvStatusUpdate = await ReceivingRepo.SetReceivingStatus(db,
                                                                                refIds.ReceivingId,
                                                                                (ReceivingStatus.CLOSED).ToString(),
                                                                                TranType.PUTAWAY,
                                                                                refIds.UserAccountId);
                if (!rcvStatusUpdate)
                {
                    return PutawayResultCode.RCVSTATUSUPDATEFAILED;
                }

                // close putaway task status
                var putawayStatusUpdt = await SetPutawayStatus(db,
                                                                            refIds.PutawayTaskId,
                                                                            (PutawayStatus.CLOSED).ToString(),
                                                                            refIds.UserAccountId,
                                                                            TranType.PUTAWAY);
                if (!putawayStatusUpdt)
                {
                    return PutawayResultCode.PUTAWAYSTATUSUPDATEFAILED;
                }

                // keep PO header and PO detail status as there's still partial qty left for putaway

                return PutawayResultCode.SUCCESS;
            }

            return PutawayResultCode.FAILED;
        }

        public async Task<QryPalletPutawayResult> QueryLPNPUtaway(string palletId)
        {
            // init return object
            var ret = new QryPalletPutawayResult();

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var contents = await GetPalletTIDContents(db, palletId);

                if (!contents.Any())
                {
                    ret.ResultCode = PalletPutawayResultCode.EMPTYPALLET;
                    return ret;
                }

                // check if each contents has correct receiving status id
                var rcvChecker = contents.Where(x => x.ReceivingStatusId != (ReceivingStatus.CREATED).ToString()).ToList();
                if (rcvChecker.Any())
                {
                    ret.ResultCode = PalletPutawayResultCode.CONFLICTEDRCVSTATUS;
                    return ret;
                }

                // check if each contents has correct inventory status id
                var invChecker = contents.Where(x => x.InventoryStatusId != (InvStatus.REFERRED).ToString()).ToList();
                if (invChecker.Any())
                {
                    ret.ResultCode = PalletPutawayResultCode.CONFLICTEDINVSTATUS;
                    return ret;
                }

                // check if each contents has correct putaway status id
                var putawayChecker = contents.Where(x => x.PutawayStatusId != (PutawayStatus.CREATED).ToString()).ToList();
                if (putawayChecker.Any())
                {
                    ret.ResultCode = PalletPutawayResultCode.CONFLICTEDPUTAWAYSTATUS;
                    return ret;
                }

                // return contents is everything seems fine
                ret.ResultCode = PalletPutawayResultCode.SUCCESS;
                ret.Data = contents;
            }

            return ret;
        }

        private async Task<IEnumerable<PalletContentModel>> GetPalletTIDContents(IDbConnection db, string palletId)
        {
            string strQry = @"CALL `spGetPalletContents`(@palletId)";

            var param = new DynamicParameters();
            param.Add("@palletId", palletId);

            return await db.QueryAsync<PalletContentModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<PalletPutawayResult> ProceedPalletPutaway(CommitPalletPutawayModel data)
        {
            var ret = new PalletPutawayResult();

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // success TID putaway counter
                int cntr = 0;

                if (!data.Contents.Any())
                {
                    ret.ResultCode = PalletPutawayResultCode.INVALIDDATAGIVEN;
                    return ret;
                }

                foreach (var itm in data.Contents)
                {
                    var tidDetails = await LPNPutawayQryTIDDetails(itm.TrackIdTo);

                    // return failed result in case specific TID has conflit or issue
                    if (tidDetails.ResultCode != PutawayResultCode.SUCCESS)
                    {
                        ret.ResultCode = PalletPutawayResultCode.SPECIFICTIDISSUE;
                        var resultMsg = (tidDetails.ResultCode).ToString();
                        ret.ConlictMsg = $"{itm.TrackIdTo} - {resultMsg}";
                        return ret;
                    }

                    if (tidDetails.Data == null)
                    {
                        ret.ResultCode = PalletPutawayResultCode.INVALIDDATAGIVEN;
                        return ret;
                    }

                    if (tidDetails.Data.PutawayWinOne == null)
                    {
                        ret.ResultCode = PalletPutawayResultCode.INVALIDDATAGIVEN;
                        return ret;
                    }

                    // continue process as tidDetails result code is success
                    var winOneData = tidDetails.Data.PutawayWinOne;

                    var winTwoData = new PutawayWinTwoModel()
                    {
                        PutawayQty = winOneData.CurrentQty,
                        PutawayLocation = data.PutawayLocation,
                        PutawayLPN = winOneData.CurrentLPN
                    };

                    var putawayData = new PutawayTaskProcModel()
                    {
                        PutawayWinOne = winOneData,
                        PutawayWinTwo = winTwoData,
                        UserAccountId = data.UserAccountId
                    };

                    var tidCommitRes = await CommitPalletPutaway(db, putawayData);

                    // check if commit transaction is completed properly
                    if (tidCommitRes.ResultCode != PutawayResultCode.SUCCESS)
                    {
                        ret.ResultCode = PalletPutawayResultCode.SPECIFICTIDISSUE;
                        var resultMsg = (tidCommitRes.ResultCode).ToString();
                        ret.ConlictMsg = $"{itm.TrackIdTo} - {resultMsg}";
                        return ret;
                    }

                    // increase success TID putaway counter
                    cntr += 1;

                    // proceed to next loop
                    continue;
                }

                if (cntr == data.Contents.Count())
                {
                    // double check that there is procesed data
                    if (data.Contents.Count() > 0)
                    {
                        ret.ResultCode = PalletPutawayResultCode.SUCCESS;

                        // finally commit all completed transactions
                    }
                }
            }

            return ret;
        }

        public async Task<PutawayResultModel> LPNPutawayQryTIDDetails(string trackId)
        {
            // init return object
            var ret = new PutawayResultModel()
            {
                ResultCode = PutawayResultCode.FAILED,
            };

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // get inventory history detail by scanned track id and lock
                var invDetail = await InvHistoryRepo.GetInvHistoryByTrackId(db, trackId);
                if (invDetail == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKINVHIST;
                    return ret;
                }

                // get and lock inventory header
                var invHead = await InventoryRepo.LockInventoryByInvId(db, invDetail.InventoryId);
                if (invHead == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKINVHEAD;
                    return ret;
                }

                // get and lock linked receiving tran
                // get actual receiving service from Func<> service (bypassed circular dependency)
                // var receivingRepo = ReceivingRepo();
                var rcvDetail = await ReceivingRepo.LockReceiveDetailRefMulti(db, invDetail.DocumentRefId, invDetail.InventoryId);
                if (rcvDetail == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKRCVDETAILS;
                    return ret;
                }

                // get and lock putaway task
                var putawayDtl = await LockPutawayTaskDtl(db, rcvDetail.ReceivingId);
                if (putawayDtl == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKPTask;
                    return ret;
                }

                // check if putawat task is still in create status // ignore warning here
                if (putawayDtl.PutawayStatusId != (PutawayStatus.CREATED).ToString())
                {
                    ret.ResultCode = PutawayResultCode.INVALIDPUTAWAYTASKSTATUS;
                    return ret;
                }

                // get product lot attributes
                var lotAtt = await LotAttRepo.GetLotAttributeDetailById(invDetail.LotAttributeId);
                if (lotAtt == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOGETLOTATT;
                    return ret;
                }

                // process details to be displayed on handheld putaway window 1

                // get sku product description // ignore warning here
                var prodDetails = await ProductRepo.GetProductById(invHead.Sku);
                if (prodDetails == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOGETPRODDETAILS;
                    return ret;
                }

                // build details for putaway window 1 display
                var putawayWinOne = new PutawayWinOneModel()
                {
                    TargetTrackId = invDetail.TrackIdTo,
                    Sku = invHead.Sku,
                    Barcode = prodDetails.Barcode,
                    Barcode2 = prodDetails.Barcode2,
                    Barcode3 = prodDetails.Barcode3,
                    Barcode4 = prodDetails.Barcode4,
                    ProductName = prodDetails.ProductName,
                    CurrentQty = invDetail.QtyTo,
                    CurrentLocation = invDetail.LocationTo,
                    CurrentLPN = invDetail.LpnTo,
                    ManufactureDate = lotAtt.ManufactureDate,
                    ExpiryDate = lotAtt.ExpiryDate,
                    WarehousingDate = lotAtt.WarehousingDate,
                    ProductConditionId = lotAtt.ProductConditionId
                };

                // details for putaway window 2 is null by default

                var data = new PutawayTaskProcModel()
                {
                    PutawayWinOne = putawayWinOne
                };

                // append success result code and data to ret object
                ret.ResultCode = PutawayResultCode.SUCCESS;
                ret.Data = data;
                return ret;
            };
        }

        public async Task<PutawayResultModel> CommitPalletPutaway(IDbConnection db, PutawayTaskProcModel data)
        {
            // init return object
            var ret = new PutawayResultModel()
            {
                ResultCode = PutawayResultCode.FAILED,
            };

            // split data
            var winOneData = data.PutawayWinOne;
            var winTwoData = data.PutawayWinTwo;

            if (winOneData != null && winTwoData != null)
            {
                // check if current and target location is not the same
                if (winOneData.CurrentLocation == winTwoData.PutawayLocation)
                {
                    ret.ResultCode = PutawayResultCode.TARGETLOCCONFLICT;
                    return ret;
                }

                // define if putaway if partial or full
                bool isPartial;
                var qtyLeft = winOneData.CurrentQty - winTwoData.PutawayQty;

                if (qtyLeft == 0)
                {
                    isPartial = false;
                }
                else if (qtyLeft > 0)
                {
                    isPartial = true;
                }
                else
                {
                    // exit process due to putaway qty exceeds original
                    ret.ResultCode = PutawayResultCode.QTYEXCEEDS;
                    return ret;
                }

                // get inventory history detail by scanned track id and lock
                var invDetail = await InvHistoryRepo.GetInvHistoryByTrackId(db, winOneData.TargetTrackId);
                if (invDetail == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOGETINVHIST;
                    return ret;
                }

                var invDtlLock = await InvHistoryRepo.LockInvHistDetail(db, invDetail.InventoryId, invDetail.SeqNum);
                if (invDtlLock == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKINVHIST;
                    return ret;
                }

                // get and lock inventory header
                var invHead = await InventoryRepo.LockInventoryByInvId(db, invDetail.InventoryId);
                if (invHead == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKINVHEAD;
                    return ret;
                }

                // get and lock linked receiving tran
                // get actual receiving service from Func<> service (bypassed circular dependency)
                //var receivingRepo = ReceivingRepo();
                var rcvDetail = await ReceivingRepo.LockReceiveDetailRefMulti(db, invDetail.DocumentRefId, invDetail.InventoryId);
                if (rcvDetail == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKRCVDETAILS;
                    return ret;
                }

                // get and lock putaway task
                var putawayDtl = await LockPutawayTaskDtl(db, rcvDetail.ReceivingId);
                if (putawayDtl == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOLOCKPTask;
                    return ret;
                }

                // check if putaway task is still in create status // ignore warning here
                if (putawayDtl.PutawayStatusId != (PutawayStatus.CREATED).ToString())
                {
                    ret.ResultCode = PutawayResultCode.INVALIDPUTAWAYTASKSTATUS;
                    return ret;
                }

                // get product lot attributes
                var lotAtt = await LotAttRepo.GetLotAttributeDetailById(invDetail.LotAttributeId);
                if (lotAtt == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOGETLOTATT;
                    return ret;
                }

                // get linked PO detail then lock

                // process details to be displayed on handheld putaway window 1

                // get sku product description // ignore warning here
                var prodDetails = await ProductRepo.GetProductById(invHead.Sku);
                if (prodDetails == null)
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTOGETPRODDETAILS;
                    return ret;
                }

                // build details for putaway window 1 display
                var putawayWinOne = new PutawayWinOneModel()
                {
                    TargetTrackId = invDetail.TrackIdTo,
                    Sku = invHead.Sku,
                    Barcode = prodDetails.Barcode,
                    Barcode2 = prodDetails.Barcode2,
                    Barcode3 = prodDetails.Barcode3,
                    Barcode4 = prodDetails.Barcode4,
                    ProductName = prodDetails.ProductName,
                    CurrentQty = invDetail.QtyTo,
                    CurrentLocation = invDetail.LocationTo,
                    CurrentLPN = invDetail.LpnTo,
                    ManufactureDate = lotAtt.ManufactureDate,
                    ExpiryDate = lotAtt.ExpiryDate,
                    WarehousingDate = lotAtt.WarehousingDate,
                    ProductConditionId = lotAtt.ProductConditionId
                };

                // compare putaway prev. and current putaway win data
                if (!winOneData.Equals(putawayWinOne))
                {
                    ret.ResultCode = PutawayResultCode.RECORDINCONSISTENCY;
                    return ret;
                }

                // check if LPNTo is not in used by any location except INSTAGING
                if (!string.IsNullOrEmpty(winTwoData.PutawayLPN))
                {
                    var isLPNUsed = await InvHistoryRepo.ChkLPNIsUsedInStorage(winTwoData.PutawayLPN);
                    if (isLPNUsed)
                    {
                        ret.ResultCode = PutawayResultCode.LPNISALREADYUSED;
                        return ret;
                    }

                    // check if LPN Id is in correct format
                    var lpnPrefix = await IdNumberRepo.GetIdPrefix(db, (TranType.LPN).ToString());
                    if (!string.IsNullOrEmpty(lpnPrefix))
                    {
                        if (!winTwoData.PutawayLPN.Contains(lpnPrefix))
                        {
                            ret.ResultCode = PutawayResultCode.INVALIDLPNFORMAT;
                            return ret;
                        }
                    }
                }

                // check if pallet is same on current receiving pallet
                if (winOneData.CurrentLPN != winTwoData.PutawayLPN)
                {
                    ret.ResultCode = PutawayResultCode.CANNOTUSEOTHERLPN;
                    return ret;
                }

                // re-validate target putaway location
                var targetLocChk = await LocationRepo.DefineTargetLocByLocId(winTwoData.PutawayLocation);
                if (targetLocChk.ResultCode != PutawayResultCode.SUCCESS)
                {
                    ret.ResultCode = targetLocChk.ResultCode;
                    return ret;
                }

                // location LPN insist condition checking
                if (targetLocChk.TargetLoc != null)
                {
                    var insistedLPNTO = targetLocChk.TargetLoc.LPNTo;
                    if (!string.IsNullOrEmpty(insistedLPNTO))
                    {
                        if (insistedLPNTO != winTwoData.PutawayLPN)
                        {
                            ret.ResultCode = PutawayResultCode.INVALIDLPNTO;
                            return ret;
                        }
                    }
                }

                // define if its either PO, Returns or Transfer type putaway
                var docTypeOrigin = await DefineTranTypeByDocId(db, invDetail.DocumentRefId);
                if (string.IsNullOrEmpty(docTypeOrigin) || docTypeOrigin == "ERR")
                {
                    ret.ResultCode = PutawayResultCode.FAILEDTODEFINEDOCORIGINTYPE;
                    return ret;
                }

                // build putaway details container object
                var container = new PutawayContainerModel()
                {
                    Sku = invHead.Sku,
                    ReceivingId = rcvDetail.ReceivingId,
                    PutawayTaskId = putawayDtl.PutawayTaskId,
                    UserAccountId = data.UserAccountId,
                    InvHistory = invDetail,
                    WinData = data,
                    LotAtt = lotAtt
                };

                // init final result holder
                var tempResult = PutawayResultCode.FAILED;

                if (!isPartial)
                {
                    // proceed to PO full putaway
                    tempResult = await ProceedFullPutawayByDoc(db, container, docTypeOrigin);
                }
                else
                {
                    // lock parent documents first
                    var docLockRes = await PartialPutawayDocLocker(db, docTypeOrigin, invDetail.DocumentRefId);
                    if (docLockRes != PutawayResultCode.SUCCESS)
                    {
                        ret.ResultCode = docLockRes;
                        return ret;
                    }

                    // proceed partial putaway
                    tempResult = await PartialPutaway(db, container);
                }

                ret.ResultCode = tempResult;

                // check tempresult status
                if (tempResult == PutawayResultCode.SUCCESS)
                {
                    // append updated data on return object
                    ret.Data = data;

                    // removed commit
                    // commit and release db transaction
                    // tran.Commit();
                }
            }

            return ret;
        }

        public async Task<string?> DefineTranTypeByDocId(IDbConnection db, string docLineId)
        {
            string strQry = @"CALL `spDefineDocTranOrigin`(@docLineId)";

            var param = new DynamicParameters();
            param.Add("@docLineId", docLineId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        private async Task<PutawayPOHeadrLockResult> LockPOHeader(IDbConnection db, string poId)
        {
            var ret = new PutawayPOHeadrLockResult();

            // get and lock linked PO
            var poHeader = await PORepo.LockPO(db, poId);
            if (poHeader == null)
            {
                ret.ResultCode = PutawayResultCode.FAILEDTOLOCKPOHEADER;
                return ret;
            }

            // check PO header status if still valid
            if (poHeader.PoStatusId == (POStatus.CREATED).ToString() ||
                poHeader.PoStatusId == (POStatus.FRCCLOSED).ToString() ||
                poHeader.PoStatusId == (POStatus.CLOSED).ToString() ||
                poHeader.PoStatusId == (POStatus.CANCELED).ToString())
            {
                ret.ResultCode = PutawayResultCode.POHEADERSTATUSNOTVALID;
                return ret;
            }

            //else
            ret.ResultCode = PutawayResultCode.SUCCESS;
            ret.POHeader = poHeader;
            return ret;
        }

        private async Task<PutawayPODetailLockResult> LockPODetail(IDbConnection db, string documentRefId)
        {
            var ret = new PutawayPODetailLockResult();

            // get and lock returns linked detail
            var poDetail = await PODetailRepo.LockPODetail(db, documentRefId);
            if (poDetail == null)
            {
                ret.ResultCode = PutawayResultCode.FAILEDTOLOCKPODETAIL;
                return ret;
            }

            // check returns detail status if still valid
            if (poDetail.PoLineStatusId == (POLneStatus.FRCCLOSED).ToString() ||
                poDetail.PoLineStatusId == (POLneStatus.CLOSED).ToString())
            {
                ret.ResultCode = PutawayResultCode.PODETAILSSTATUSNOTVALID;
                return ret;
            }

            //else
            ret.ResultCode = PutawayResultCode.SUCCESS;
            ret.PODetail = poDetail;
            return ret;
        }

        private async Task<PutawayRetHeadrLockResult> LockReturnsHeader(IDbConnection db, string returnsId)
        {
            var ret = new PutawayRetHeadrLockResult();

            // get and lock linked returns
            var retHeader = await ReturnsRepo.LockReturns(db, returnsId);
            if (retHeader == null)
            {
                ret.ResultCode = PutawayResultCode.FAILEDTOLOCKRETHEADER;
                return ret;
            }

            // check returns header status if still valid
            if (retHeader.ReturnsStatusId == (POStatus.CREATED).ToString() ||
                retHeader.ReturnsStatusId == (POStatus.FRCCLOSED).ToString() ||
                retHeader.ReturnsStatusId == (POStatus.CLOSED).ToString() ||
                retHeader.ReturnsStatusId == (POStatus.CANCELED).ToString())
            {
                ret.ResultCode = PutawayResultCode.RETHEADERSTATUSNOTVALID;
                return ret;
            }

            //else
            ret.ResultCode = PutawayResultCode.SUCCESS;
            ret.ReturnsHeader = retHeader;
            return ret;
        }

        private async Task<PutawayRetDetailLockResult> LockReturnsDetail(IDbConnection db, string documentRefId)
        {
            var ret = new PutawayRetDetailLockResult();

            // get and lock returns linked detail
            var retDetail = await RetDetailRepo.LockRetDetail(db, documentRefId);
            if (retDetail == null)
            {
                ret.ResultCode = PutawayResultCode.FAILEDTOLOCKRETDETAIL;
                return ret;
            }

            // check returns detail status if still valid
            if (retDetail.ReturnsLineStatusId == (POLneStatus.FRCCLOSED).ToString() ||
                retDetail.ReturnsLineStatusId == (POLneStatus.CLOSED).ToString())
            {
                ret.ResultCode = PutawayResultCode.RETDETAILSSTATUSNOTVALID;
                return ret;
            }

            //else
            ret.ResultCode = PutawayResultCode.SUCCESS;
            ret.ReturnsDetail = retDetail;
            return ret;
        }

        private async Task<PutawayWhTransHeadrLockResult> LockWhTransHeader(IDbConnection db, string whTransId)
        {
            var ret = new PutawayWhTransHeadrLockResult();

            // get and lock linked wh transfer
            var whTransHeader = await WhTransferRepo.LockWhTransfer(db, whTransId);
            if (whTransHeader == null)
            {
                ret.ResultCode = PutawayResultCode.FAILEDTOLOCKWHTRANSHEADER;
                return ret;
            }

            // check returns header status if still valid
            if (whTransHeader.WhTransStatusId == (POStatus.CREATED).ToString() ||
                whTransHeader.WhTransStatusId == (POStatus.FRCCLOSED).ToString() ||
                whTransHeader.WhTransStatusId == (POStatus.CLOSED).ToString() ||
                whTransHeader.WhTransStatusId == (POStatus.CANCELED).ToString())
            {
                ret.ResultCode = PutawayResultCode.WHTRANSHEADERSTATUSNOTVALID;
                return ret;
            }

            //else
            ret.ResultCode = PutawayResultCode.SUCCESS;
            ret.WhTransferHeader = whTransHeader;
            return ret;
        }

        private async Task<PutawayWhTransDetailLockResult> LockWhTransDetail(IDbConnection db, string documentRefId)
        {
            var ret = new PutawayWhTransDetailLockResult();

            // get and lock wh transfer linked detail
            var whTransDetail = await WhTransDetailRepo.LockWhTransDetail(db, documentRefId);
            if (whTransDetail == null)
            {
                ret.ResultCode = PutawayResultCode.FAILEDTOLOCKWHTRANSDETAIL;
                return ret;
            }

            // check wh transfer detail status if still valid
            if (whTransDetail.WhTransLineStatusId == (POLneStatus.FRCCLOSED).ToString() ||
                whTransDetail.WhTransLineStatusId == (POLneStatus.CLOSED).ToString())
            {
                ret.ResultCode = PutawayResultCode.WHTRANSDETAILSSTATUSNOTVALID;
                return ret;
            }

            //else
            ret.ResultCode = PutawayResultCode.SUCCESS;
            ret.WhTransDetail = whTransDetail;
            return ret;
        }

        private async Task<PutawayResultCode> ProceedFullPutawayByDoc(IDbConnection db, PutawayContainerModel container, string docTypeOrigin)
        {
            // proceed to PO full putaway
            if (docTypeOrigin == (TranType.PO).ToString())
            {
                return await FullPutawayPO(db, container);
            }

            // proceed to returns full putaway
            if (docTypeOrigin == (TranType.RCVRET).ToString())
            {
                return await FullPutawayReturns(db, container);
            }

            // proceed to WH transfer full putaway
            if (docTypeOrigin == (TranType.RCVTRANS).ToString())
            {
                return await FullPutawayWhTrans(db, container);
            }

            return PutawayResultCode.FAILEDTODEFINEDOCORIGINTYPE;
        }

        private async Task<PutawayResultCode> PartialPutawayDocLocker(IDbConnection db, string docTypeOrigin, string documentRefId)
        {
            // proceed to PO full putaway
            if (docTypeOrigin == (TranType.PO).ToString())
            {
                return await CommitPODocLock(db, documentRefId);
            }

            // proceed to returns full putaway
            if (docTypeOrigin == (TranType.RCVRET).ToString())
            {
                return await CommitReturnsDocLock(db, documentRefId);
            }

            // proceed to WH transfer full putaway
            if (docTypeOrigin == (TranType.RCVTRANS).ToString())
            {
                return await CommitWhTransDocLock(db, documentRefId);
            }

            return PutawayResultCode.FAILEDTODEFINEDOCORIGINTYPE;
        }

        private async Task<PutawayResultCode> CommitPODocLock(IDbConnection db, string documentRefId)
        {
            // lock po detail
            var poDtlLockRes = await LockPODetail(db, documentRefId);
            if (poDtlLockRes.ResultCode != PutawayResultCode.SUCCESS ||
                poDtlLockRes.PODetail == null)
            {
                return poDtlLockRes.ResultCode;
            }

            // lock po header
            var poHeaderRes = await LockPOHeader(db, poDtlLockRes.PODetail.PoId);
            if (poHeaderRes.ResultCode != PutawayResultCode.SUCCESS ||
                poHeaderRes.POHeader == null)
            {
                return poHeaderRes.ResultCode;
            }

            return PutawayResultCode.SUCCESS;
        }
        private async Task<PutawayResultCode> CommitReturnsDocLock(IDbConnection db, string documentRefId)
        {
            // lock returns detail
            var retDtlLockRes = await LockReturnsDetail(db, documentRefId);
            if (retDtlLockRes.ResultCode != PutawayResultCode.SUCCESS ||
                retDtlLockRes.ReturnsDetail == null)
            {
                return retDtlLockRes.ResultCode;
            }

            // lock returns header
            var retHeaderRes = await LockReturnsHeader(db, retDtlLockRes.ReturnsDetail.ReturnsId);
            if (retHeaderRes.ResultCode != PutawayResultCode.SUCCESS ||
                retHeaderRes.ReturnsHeader == null)
            {
                return retHeaderRes.ResultCode;
            }

            return PutawayResultCode.SUCCESS;
        }

        private async Task<PutawayResultCode> CommitWhTransDocLock(IDbConnection db, string documentRefId)
        {
            // lock wh transfer detail
            var whTransDtlLockRes = await LockWhTransDetail(db, documentRefId);
            if (whTransDtlLockRes.ResultCode != PutawayResultCode.SUCCESS ||
                whTransDtlLockRes.WhTransDetail == null)
            {
                return whTransDtlLockRes.ResultCode;
            }

            // lock wh transfer header
            var whTransHeaderRes = await LockWhTransHeader(db, whTransDtlLockRes.WhTransDetail.WhTransferId);
            if (whTransHeaderRes.ResultCode != PutawayResultCode.SUCCESS ||
                whTransHeaderRes.WhTransferHeader == null)
            {
                return whTransHeaderRes.ResultCode;
            }

            return PutawayResultCode.SUCCESS;
        }

    }
}