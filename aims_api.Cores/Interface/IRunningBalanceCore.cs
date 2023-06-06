using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IRunningBalanceCore
    {
        Task<RequestResponse> GetRunningBalancePg(int pageNum, int pageItem);
        Task<RequestResponse> GetRunningBalancePgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetRunningBalanceById(string movementTypeId);
        Task<RequestResponse> CreateRunningBalance(RunningBalanceModel runningBalance);
        Task<RequestResponse> UpdateRunningBalance(RunningBalanceModel runningBalance);
        Task<RequestResponse> DeleteRunningBalance(string movementTypeId);
    }
}
