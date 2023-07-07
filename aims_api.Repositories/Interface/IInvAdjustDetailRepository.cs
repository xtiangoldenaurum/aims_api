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
    public interface IInvAdjustDetailRepository
    {
        Task<InvAdjustDetailPagedMdl?> GetInvAdjustDetailByInvAdjustIDPaged(string invAdjustId, int pageNum, int pageItem);
        Task<InvAdjustDetailPagedMdlMod?> GetInvAdjustDetailByInvAdjustIDPagedMod(string invAdjustId, int pageNum, int pageItem);
        Task<Pagination?> GetInvAdjustDetailPageDetail(IDbConnection db, string InvAdjustId, int pageNum, int pageItem, int rowCount);
        Task<Pagination?> GetInvAdjustDetailPageDetailMod(IDbConnection db, string poId, int pageNum, int pageItem, int rowCount);
        Task<IEnumerable<InvAdjustDetailModel>> GetInvAdjustDetailPg(int pageNum, int pageItem);
        Task<IEnumerable<InvAdjustDetailModel>> GetInvAdjustDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvAdjustDetailModel> GetInvAdjustDetailById(string invAdjustLineId);
        Task<InvAdjustDetailModel> GetInvAdjustDetailByIdMod(IDbConnection db, string invAdjustLineId);
        Task<string> GetInvAdjustDtlCancelAdjUpdatedStatus(IDbConnection db, string invAdjustDetailId, string invAdjustId);
        Task<bool> InvAdjustDetailExists(string InvAdjustLineId);
        Task<bool> ChkInvAdjustDetailLock(string InvAdjustLineId);
        Task<InvAdjustDetailModel> LockInvAdjustDetail(IDbConnection db, string invAdjustLineId);
        Task<IEnumerable<InvAdjustDetailModel>> LockInvAdjustDetails(IDbConnection db, string invAdjustId);
        Task<bool> CreateInvAdjustDetail(InvAdjustDetailModel invAdjustDetail);
        Task<bool> CreateInvAdjustDetailMod(IDbConnection db, InvAdjustDetailModel invAdjustDetail);
        Task<bool> UpdateInvAdjustDetail(InvAdjustDetailModel invAdjustDetail);
        Task<bool> UpdateInvAdjustDetailMod(IDbConnection db, InvAdjustDetailModel invAdjustDetail, TranType tranTyp);
        Task<bool> DeleteInvAdjustDetail(string invAdjustLineId);
        Task<InvAdjustDetailDelResultCode> DeleteInvAdjustDetailMod(string invAdjustLineId, string userAccountId);
    }
}
