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
    public interface IWhTransReceivingRepository
    {
        Task<WhTransReceivePagedMdl?> GetReceivesByWhTransId(string whTransId, int pageNum, int pageItem);
        Task<IEnumerable<WhTransReceivedDetailModel>?> GetCancelableRcvsById(string whTransLineId, int pageNum, int pageItem);
        Task<IEnumerable<WhTransReceivedDetailModel>?> GetCancelableRcvsByWhTransId(string whTransId, int pageNum, int pageItem);
        Task<IEnumerable<ReceivingModel>> GetReceivingPgSrch(string searchKey, int pageNum, int pageItem);
        Task<ReceivingModel> GetReceivingById(string receivingId);
        Task<ReceivingModel> GetReceivingByIdMod(IDbConnection db, string receivingId);
        Task<bool> ReceivingExists(string receivingId);
        Task<ReceivingResultModel> Receiving(WhTransReceivingModelMod data);
        Task<int> GetDocLineRcvdQty(IDbConnection db, string docDetailId);
        Task<bool> CreateReceivingMod(IDbConnection db, ReceivingModel receiving, TranType tranTyp);
        Task<CancelRcvResultCode> CancelReceived(string receivingId, string userAccountId);
        Task<ReceivingModel> LockReceiveDetail(IDbConnection db, string receivingId);
        Task<bool> SetReceivingStatus(string receivingId, string receivingStatus);
        Task<bool> SetReceivingStatus(IDbConnection db, string receivingId, string receivingStatus, TranType tranTyp, string userAccountId);
    }
}
