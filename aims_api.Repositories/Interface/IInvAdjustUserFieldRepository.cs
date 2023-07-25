using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvAdjustUserFieldRepository
    {
        Task<IEnumerable<InvAdjustUserFieldModel>> GetInvAdjustUserFieldPg(int pageNum, int pageItem);
        Task<IEnumerable<InvAdjustUserFieldModel>> GetInvAdjustUserFieldPgSrch(string searchKey, int pageNum, int pageItem);
        Task<dynamic> GetInvAdjustUserFieldById(string invAdjustId);
        Task<dynamic?> GetInvAdjustUFields();
        Task<string?> GetInvAdjustUsrFldQryFilter();
        Task<bool> CreateInvAdjustUField(string fieldName, string createdBy);
        Task<bool> InitInvAdjustUField(IDbConnection db, string invAdjustId);
        Task<bool> UpdateInvAdjustUField(IDbConnection db, string invAdjustId, string createdBy, dynamic data);
        Task<bool> UpdateInvAdjustUFieldMOD(IDbConnection db, string invAdjustId, string modifiedBy, dynamic data);
        Task<bool> ChkColExists(string fieldName);
        Task<bool> UpdateInvAdjustUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<bool> DeleteInvAdjustUField(string fieldName, string userAccountId);
        Task<bool> InvAdjustUserFieldExists(string invAdjustId);
        Task<bool> CreateInvAdjustUserField(InvAdjustUserFieldModel invAdjustUserField);
        Task<bool> UpdateInvAdjustUserField(InvAdjustUserFieldModel invAdjustUserField);
        Task<bool> DeleteInvAdjustUserField(string invAdjustId);
    }
}
