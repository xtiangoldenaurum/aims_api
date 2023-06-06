using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IPODetailCore
    {
        Task<RequestResponse> GetPODetailByPoIDPaged(string poId, int pageNum, int pageItem);
        Task<RequestResponse> GetPODetailByPoIDPagedMod(string poId, int pageNum, int pageItem);
        Task<RequestResponse> GetPODetailPg(int pageNum, int pageItem);
        Task<RequestResponse> GetPODetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetPODetailById(string poLineId);
        Task<RequestResponse> CreatePODetail(PODetailModel poDetail);
        Task<RequestResponse> UpdatePODetail(PODetailModel poDetail);
        Task<RequestResponse> DeletePODetail(string poLineId);
        Task<RequestResponse> DeletePODetailMod(string poLineId, string userAccountId);
    }
}
