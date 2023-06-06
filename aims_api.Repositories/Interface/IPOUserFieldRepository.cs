using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IPOUserFieldRepository
    {
        Task<IEnumerable<POUserFieldModel>> GetPOUserFieldPg(int pageNum, int pageItem);
        Task<IEnumerable<POUserFieldModel>> GetPOUserFieldPgSrch(string searchKey, int pageNum, int pageItem);
        Task<dynamic> GetPOUserFieldById(string poId);
        Task<dynamic?> GetPOUFields();
        Task<string?> GetPOUsrFldQryFilter();
        Task<bool> CreatePOUField(string fieldName, string createdBy);
        Task<bool> InitPOUField(IDbConnection db, string poId);
        Task<bool> UpdatePOUField(IDbConnection db, string poId, string createdBy, dynamic data);
        Task<bool> UpdatePOUFieldMOD(IDbConnection db, string poId, string modifiedBy, dynamic data);
        Task<bool> ChkColExists(string fieldName);
        Task<bool> UpdatePOUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<bool> DeletePOUField(string fieldName, string userAccountId);
        Task<bool> POUserFieldExists(string poId);
        Task<bool> CreatePOUserField(POUserFieldModel poUserField);
        Task<bool> UpdatePOUserField(POUserFieldModel poUserField);
        Task<bool> DeletePOUserField(string poId);
    }
}
