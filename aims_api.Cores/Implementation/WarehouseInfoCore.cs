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
    public class WarehouseInfoCore : IWarehouseInfoCore
    {
        private IWarehouseInfoRepository WarehouseInfoRepo { get; set; }
        public WarehouseInfoCore(IWarehouseInfoRepository warehouseInfoRepo)
        {
            WarehouseInfoRepo = warehouseInfoRepo;
        }

        public async Task<RequestResponse> GetWarehouseInfoPg(int pageNum, int pageItem)
        {   
            var data = await WarehouseInfoRepo.GetWarehouseInfoPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetWarehouseInfoPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await WarehouseInfoRepo.GetWarehouseInfoPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetWarehouseInfoById(string warehouseId)
        {
            var data = await WarehouseInfoRepo.GetWarehouseInfoById(warehouseId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateWarehouseInfo(WarehouseInfoModel warehouseInfo)
        {
            bool warehouseInfoExists = await WarehouseInfoRepo.WarehouseInfoExists(warehouseInfo.WarehouseId);
            if (warehouseInfoExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar WarehouseId exists.");
            }

            bool res = await WarehouseInfoRepo.CreateWarehouseInfo(warehouseInfo);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateWarehouseInfo(WarehouseInfoModel warehouseInfo)
        {
            bool res = await WarehouseInfoRepo.UpdateWarehouseInfo(warehouseInfo);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteWarehouseInfo(string warehouseId)
        {
			// place item in use validator here

            bool res = await WarehouseInfoRepo.DeleteWarehouseInfo(warehouseId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
