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
    public class InventoryCore : IInventoryCore
    {
        private IInventoryRepository InventoryRepo { get; set; }
        public InventoryCore(IInventoryRepository inventoryRepo)
        {
            InventoryRepo = inventoryRepo;
        }

        public async Task<RequestResponse> GetInventoryPg(int pageNum, int pageItem)
        {   
            var data = await InventoryRepo.GetInventoryPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInventoryPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InventoryRepo.GetInventoryPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInventoryById(string inventoryId)
        {
            var data = await InventoryRepo.GetInventoryById(inventoryId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInventory(InventoryModel inventory)
        {
            bool inventoryExists = await InventoryRepo.InventoryExists(inventory.InventoryId);
            if (inventoryExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InventoryId exists.");
            }

            bool res = await InventoryRepo.CreateInventory(inventory);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateInventory(InventoryModel inventory)
        {
            bool res = await InventoryRepo.UpdateInventory(inventory);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteInventory(string inventoryId)
        {
			// place item in use validator here

            bool res = await InventoryRepo.DeleteInventory(inventoryId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
