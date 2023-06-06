using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IPOUserFieldCore
    {
        Task<RequestResponse> GetPOUserFieldPg(int pageNum, int pageItem);
        Task<RequestResponse> GetPOUserFieldPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetPOUserFieldById(string poId);
        Task<RequestResponse> GetPOUFields();
        Task<RequestResponse> CreatePOUField(string fieldName, string createdBy);
        Task<RequestResponse> UpdatePOUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<RequestResponse> DeletePOUField(string fieldName, string userAccountId);
    }
}
