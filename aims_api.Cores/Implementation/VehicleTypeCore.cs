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
    public class VehicleTypeCore : IVehicleTypeCore
    {
        private IVehicleTypeRepository VehicleTypeRepo { get; set; }
        public VehicleTypeCore(IVehicleTypeRepository vehicleTypeRepo)
        {
            VehicleTypeRepo = vehicleTypeRepo;
        }

        public async Task<RequestResponse> GetVehicleTypePg(int pageNum, int pageItem)
        {   
            var data = await VehicleTypeRepo.GetVehicleTypePg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetVehicleTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await VehicleTypeRepo.GetVehicleTypePgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetVehicleTypeById(string vehicleTypeId)
        {
            var data = await VehicleTypeRepo.GetVehicleTypeById(vehicleTypeId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateVehicleType(VehicleTypeModel vehicleType)
        {
            bool vehicleTypeExists = await VehicleTypeRepo.VehicleTypeExists(vehicleType.VehicleTypeId);
            if (vehicleTypeExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar VehicleTypeId exists.");
            }

            bool res = await VehicleTypeRepo.CreateVehicleType(vehicleType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateVehicleType(VehicleTypeModel vehicleType)
        {
            bool res = await VehicleTypeRepo.UpdateVehicleType(vehicleType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteVehicleType(string vehicleTypeId, string userAccountId)
        {
			// place item in use validator here

            bool res = await VehicleTypeRepo.DeleteVehicleType(vehicleTypeId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
