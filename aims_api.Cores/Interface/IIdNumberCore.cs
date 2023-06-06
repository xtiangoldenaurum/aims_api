using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IIdNumberCore
    {
        Task<RequestResponse> GetIdNumberPg(int pageNum, int pageItem);
        Task<RequestResponse> GetIdNumberPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetIdNumberById(string transactionTypeId);
        Task<RequestResponse> GetNextIdNum(string tranTypeId);
        Task<RequestResponse> GetNxtDocNum(string tranTypeId, string userAccountId);
        Task<RequestResponse> CreateIdNumber(IdNumberModel idNumber);
        Task<RequestResponse> UpdateIdNumber(IdNumberModel idNumber);
        Task<RequestResponse> DeleteIdNumber(string transactionTypeId, string userAccountId);
        Task<RequestResponse> PrePrintEPC(int count);
        Task<RequestResponse> PrePrintLPN(int count);
    }
}
