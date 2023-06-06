using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ISOTypeCore
    {
        Task<RequestResponse> GetSOTypePg(int pageNum, int pageItem);
        Task<RequestResponse> GetSOTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetSOTypeById(string soTypeId);
        Task<RequestResponse> CreateSOType(SOTypeModel soType);
        Task<RequestResponse> UpdateSOType(SOTypeModel soType);
        Task<RequestResponse> DeleteSOType(string soTypeId);
    }
}
