using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ISOUserFieldRepository
    {
        Task<dynamic> GetSOUserFieldById(string soId);
        Task<dynamic?> GetSOUFields();
        Task<string?> GetSOUsrFldQryFilter();
        Task<bool> CreateSOUField(string fieldName, string createdBy);
        Task<bool> InitSOUField(IDbConnection db, string soId);
        Task<bool> UpdateSOUField(IDbConnection db, string soId, string createdBy, dynamic data);
        Task<bool> UpdateSOUFieldMOD(IDbConnection db, string soId, string modifiedBy, dynamic data);
        Task<bool> ChkColExists(string fieldName);
        Task<bool> UpdateSOUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<bool> DeleteSOUField(string fieldName, string userAccountId);
        Task<bool> SOUserFieldExists(string soId);
        Task<bool> CreateSOUserField(SOUserFieldModel soUserField);
        Task<bool> UpdateSOUserField(SOUserFieldModel soUserField);
        Task<bool> DeleteSOUserField(string soId);
    }
}
