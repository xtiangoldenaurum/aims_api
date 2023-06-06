using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IReturnsUserFieldRepository
    {
        Task<IEnumerable<ReturnsUserFieldModel>> GetReturnsUserFieldPg(int pageNum, int pageItem);
        Task<IEnumerable<ReturnsUserFieldModel>> GetReturnsUserFieldPgSrch(string searchKey, int pageNum, int pageItem);
        Task<dynamic> GetReturnsUserFieldById(string returnsId);
        Task<dynamic?> GetReturnsUFields();
        Task<string?> GetReturnsUsrFldQryFilter();
        Task<bool> CreateReturnsUField(string fieldName, string createdBy);
        Task<bool> InitReturnsUField(IDbConnection db, string returnsId);
        Task<bool> UpdateReturnsUField(IDbConnection db, string returnsId, string createdBy, dynamic data);
        Task<bool> UpdateReturnsUFieldMOD(IDbConnection db, string returnsId, string modifiedBy, dynamic data);
        Task<bool> ChkColExists(string fieldName);
        Task<bool> UpdateReturnsUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<bool> DeleteReturnsUField(string fieldName, string userAccountId);
        Task<bool> ReturnsUserFieldExists(string returnsId);
        Task<bool> CreateReturnsUserField(ReturnsUserFieldModel returnsUserField);
        Task<bool> UpdateReturnsUserField(ReturnsUserFieldModel returnsUserField);
        Task<bool> DeleteReturnsUserField(string returnsId);
    }
}
