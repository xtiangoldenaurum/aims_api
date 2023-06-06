using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ILotAttributeDetailCore
    {
        Task<RequestResponse> GetLotAttributeDetailPg(int pageNum, int pageItem);
        Task<RequestResponse> GetLotAttributeDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetLotAttributeDetailById(string lotAttributeId);
        Task<RequestResponse> CreateLotAttributeDetail(LotAttributeDetailModel lotAttributeDetail);
        Task<RequestResponse> UpdateLotAttributeDetail(LotAttributeDetailModel lotAttributeDetail);
        Task<RequestResponse> DeleteLotAttributeDetail(string lotAttributeId, string userAccountId);
    }
}
