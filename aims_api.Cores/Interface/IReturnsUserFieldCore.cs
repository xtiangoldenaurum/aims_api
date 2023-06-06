using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IReturnsUserFieldCore
    {
        Task<RequestResponse> GetReturnsUserFieldPg(int pageNum, int pageItem);
        Task<RequestResponse> GetReturnsUserFieldPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetReturnsUserFieldById(string returnsId);
        Task<RequestResponse> GetReturnsUFields();
        Task<RequestResponse> CreateReturnsUField(string fieldName, string createdBy);
        Task<RequestResponse> UpdateReturnsUField(string oldFieldName, string newFieldName, string modifiedByd);
        Task<RequestResponse> DeleteReturnsUField(string fieldName, string userAccountId);
    }
}
