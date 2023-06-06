using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aims_api.Enums;

namespace aims_api.Repositories.Interface
{
    public interface IPODetailRepository
    {
        Task<PODetailPagedMdl?> GetPODetailByPoIDPaged(string poId, int pageNum, int pageItem);
        Task<PODetailPagedMdlMod?> GetPODetailByPoIDPagedMod(string poId, int pageNum, int pageItem);
        Task<Pagination?> GetPODetailPageDetail(IDbConnection db, string poId, int pageNum, int pageItem, int rowCount);
        Task<Pagination?> GetPODetailPageDetailMod(IDbConnection db, string poId, int pageNum, int pageItem, int rowCount);
        Task<IEnumerable<PODetailModel>> GetPODetailPg(int pageNum, int pageItem);
        Task<IEnumerable<PODetailModel>> GetPODetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<PODetailModel> GetPODetailById(string poLineId);
        Task<PODetailModel> GetPODetailByIdMod(IDbConnection db, string poLineId);
        Task<string> GetPODtlCancelRCvUpdatedStatus(IDbConnection db, string poDetailId, string receivingId);
        Task<bool> PODetailExists(string poLineId);
        Task<bool> ChkPoDetailLock(string poLineId);
        Task<bool> PODetailReceivable(string poLineId);
        Task<PODetailModel> LockPODetail(IDbConnection db, string poLineId);
        Task<IEnumerable<PODetailModel>> LockPODetails(IDbConnection db, string poId);
        Task<bool> CreatePODetail(PODetailModel poDetail);
        Task<bool> CreatePODetailMod(IDbConnection db, PODetailModel poDetail);
        Task<bool> UpdatePODetail(PODetailModel poDetail);
        Task<bool> UpdatePODetailMod(IDbConnection db, PODetailModel poDetail, TranType tranTyp);
        Task<bool> DeletePODetail(string poLineId);
        Task<PODetailDelResultCode> DeletePODetailMod(string poLineId, string userAccountId);
    }
}
