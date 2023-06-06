using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ISODetailCore
    {
        Task<RequestResponse> GetSODetailPg(int pageNum, int pageItem);
        Task<RequestResponse> GetSODetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetSODetailById(string soLineId);
        Task<RequestResponse> CreateSODetail(SODetailModel soDetail);
        Task<RequestResponse> UpdateSODetail(SODetailModel soDetail);
        Task<RequestResponse> DeleteSODetail(string soLineId);
    }
}
