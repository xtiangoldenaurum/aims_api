using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ISOLineStatusCore
    {
        Task<RequestResponse> GetSOLineStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetSOLineStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetSOLineStatusById(string soLineStatusId);
        Task<RequestResponse> CreateSOLineStatus(SOLineStatusModel soLineStatus);
        Task<RequestResponse> UpdateSOLineStatus(SOLineStatusModel soLineStatus);
        Task<RequestResponse> DeleteSOLineStatus(string soLineStatusId);
    }
}
