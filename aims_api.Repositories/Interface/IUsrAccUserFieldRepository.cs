using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IUsrAccUserFieldRepository
    {
        Task<SOUserFieldModel> GetAccUserFieldById(string userAccountId);
        Task<dynamic?> GetUserAccountUFields();
        Task<string?> GetAccUsrFldQryFilter();
        Task<bool> CreateUserAccUField(string fieldName, string createdBy);
        Task<bool> InitUserAccUField(IDbConnection db, string userAccountId);
        Task<bool> UpdateUserAccUField(IDbConnection db, string userAccountId, string createdBy, dynamic data);
        Task<bool> UpdateUserAccUFieldMOD(IDbConnection db, string userAccountId, string modifiedBy, dynamic data);
        Task<bool> ChkColExists(string fieldName);
        Task<bool> UpdateUserAccUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<bool> DeleteUserAccUField(string fieldName, string userAccountId);
        Task<bool> UsrAccUserFieldExists(string soId);
        Task<bool> DeleteUserAccUField(string userAccountId);
    }
}
