using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvMoveUserFieldCore
    {
        Task<RequestResponse> GetInvMoveUserFieldPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveUserFieldPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveUserFieldById(string invMoveId);
        Task<RequestResponse> GetInvMoveUFields();
        Task<RequestResponse> CreateInvMoveUField(string fieldName, string createdBy);
        Task<RequestResponse> UpdateInvMoveUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<RequestResponse> DeleteInvMoveUField(string fieldName, string userAccountId);
    }
}
