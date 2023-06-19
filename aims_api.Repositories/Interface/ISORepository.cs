using aims_api.Enums;
using aims_api.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ISORepository
    {
        Task<IEnumerable<SOModel>> GetSOPg(int pageNum, int pageItem);
        Task<IEnumerable<SOModel>> GetSOPgSrch(string searchKey, int pageNum, int pageItem);
        Task<SOModel> GetSOById(string soId);
        Task<bool> SOExists(string soId);
        Task<SOCreateTranResult> CreateSOMod(SOModelMod so);
        Task<bool> CreateSO(IDbConnection db, SOModel so);
        Task<SOTranResultCode> UpdateSOMod(SOModelMod so);
        //Task<bool> UpdateSO(SOModel so);
        Task<bool> DeleteSO(string soId);
        Task<IEnumerable<SOModel>> GetExportSO();
        Task<SOCreateTranResult> CreateBulkSO(IFormFile file, string path);
        bool ValidateCsvHeader(string headerLine);
        Task<bool> ValidateXlsxHeader(ExcelWorksheet worksheet);
    }
}
