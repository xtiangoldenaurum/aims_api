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
    public class InventoryHistoryCore : IInventoryHistoryCore
    {
        private IInventoryHistoryRepository InventoryHistoryRepo { get; set; }
        public InventoryHistoryCore(IInventoryHistoryRepository inventoryHistoryRepo)
        {
            InventoryHistoryRepo = inventoryHistoryRepo;
        }

        public async Task<RequestResponse> GetInventoryHistoryPg(int pageNum, int pageItem)
        {   
            var data = await InventoryHistoryRepo.GetInventoryHistoryPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInventoryHistoryPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InventoryHistoryRepo.GetInventoryHistoryPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInventoryHistoryById(string inventoryId)
        {
            var data = await InventoryHistoryRepo.GetInventoryHistoryById(inventoryId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInventoryHistory(InventoryHistoryModel inventoryHistory)
        {
            bool inventoryHistoryExists = await InventoryHistoryRepo.InventoryHistoryExists(inventoryHistory.InventoryId);
            if (inventoryHistoryExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InventoryId exists.");
            }

            bool res = await InventoryHistoryRepo.CreateInventoryHistory(inventoryHistory);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateInventoryHistory(InventoryHistoryModel inventoryHistory)
        {
            bool res = await InventoryHistoryRepo.UpdateInventoryHistory(inventoryHistory);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteInventoryHistory(string inventoryId)
        {
			// place item in use validator here

            bool res = await InventoryHistoryRepo.DeleteInventoryHistory(inventoryId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
