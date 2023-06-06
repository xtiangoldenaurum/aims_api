using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvMoveLineStatusRepository
    {
        Task<IEnumerable<InvMoveLineStatusModel>> GetInvMoveLineStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<InvMoveLineStatusModel>> GetInvMoveLineStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvMoveLineStatusModel> GetInvMoveLineStatusById(string invMoveLineStatusId);
        Task<bool> InvMoveLineStatusExists(string invMoveLineStatusId);
        Task<bool> CreateInvMoveLineStatus(InvMoveLineStatusModel invMoveLineStatus);
        Task<bool> UpdateInvMoveLineStatus(InvMoveLineStatusModel invMoveLineStatus);
        Task<bool> DeleteInvMoveLineStatus(string invMoveLineStatusId);
    }
}
