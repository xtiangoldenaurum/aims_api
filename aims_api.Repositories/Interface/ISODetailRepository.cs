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
    public interface ISODetailRepository
    {
        Task<IEnumerable<SODetailModel>> GetSODetailPg(int pageNum, int pageItem);
        Task<IEnumerable<SODetailModel>> GetSODetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<SODetailModel> GetSODetailById(string soLineId);
        Task<SODetailModel> GetSODetailByIdMod(IDbConnection db, string soLineId);
        Task<bool> SODetailExists(string soLineId);
        Task<SODetailModel> LockSODetail(IDbConnection db, string soLineId);
        Task<IEnumerable<SODetailModel>> LockSODetails(IDbConnection db, string soId);
        Task<bool> CreateSODetail(SODetailModel soDetail);
        Task<bool> CreateSODetailMod(IDbConnection db, SODetailModel soDetail);
        Task<bool> UpdateSODetail(SODetailModel soDetail);
        Task<bool> UpdateSODetailMod(IDbConnection db, SODetailModel soDetail, TranType tranTyp);
        Task<bool> DeleteSODetail(string soLineId);
    }
}
