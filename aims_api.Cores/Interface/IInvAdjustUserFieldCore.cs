using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvAdjustUserFieldCore
    {
        Task<RequestResponse> GetInvAdjustUserFieldPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustUserFieldPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustUserFieldById(string invAdjustId);
        Task<RequestResponse> GetInvAdjustUFields();
        Task<RequestResponse> CreateInvAdjustUField(string fieldName, string createdBy);
        Task<RequestResponse> UpdateInvAdjustUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<RequestResponse> DeleteInvAdjustUField(string fieldName, string userAccountId);
    }
}
