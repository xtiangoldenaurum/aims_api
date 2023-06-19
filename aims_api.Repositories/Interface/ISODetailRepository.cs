using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ISODetailRepository
    {
        Task<IEnumerable<SODetailModel>> GetSODetailPg(int pageNum, int pageItem);
        Task<IEnumerable<SODetailModel>> GetSODetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<SODetailModel> GetSODetailById(string soLineId);
        Task<bool> SODetailExists(string soLineId);
        Task<bool> CreateSODetail(SODetailModel soDetail);
        Task<bool> CreateSODetailMod(IDbConnection db, SODetailModel soDetail);
        Task<bool> UpdateSODetail(SODetailModel soDetail);
        Task<bool> DeleteSODetail(string soLineId);
    }
}
