using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IRunningBalanceRepository
    {
        Task<IEnumerable<RunningBalanceModel>> GetRunningBalancePg(int pageNum, int pageItem);
        Task<IEnumerable<RunningBalanceModel>> GetRunningBalancePgSrch(string searchKey, int pageNum, int pageItem);
        Task<RunningBalanceModel> GetRunningBalanceById(string movementTypeId);
        Task<bool> RunningBalanceExists(string movementTypeId);
        Task<bool> CreateRunningBalance(RunningBalanceModel runningBalance);
        Task<bool> UpdateRunningBalance(RunningBalanceModel runningBalance);
        Task<bool> DeleteRunningBalance(string movementTypeId);
    }
}
