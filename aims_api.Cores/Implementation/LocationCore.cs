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
    public class LocationCore : ILocationCore
    {
        private ILocationRepository LocationRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public LocationCore(ILocationRepository locationRepo, EnumHelper enumHelper)
        {
            LocationRepo = locationRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetLocationSpecial(LocationFilterMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            LocationPagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.LocationTypeId) ||
                !string.IsNullOrEmpty(filter.LocationGroupId) ||
                !string.IsNullOrEmpty(filter.AreaId) ||
                filter.Inactive == null ||
                !string.IsNullOrEmpty(filter.AisleCode) ||
                !string.IsNullOrEmpty(filter.BayCode))
            {
                data = await LocationRepo.GetLocationFltrPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await LocationRepo.GetLocationSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await LocationRepo.GetLocationPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationPg(int pageNum, int pageItem)
        {   
            var data = await LocationRepo.GetLocationPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationPaged(int pageNum, int pageItem)
        {
            var data = await LocationRepo.GetLocationPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInbStatingLocationPaged(int pageNum, int pageItem)
        {
            var data = await LocationRepo.GetInbStatingLocationPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOutStatingLocationPaged(int pageNum, int pageItem)
        {
            var data = await LocationRepo.GetOutStatingLocationPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await LocationRepo.GetLocationPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            var data = await LocationRepo.GetLocationSrchPaged(searchKey, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationById(string locationId)
        {
            var data = await LocationRepo.GetLocationById(locationId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetDIstinctAisle()
        {
            var data = await LocationRepo.GetDIstinctAisle();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetDIstinctBay()
        {
            var data = await LocationRepo.GetDIstinctBay();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationPgFiltered(LocationFilterMdl filter, int pageNum, int pageItem)
        {
            var data = await LocationRepo.GetLocationPgFiltered(filter, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLocationFltrPaged(LocationFilterMdl filter, int pageNum, int pageItem)
        {
            var data = await LocationRepo.GetLocationFltrPaged(filter, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateLocation(LocationModel location)
        {
            bool locationExists = await LocationRepo.LocationExists(location.LocationId);
            if (locationExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar LocationId exists.");
            }

            bool res = await LocationRepo.CreateLocation(location);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateLocation(LocationModel location)
        {
            bool res = await LocationRepo.UpdateLocation(location);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteLocation(string locationId, string userAccountId)
        {
            // check if location is in use
            var inUse = await LocationRepo.LocationInUse(locationId);
            if (inUse)
            {
                return new RequestResponse(ResponseCode.FAILED, "Delete failed. Location is in use.");
            }

            bool res = await LocationRepo.DeleteLocation(locationId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> DefineTargetLocation(string locVCode)
        {
            var res = await LocationRepo.DefineTargetLocation(locVCode);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == PutawayResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.TargetLoc);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg);
        }

        public async Task<RequestResponse> DefineLPNPutawayLoc(string lpnId, string locVCode)
        {
            var res = await LocationRepo.DefineLPNPutawayLoc(lpnId, locVCode);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == PutawayResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.TargetLoc);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg);
        }
    }
}
