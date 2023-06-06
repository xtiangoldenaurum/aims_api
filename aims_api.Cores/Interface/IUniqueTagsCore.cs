using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IUniqueTagsCore
    {
        Task<RequestResponse> GetUniqueTagsPg(int pageNum, int pageItem);
        Task<RequestResponse> GetUniqueTagsPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetUniqueTagsById(int uniqueTagId);
        Task<RequestResponse> CreateUniqueTags(UniqueTagsModel uniqueTags);
        Task<RequestResponse> UpdateUniqueTags(UniqueTagsModel uniqueTags);
        Task<RequestResponse> DeleteUniqueTags(int uniqueTagId);
    }
}
