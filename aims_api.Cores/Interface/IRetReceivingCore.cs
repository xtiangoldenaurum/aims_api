using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IRetReceivingCore
    {
        Task<RequestResponse> GetReceivesByReturnsId(string returnsId, int pageNum, int pageItem);
        Task<RequestResponse> GetCancelableRcvsById(string retLineId, int pageNum, int pageItem);
        Task<RequestResponse> GetCancelableRcvsByRetId(string returnsId, int pageNum, int pageItem);
        Task<RequestResponse> GetReceivingPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetReceivingById(string receivingId);
        Task<RequestResponse> Receiving(RetReceivingModelMod data);
        Task<RequestResponse> CancelReceived(string receivingId, string userAccountId);
    }
}
