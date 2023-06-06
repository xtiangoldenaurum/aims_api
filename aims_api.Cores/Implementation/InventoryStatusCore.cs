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
    public class InventoryStatusCore : IInventoryStatusCore
    {
        private IInventoryStatusRepository InventoryStatusRepo { get; set; }
        public InventoryStatusCore(IInventoryStatusRepository inventoryStatusRepo)
        {
            InventoryStatusRepo = inventoryStatusRepo;
        }

        public async Task<RequestResponse> GetInventoryStatusPg(int pageNum, int pageItem)
        {   
            var data = await InventoryStatusRepo.GetInventoryStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInventoryStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InventoryStatusRepo.GetInventoryStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInventoryStatusById(string inventoryStatusId)
        {
            var data = await InventoryStatusRepo.GetInventoryStatusById(inventoryStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInventoryStatus(InventoryStatusModel inventoryStatus)
        {
            bool inventoryStatusExists = await InventoryStatusRepo.InventoryStatusExists(inventoryStatus.InventoryStatusId);
            if (inventoryStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InventoryStatusId exists.");
            }

            bool res = await InventoryStatusRepo.CreateInventoryStatus(inventoryStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateInventoryStatus(InventoryStatusModel inventoryStatus)
        {
            bool res = await InventoryStatusRepo.UpdateInventoryStatus(inventoryStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteInventoryStatus(string inventoryStatusId)
        {
			// place item in use validator here

            bool res = await InventoryStatusRepo.DeleteInventoryStatus(inventoryStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
