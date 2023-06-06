using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IReceivingCore
    {
        Task<RequestResponse> GetReceivingPg(int pageNum, int pageItem);
        Task<RequestResponse> GetCancelableRcvs(int pageNum, int pageItem);
        Task<RequestResponse> GetReceivesByPOId(string poId, int pageNum, int pageItem);
        Task<RequestResponse> GetCancelableRcvsById(string poLineId, int pageNum, int pageItem);
        Task<RequestResponse> GetCancelableRcvsByPOId(string poId, int pageNum, int pageItem);
        Task<RequestResponse> GetReceivingPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetReceivingById(string receivingId);
        Task<RequestResponse> Receiving(ReceivingModelMod data);
        Task<RequestResponse> CreateReceiving(ReceivingModel receiving);
        Task<RequestResponse> UpdateReceiving(ReceivingModel receiving);
        Task<RequestResponse> CancelReceived(string receivingId, string userAccountId);
        Task<RequestResponse> DeleteReceiving(string receivingId);
        Task<RequestResponse> GetReceivingDetailByTrackId(string trackId);
    }
}
