using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IReferredTagsCore
    {
        Task<RequestResponse> GetReferredTagsPg(int pageNum, int pageItem);
        Task<RequestResponse> GetReferredTagsPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetReferredTagsById(int referredTagId);
        Task<RequestResponse> CreateReferredTags(ReferredTagsModel referredTags);
        Task<RequestResponse> UpdateReferredTags(ReferredTagsModel referredTags);
        Task<RequestResponse> DeleteReferredTags(int referredTagId);
    }
}
