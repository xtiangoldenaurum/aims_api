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
    public class LocationGroupCore : ILocationGroupCore
    {
        private ILocationGroupRepository LocationGroupRepo { get; set; }
        public LocationGroupCore(ILocationGroupRepository locationGroupRepo)
        {
            LocationGroupRepo = locationGroupRepo;
        }

        public async Task<RequestResponse> GetLocationGroupPg(int pageNum, int pageItem)
        {   
            var data = await LocationGroupRepo.GetLocationGroupPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationGroupPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await LocationGroupRepo.GetLocationGroupPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationGroupById(string locationGroupId)
        {
            var data = await LocationGroupRepo.GetLocationGroupById(locationGroupId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateLocationGroup(LocationGroupModel locationGroup)
        {
            bool locationGroupExists = await LocationGroupRepo.LocationGroupExists(locationGroup.LocationGroupId);
            if (locationGroupExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar LocationGroupId exists.");
            }

            bool res = await LocationGroupRepo.CreateLocationGroup(locationGroup);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateLocationGroup(LocationGroupModel locationGroup)
        {
            bool res = await LocationGroupRepo.UpdateLocationGroup(locationGroup);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteLocationGroup(string locationGroupId, string userAccountId)
        {
			// place item in use validator here

            bool res = await LocationGroupRepo.DeleteLocationGroup(locationGroupId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
