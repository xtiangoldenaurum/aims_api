using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IPOLineStatusCore
    {
        Task<RequestResponse> GetPOLineStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetPOLineStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetPOLineStatusById(string poLineStatusId);
        Task<RequestResponse> CreatePOLineStatus(POLineStatusModel poLineStatus);
        Task<RequestResponse> UpdatePOLineStatus(POLineStatusModel poLineStatus);
        Task<RequestResponse> DeletePOLineStatus(string poLineStatusId);
    }
}
