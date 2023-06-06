using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvCountStatusRepository
    {
        Task<IEnumerable<InvCountStatusModel>> GetInvCountStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<InvCountStatusModel>> GetInvCountStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvCountStatusModel> GetInvCountStatusById(string invCountStatusId);
        Task<bool> InvCountStatusExists(string invCountStatusId);
        Task<bool> CreateInvCountStatus(InvCountStatusModel invCountStatus);
        Task<bool> UpdateInvCountStatus(InvCountStatusModel invCountStatus);
        Task<bool> DeleteInvCountStatus(string invCountStatusId);
    }
}
