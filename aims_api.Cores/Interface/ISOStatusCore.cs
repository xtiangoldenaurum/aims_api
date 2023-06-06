using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ISOStatusCore
    {
        Task<RequestResponse> GetSOStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetSOStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetSOStatusById(string soStatusId);
        Task<RequestResponse> CreateSOStatus(SOStatusModel soStatus);
        Task<RequestResponse> UpdateSOStatus(SOStatusModel soStatus);
        Task<RequestResponse> DeleteSOStatus(string soStatusId);
    }
}
