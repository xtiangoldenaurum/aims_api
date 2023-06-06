using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IWhTransUserFieldRepository
    {
        Task<dynamic> GetWhTransUserFieldById(string whTransferId);
        Task<dynamic?> GetWhTransferUFields();
        Task<string?> GetWhTransUsrFldQryFilter();
        Task<bool> CreateWhTransferUField(string fieldName, string createdBy);
        Task<bool> InitWhTransferUField(IDbConnection db, string whTransferId);
        Task<bool> UpdateWhTransferUField(IDbConnection db, string whTransferId, string createdBy, dynamic data);
        Task<bool> UpdateWhTransUFieldMOD(IDbConnection db, string whTransferId, string modifiedBy, dynamic data);
        Task<bool> ChkColExists(string fieldName);
        Task<bool> UpdateWhTransferUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<bool> DeleteWhTransferUField(string fieldName, string userAccountId);
        Task<bool> DeleteWhTransUserField(string whTransferId);
    }
}
