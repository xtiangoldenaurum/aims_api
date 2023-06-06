using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvMoveStatusRepository
    {
        Task<IEnumerable<InvMoveStatusModel>> GetInvMoveStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<InvMoveStatusModel>> GetInvMoveStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvMoveStatusModel> GetInvMoveStatusById(string invMoveStatusId);
        Task<bool> InvMoveStatusExists(string invMoveStatusId);
        Task<bool> CreateInvMoveStatus(InvMoveStatusModel invMoveStatus);
        Task<bool> UpdateInvMoveStatus(InvMoveStatusModel invMoveStatus);
        Task<bool> DeleteInvMoveStatus(string invMoveStatusId);
    }
}
