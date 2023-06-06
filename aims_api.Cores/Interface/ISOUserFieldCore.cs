using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ISOUserFieldCore
    {
        Task<RequestResponse> GetSOUserFieldById(string soId);
        Task<RequestResponse> GetSOUFields();
        Task<RequestResponse> CreateSOUField(string fieldName, string createdBy);
        Task<RequestResponse> UpdateSOUField(string oldFieldName, string newFieldName, string modifiedByd);
        Task<RequestResponse> DeleteSOUField(string fieldName, string userAccountId);
    }
}
