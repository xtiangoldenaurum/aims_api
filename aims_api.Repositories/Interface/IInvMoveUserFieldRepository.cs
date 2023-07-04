using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvMoveUserFieldRepository
    {
        Task<IEnumerable<InvMoveUserFieldModel>> GetInvMoveUserFieldPg(int pageNum, int pageItem);
        Task<IEnumerable<InvMoveUserFieldModel>> GetInvMoveUserFieldPgSrch(string searchKey, int pageNum, int pageItem);
        Task<dynamic> GetInvMoveUserFieldById(string invMoveId);
        Task<dynamic?> GetInvMoveUFields();
        Task<string?> GetInvMoveUsrFldQryFilter();
        Task<bool> CreateInvMoveUField(string fieldName, string createdBy);
        Task<bool> InitInvMoveUField(IDbConnection db, string invMoveId);
        Task<bool> UpdateinvMoveUField(IDbConnection db, string invMoveId, string createdBy, dynamic data);
        Task<bool> UpdateinvMoveUFieldMOD(IDbConnection db, string invMoveId, string modifiedBy, dynamic data);
        Task<bool> ChkColExists(string fieldName);
        Task<bool> UpdateInvMoveUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<bool> DeleteInvMoveUField(string fieldName, string userAccountId);
        Task<bool> InvMoveUserFieldExists(string invMoveId);
        Task<bool> CreateInvMoveUserField(InvMoveUserFieldModel invMoveUserField);
        Task<bool> UpdateInvMoveUserField(InvMoveUserFieldModel invMoveUserField);
        Task<bool> DeleteInvMoveUserField(string poId);
    }
}
