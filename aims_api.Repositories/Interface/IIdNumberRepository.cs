using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IIdNumberRepository
    {
        Task<IEnumerable<IdNumberModel>> GetIdNumberPg(int pageNum, int pageItem);
        Task<IEnumerable<IdNumberModel>> GetIdNumberPgSrch(string searchKey, int pageNum, int pageItem);
        Task<IdNumberModel> GetIdNumberById(string transactionTypeId);
        Task<IdNumberModel> GetIdNumberByIdMod(IDbConnection db, string transactionTypeId);
        Task<bool> IdNumberExists(string transactionTypeId);
        Task<string?> GetNextIdNum(string tranTypeId);
        Task<string?> GetNxtDocNum(string tranTypeId, string userAccountId);
        Task<bool> SaveLastUsedID(IDbConnection db, int lastId, string tranTypeId);
        Task<bool> CreateIdNumber(IdNumberModel idNumber);
        Task<bool> UpdateIdNumber(IdNumberModel idNumber);
        Task<bool> DeleteIdNumber(string transactionTypeId, string userAccountId);
        Task<string?> GetIdPrefix(IDbConnection db, string tranTypeId);
        Task<ReceivingResultModel?> PrePrintEPC(int count);
        Task<ReceivingResultModel?> PrePrintLPN(int count);
    }
}
