using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IReturnsDetailCore
    {
        Task<RequestResponse> GetRetDetailByRetIdPagedMod(string returnsId, int pageNum, int pageItem);
        Task<RequestResponse> GetReturnsDetailPg(int pageNum, int pageItem);
        Task<RequestResponse> GetReturnsDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetReturnsDetailById(string returnsLineId);
        Task<RequestResponse> CreateReturnsDetail(ReturnsDetailModel returnsDetail);
        Task<RequestResponse> UpdateReturnsDetail(ReturnsDetailModel returnsDetail);
        Task<RequestResponse> DeleteReturnsDetail(string returnsLineId);
        Task<RequestResponse> DeleteReturnsDetailMod(string returnsLineId, string userAccountId);
    }
}
