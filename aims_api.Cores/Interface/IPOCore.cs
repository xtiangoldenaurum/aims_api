using aims_api.Models;
using aims_api.Utilities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IPOCore
    {
        Task<RequestResponse> GetPOSpecial(POFilteredMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetPOForRcvPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetPOPg(int pageNum, int pageItem);
        Task<RequestResponse> GetPOPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetPOById(string poId);
        Task<RequestResponse> GetPOByIdMod(string poId);
        Task<RequestResponse> CreatePOMod(POModelMod po);
        Task<RequestResponse> UpdatePOMod(POModelMod po);
        //Task<RequestResponse> CreatePO(POModel po);
        //Task<RequestResponse> UpdatePO(POModel po);
        Task<RequestResponse> DeletePO(string poId);
        Task<RequestResponse> CancelPO(string poId, string userAccountId);
        Task<RequestResponse> ForceCancelPO(string poId, string userAccountId);
        Task<string> DownloadPOTemplate();
        Task<RequestResponse> ExportPO();
        Task<RequestResponse> CreateBulkPO(IFormFile file, string path);

    }
}
