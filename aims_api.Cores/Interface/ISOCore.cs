using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ISOCore
    {
        Task<RequestResponse> GetSOPg(int pageNum, int pageItem);
        Task<RequestResponse> GetSOPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetSOById(string soId);
        Task<RequestResponse> GetSOByIdMod(string soId);
        Task<RequestResponse> CreateSO(SOModel so);
        Task<RequestResponse> UpdateSO(SOModel so);
        Task<RequestResponse> DeleteSO(string soId);
        Task<string> DownloadSOTemplate();
        Task<RequestResponse> ExportSO();
    }
}
