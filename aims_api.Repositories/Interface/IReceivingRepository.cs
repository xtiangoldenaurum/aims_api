using aims_api.Enums;
using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IReceivingRepository
    {
        Task<IEnumerable<ReceivingModel>> GetReceivingPg(int pageNum, int pageItem);
        Task<ReceivePagedMdl?> GetCancelableRcvs(int pageNum, int pageItem);
        Task<ReceivePagedMdl?> GetReceivesByPOId(string poId, int pageNum, int pageItem);
        Task<IEnumerable<POReceivedDetailModel>?> GetCancelableRcvsById(string poLineId, int pageNum, int pageItem);
        Task<IEnumerable<POReceivedDetailModel>?> GetCancelableRcvsByPOId(string poId, int pageNum, int pageItem);
        Task<IEnumerable<ReceivingModel>> GetReceivingPgSrch(string searchKey, int pageNum, int pageItem);
        Task<ReceivingModel> GetReceivingById(string receivingId);
        Task<ReceivingModel> GetReceivingByIdMod(IDbConnection db, string receivingId);
        Task<bool> ReceivingExists(string receivingId);
        Task<ReceivingResultModel> Receiving(ReceivingModelMod data);
        Task<bool> CreateReceiving(ReceivingModel receiving);
        Task<bool> CreateReceivingMod(IDbConnection db, ReceivingModel receiving, TranType tranTyp);
        Task<bool> UpdateReceiving(ReceivingModel receiving);
        Task<CancelRcvResultCode> CancelReceived(string receivingId, string userAccountId);
        Task<ReceivingModel> LockReceiveDetail(IDbConnection db, string receivingId);
        Task<bool> SetReceivingStatus(string receivingId, string receivingStatus);
        Task<bool> SetReceivingStatus(IDbConnection db, string receivingId, string receivingStatus, TranType tranTyp, string userAccountId);
        Task<bool> DeleteReceiving(string receivingId);
        Task<ReceivingModel> LockReceiveDetailRefMulti(IDbConnection db, string poLineId, string inventoryId);
        Task<ReceivingDetailModel?> GetReceivingDetailByTrackId(string trackId);

    }
}
