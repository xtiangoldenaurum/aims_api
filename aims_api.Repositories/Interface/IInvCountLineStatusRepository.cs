using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvCountLineStatusRepository
    {
        Task<IEnumerable<InvCountLineStatusModel>> GetInvCountLineStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<InvCountLineStatusModel>> GetInvCountLineStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvCountLineStatusModel> GetInvCountLineStatusById(string invCountLineStatusId);
        Task<bool> InvCountLineStatusExists(string invCountLineStatusId);
        Task<bool> CreateInvCountLineStatus(InvCountLineStatusModel invCountLineStatus);
        Task<bool> UpdateInvCountLineStatus(InvCountLineStatusModel invCountLineStatus);
        Task<bool> DeleteInvCountLineStatus(string invCountLineStatusId);
    }
}
