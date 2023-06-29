using aims_api.Models;
using aims_api.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class InvMoveUserFieldRepository : IInvMoveUserFieldRepository
    {
        public Task<bool> ChkColExists(string fieldName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateInvMoveUField(string fieldName, string createdBy)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateInvMoveUserField(InvMoveUserFieldModel invMoveUserField)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteInvMoveUField(string fieldName, string userAccountId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteInvMoveUserField(string poId)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GeInvMoveOUFields()
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetInvMoveUserFieldById(string invMoveId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InvMoveUserFieldModel>> GetInvMoveUserFieldPg(int pageNum, int pageItem)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InvMoveUserFieldModel>> GetInvMoveUserFieldPgSrch(string searchKey, int pageNum, int pageItem)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetInvMoveUsrFldQryFilter()
        {
            throw new NotImplementedException();
        }

        public Task<bool> InitInvMoveUField(IDbConnection db, string invMoveId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InvMoveUserFieldExists(string invMoveId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateinvMoveUField(IDbConnection db, string invMoveId, string createdBy, dynamic data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateInvMoveUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateinvMoveUFieldMOD(IDbConnection db, string invMoveId, string modifiedBy, dynamic data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateInvMoveUserField(InvMoveUserFieldModel invMoveUserField)
        {
            throw new NotImplementedException();
        }
    }
}
