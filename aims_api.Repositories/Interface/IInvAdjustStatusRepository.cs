using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvAdjustStatusRepository
    {
        Task<IEnumerable<InvAdjustStatusModel>> GetInvAdjustStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<InvAdjustStatusModel>> GetInvAdjustStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvAdjustStatusModel> GetInvAdjustStatusById(string invAdjustStatusId);
        Task<bool> InvAdjustStatusExists(string invAdjustStatusId);
        Task<bool> CreateInvAdjustStatus(InvAdjustStatusModel invAdjustStatus);
        Task<bool> UpdateInvAdjustStatus(InvAdjustStatusModel invAdjustStatus);
        Task<bool> DeleteInvAdjustStatus(string invAdjustStatusId);
    }
}
