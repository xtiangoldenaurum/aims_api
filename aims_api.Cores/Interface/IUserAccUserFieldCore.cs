using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IUserAccUserFieldCore
    {
        Task<RequestResponse> GetAccUserFieldById(string userAccountId);
        Task<RequestResponse> GetUserAccountUFields();
        Task<RequestResponse> CreateUserAccUField(string fieldName, string createdBy);
        Task<RequestResponse> UpdateUserAccUField(string oldFieldName, string newFieldName, string modifiedByd);
        Task<RequestResponse> DeleteUserAccUField(string fieldName, string userAccountId);
    }
}
