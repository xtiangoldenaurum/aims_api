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
    public interface ILotAttributeDetailRepository
    {
        Task<IEnumerable<LotAttributeDetailModel>> GetLotAttributeDetailPg(int pageNum, int pageItem);
        Task<IEnumerable<LotAttributeDetailModel>> GetLotAttributeDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<LotAttributeDetailModel> GetLotAttributeDetailById(string lotAttributeId);
        Task<string> GetLotAttributeIdWithSameDetail(LotAttributeDetailModel lotatt);
        Task<bool> LotAttributeDetailExists(string lotAttributeId);
        Task<bool> CreateLotAttributeDetail(LotAttributeDetailModel lotAttributeDetail);
        Task<bool> CreateLotAttributeDetailMod(IDbConnection db, LotAttributeDetailModel lotAttributeDetail, TranType tranTyp);
        Task<bool> UpdateLotAttributeDetail(LotAttributeDetailModel lotAttributeDetail);
        Task<bool> DeleteLotAttributeDetail(string lotAttributeId, string userAccountId);
    }
}
