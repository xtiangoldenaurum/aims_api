using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IWhTransUserFieldCore
    {
        Task<RequestResponse> GetWhTransUserFieldById(string whTransferId);
        Task<RequestResponse> GetWhTransferUFields();
        Task<RequestResponse> CreateWhTransferUField(string fieldName, string createdBy);
        Task<RequestResponse> UpdateWhTransferUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<RequestResponse> DeleteWhTransferUField(string fieldName, string userAccountId);
    }
}
