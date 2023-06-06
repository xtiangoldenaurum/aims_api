using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ILocationTypeCore
    {
        Task<RequestResponse> GetLocationTypePg(int pageNum, int pageItem);
        Task<RequestResponse> GetLocationTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetLocationTypeById(string locationTypeId);
        Task<RequestResponse> CreateLocationType(LocationTypeModel locationType);
        Task<RequestResponse> UpdateLocationType(LocationTypeModel locationType);
        Task<RequestResponse> DeleteLocationType(string locationTypeId, string userAccountId);
    }
}
