using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvAdjustLineStatusRepository
    {
        Task<IEnumerable<InvAdjustLineStatusModel>> GetInvAdjustLineStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<InvAdjustLineStatusModel>> GetInvAdjustLineStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvAdjustLineStatusModel> GetInvAdjustLineStatusById(string invAdjustLineStatusId);
        Task<bool> InvAdjustLineStatusExists(string invAdjustLineStatusId);
        Task<bool> CreateInvAdjustLineStatus(InvAdjustLineStatusModel invAdjustLineStatus);
        Task<bool> UpdateInvAdjustLineStatus(InvAdjustLineStatusModel invAdjustLineStatus);
        Task<bool> DeleteInvAdjustLineStatus(string invAdjustLineStatusId);
    }
}
