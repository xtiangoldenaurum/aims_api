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
    public interface IUniqueTagsRepository
    {
        Task<IEnumerable<UniqueTagsModel>> GetUniqueTagsPg(int pageNum, int pageItem);
        Task<IEnumerable<UniqueTagsModel>> GetUniqueTagsPgSrch(string searchKey, int pageNum, int pageItem);
        Task<UniqueTagsModel> GetUniqueTagsById(int uniqueTagId);
        Task<IEnumerable<UniqueTagsModel>> GetUniqueTagsByTrackId(IDbConnection db, string trackId);
        Task<bool> UniqueTagsExists(int uniqueTagId);
        Task<bool> CreateUniqueTags(UniqueTagsModel uniqueTags);
        Task<RecordTagResultCode> CreateUniqueTagsMod(IDbConnection db,
                                                    IEnumerable<UniqueTagsModel> tags,
                                                    string trackId,
                                                    TranType tranType,
                                                    string documentRefId,
                                                    string docLineRefId,
                                                    string userAccountId);
        Task<bool> UpdateUniqueTags(UniqueTagsModel uniqueTags);
        Task<bool> DeleteUniqueTags(int uniqueTagId);
        Task<CancelRcvResultCode> DeleteUniqueTagsMod(IDbConnection db, string trackId, string userAccountId);
    }
}
