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
    public class LocationTypeCore : ILocationTypeCore
    {
        private ILocationTypeRepository LocationTypeRepo { get; set; }
        public LocationTypeCore(ILocationTypeRepository locationTypeRepo)
        {
            LocationTypeRepo = locationTypeRepo;
        }

        public async Task<RequestResponse> GetLocationTypePg(int pageNum, int pageItem)
        {   
            var data = await LocationTypeRepo.GetLocationTypePg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await LocationTypeRepo.GetLocationTypePgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationTypeById(string locationTypeId)
        {
            var data = await LocationTypeRepo.GetLocationTypeById(locationTypeId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateLocationType(LocationTypeModel locationType)
        {
            bool locationTypeExists = await LocationTypeRepo.LocationTypeExists(locationType.LocationTypeId);
            if (locationTypeExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar LocationTypeId exists.");
            }

            bool res = await LocationTypeRepo.CreateLocationType(locationType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateLocationType(LocationTypeModel locationType)
        {
            bool res = await LocationTypeRepo.UpdateLocationType(locationType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteLocationType(string locationTypeId, string userAccountId)
        {
			// place item in use validator here

            bool res = await LocationTypeRepo.DeleteLocationType(locationTypeId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
