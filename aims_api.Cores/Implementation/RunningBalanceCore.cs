using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Implementation
{
    public class RunningBalanceCore : IRunningBalanceCore
    {
        private IRunningBalanceRepository RunningBalanceRepo { get; set; }
        public RunningBalanceCore(IRunningBalanceRepository runningBalanceRepo)
        {
            RunningBalanceRepo = runningBalanceRepo;
        }

        public async Task<RequestResponse> GetRunningBalancePg(int pageNum, int pageItem)
        {   
            var data = await RunningBalanceRepo.GetRunningBalancePg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetRunningBalancePgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await RunningBalanceRepo.GetRunningBalancePgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetRunningBalanceById(string movementTypeId)
        {
            var data = await RunningBalanceRepo.GetRunningBalanceById(movementTypeId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateRunningBalance(RunningBalanceModel runningBalance)
        {
            bool runningBalanceExists = await RunningBalanceRepo.RunningBalanceExists(runningBalance.MovementTypeId);
            if (runningBalanceExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar MovementTypeId exists.");
            }

            bool res = await RunningBalanceRepo.CreateRunningBalance(runningBalance);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateRunningBalance(RunningBalanceModel runningBalance)
        {
            bool res = await RunningBalanceRepo.UpdateRunningBalance(runningBalance);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteRunningBalance(string movementTypeId)
        {
			// place item in use validator here

            bool res = await RunningBalanceRepo.DeleteRunningBalance(movementTypeId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
