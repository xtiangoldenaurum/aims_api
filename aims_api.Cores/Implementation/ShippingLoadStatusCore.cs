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
    public class ShippingLoadStatusCore : IShippingLoadStatusCore
    {
        private IShippingLoadStatusRepository ShippingLoadStatusRepo { get; set; }
        public ShippingLoadStatusCore(IShippingLoadStatusRepository shippingLoadStatusRepo)
        {
            ShippingLoadStatusRepo = shippingLoadStatusRepo;
        }

        public async Task<RequestResponse> GetShippingLoadStatusPg(int pageNum, int pageItem)
        {   
            var data = await ShippingLoadStatusRepo.GetShippingLoadStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetShippingLoadStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ShippingLoadStatusRepo.GetShippingLoadStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetShippingLoadStatusById(string shippingLoadStatusId)
        {
            var data = await ShippingLoadStatusRepo.GetShippingLoadStatusById(shippingLoadStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateShippingLoadStatus(ShippingLoadStatusModel shippingLoadStatus)
        {
            bool shippingLoadStatusExists = await ShippingLoadStatusRepo.ShippingLoadStatusExists(shippingLoadStatus.ShippingLoadStatusId);
            if (shippingLoadStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar ShippingLoadStatusId exists.");
            }

            bool res = await ShippingLoadStatusRepo.CreateShippingLoadStatus(shippingLoadStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateShippingLoadStatus(ShippingLoadStatusModel shippingLoadStatus)
        {
            bool res = await ShippingLoadStatusRepo.UpdateShippingLoadStatus(shippingLoadStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteShippingLoadStatus(string shippingLoadStatusId)
        {
			// place item in use validator here

            bool res = await ShippingLoadStatusRepo.DeleteShippingLoadStatus(shippingLoadStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
