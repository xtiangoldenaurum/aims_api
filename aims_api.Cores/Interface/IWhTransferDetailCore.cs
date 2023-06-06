using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IWhTransferDetailCore
    {
        Task<RequestResponse> GetWhTransDetailByTrnasIdPagedMod(string whTransId, int pageNum, int pageItem);
        Task<RequestResponse> GetWhTransferDetailPg(int pageNum, int pageItem);
        Task<RequestResponse> GetWhTransferDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetWhTransferDetailById(string whTransferLineId);
        Task<RequestResponse> CreateWhTransferDetail(WhTransferDetailModel whTransferDetail);
        Task<RequestResponse> UpdateWhTransferDetail(WhTransferDetailModel whTransferDetail);
        Task<RequestResponse> DeleteWhTransferDetail(string whTransferLineId);
        Task<RequestResponse> DeleteWhTransferDetailMmod(string whTransferLineId, string userAccountId);
    }
}
