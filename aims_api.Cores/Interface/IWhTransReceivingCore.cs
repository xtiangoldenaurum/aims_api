using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IWhTransReceivingCore
    {
        Task<RequestResponse> GetReceivesByWhTransId(string whTransId, int pageNum, int pageItem);
        Task<RequestResponse> GetCancelableRcvsById(string whTranLineId, int pageNum, int pageItem);
        Task<RequestResponse> GetCancelableRcvsByWhTransId(string whTransId, int pageNum, int pageItem);
        Task<RequestResponse> GetReceivingPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetReceivingById(string receivingId);
        Task<RequestResponse> Receiving(WhTransReceivingModelMod data);
        Task<RequestResponse> CancelReceived(string receivingId, string userAccountId);
    }
}
