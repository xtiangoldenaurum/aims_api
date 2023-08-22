using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvCountUserFieldRepository
    {
        Task<IEnumerable<InvCountUserFieldModel>> GetInvCountUserFieldPg(int pageNum, int pageItem);
        Task<IEnumerable<InvCountUserFieldModel>> GetInvCountUserFieldPgSrch(string searchKey, int pageNum, int pageItem);
        Task<dynamic> GetInvCountUserFieldById(string invCountId);
        Task<dynamic?> GetInvCountUFields();
        Task<string?> GetInvCountUsrFldQryFilter();
        Task<bool> CreateInvCountUField(string fieldName, string createdBy);
        Task<bool> InitInvCountUField(IDbConnection db, string invCountId);
        Task<bool> UpdateinvCountUField(IDbConnection db, string invCountId, string createdBy, dynamic data);
        Task<bool> UpdateinvCountUFieldMOD(IDbConnection db, string invCountId, string modifiedBy, dynamic data);
        Task<bool> ChkColExists(string fieldName);
        Task<bool> UpdateInvCountUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<bool> DeleteInvCountUField(string fieldName, string userAccountId);
        Task<bool> InvCountUserFieldExists(string invCountId);
        Task<bool> CreateInvCountUserField(InvCountUserFieldModel invCountUserField);
        Task<bool> UpdateInvCountUserField(InvCountUserFieldModel invCountUserField);
        Task<bool> DeleteInvCountUserField(string invCountId);
    }
}
