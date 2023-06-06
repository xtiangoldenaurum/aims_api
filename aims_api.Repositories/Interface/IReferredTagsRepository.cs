using aims_api.Enums;
using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IReferredTagsRepository
    {
        Task<IEnumerable<ReferredTagsModel>> GetReferredTagsPg(int pageNum, int pageItem);
        Task<ReferredTagsModel> LockReferredTags(IDbConnection db, int uniqueTagId);
        Task<IEnumerable<ReferredTagsModel>> GetReferredTagsPgSrch(string searchKey, int pageNum, int pageItem);
        Task<ReferredTagsModel> GetReferredTagsById(int referredTagId);
        Task<bool> ReferredTagsExists(int referredTagId);
        Task<bool> CreateReferredTags(ReferredTagsModel referredTags);
        Task<bool> CreateReferredTagsMod(IDbConnection db, ReferredTagsModel referredTags, TranType tranType);
        Task<bool> UpdateReferredTags(ReferredTagsModel referredTags);
        Task<bool> DeleteReferredTags(int referredTagId);
    }
}
