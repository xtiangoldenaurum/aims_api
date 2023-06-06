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
    public interface IWhTransferDetailRepository
    {
        Task<WhtransDetailPagedMdl?> GetWhTransDetailByTransIdPaged(string whTransferId, int pageNum, int pageItem);
        Task<WhtransDetailPagedMdlMod?> GetWhTransDetailByTrnasIdPagedMod(string whTransId, int pageNum, int pageItem);
        Task<IEnumerable<WhTransferDetailModel>> GetWhTransferDetailPg(int pageNum, int pageItem);
        Task<IEnumerable<WhTransferDetailModel>> GetWhTransferDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<WhTransferDetailModel> GetWhTransferDetailById(string whTransferLineId);
        Task<WhTransferDetailModel> GetWhTransferDetailByIdmod(IDbConnection db, string whTransferLineId);
        Task<string> GetWhTransDtlCancelRcvUpdatedStatus(IDbConnection db, string whTransDetailId, string receivingId);
        Task<bool> WhTransferDetailExists(string whTransferLineId);
        Task<bool> ChkWhTransDetailLock(string whTransLineId);
        Task<bool> WhTransDetailReceivable(string whTransLineId);
        Task<WhTransferDetailModel> LockWhTransDetail(IDbConnection db, string whTransLineId);
        Task<IEnumerable<WhTransferDetailModel>> LockWhTransDetails(IDbConnection db, string whTransId);
        Task<bool> CreateWhTransferDetail(WhTransferDetailModel whTransferDetail);
        Task<bool> CreateWhTransferDetailMod(IDbConnection db, WhTransferDetailModel whTransferDetail);
        Task<bool> UpdateWhTransDetailMod(IDbConnection db, WhTransferDetailModel whTransDetail, TranType tranTyp);
        Task<bool> UpdateWhTransferDetail(WhTransferDetailModel whTransferDetail);
        Task<bool> DeleteWhTransferDetail(string whTransferLineId);
        Task<WhTransDetailDelResultCode> DeleteWhTransDetailMod(string whTransLineId, string userAccountId);
    }
}
