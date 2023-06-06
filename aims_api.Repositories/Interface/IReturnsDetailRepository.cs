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
    public interface IReturnsDetailRepository
    {
        Task<ReturnsDetailPagedMdl?> GetRetDetailByRetIdPaged(string returnsId, int pageNum, int pageItem);
        Task<RetDetailPagedMdlMod?> GetRetDetailByRetIdPagedMod(string returnsId, int pageNum, int pageItem);
        Task<Pagination?> GetReturnsDetailPageDetail(IDbConnection db, string returnsId, int pageNum, int pageItem, int rowCount);
        Task<Pagination?> GetReturnsDetailPageDetailMod(IDbConnection db, string returnsId, int pageNum, int pageItem, int rowCount);
        Task<IEnumerable<ReturnsDetailModel>> GetReturnsDetailPg(int pageNum, int pageItem);
        Task<IEnumerable<ReturnsDetailModel>> GetReturnsDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<ReturnsDetailModel> GetReturnsDetailById(string returnsLineId);
        Task<ReturnsDetailModel> GetReturnsDetailByIdMod(IDbConnection db, string returnsLineId);
        Task<string> GetRetDtlCancelRcvUpdatedStatus(IDbConnection db, string retDetailId, string receivingId);
        Task<bool> ReturnsDetailExists(string returnsLineId);
        Task<bool> ChkRetDetailLock(string retLineId);
        Task<bool> RetDetailReceivable(string retLineId);
        Task<ReturnsDetailModel> LockRetDetail(IDbConnection db, string retLineId);
        Task<IEnumerable<ReturnsDetailModel>> LockReturnsDetails(IDbConnection db, string returnsId);
        Task<bool> CreateReturnsDetail(ReturnsDetailModel returnsDetail);
        Task<bool> CreateReturnsDetailMod(IDbConnection db, ReturnsDetailModel returnsDetail);
        Task<bool> UpdateReturnsDetailMod(IDbConnection db, ReturnsDetailModel returnsDetail, TranType tranTyp);
        Task<bool> UpdateReturnsDetail(ReturnsDetailModel returnsDetail);
        Task<bool> DeleteReturnsDetail(string returnsLineId);
        Task<RetDetailDelResultCode> DeleteRetDetailMod(string retLineId, string userAccountId);
    }
}
