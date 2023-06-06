using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IPOStatusCore
    {
        Task<RequestResponse> GetPOStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetPOStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetPOStatusById(string poStatusId);
        Task<RequestResponse> CreatePOStatus(POStatusModel poStatus);
        Task<RequestResponse> UpdatePOStatus(POStatusModel poStatus);
        Task<RequestResponse> DeletePOStatus(string poStatusId);
    }
}
