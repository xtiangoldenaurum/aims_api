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
    public class OrganizationTypeCore : IOrganizationTypeCore
    {
        private IOrganizationTypeRepository OrganizationTypeRepo { get; set; }
        public OrganizationTypeCore(IOrganizationTypeRepository organizationTypeRepo)
        {
            OrganizationTypeRepo = organizationTypeRepo;
        }

        public async Task<RequestResponse> GetOrganizationTypePg(int pageNum, int pageItem)
        {   
            var data = await OrganizationTypeRepo.GetOrganizationTypePg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOrganizationTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await OrganizationTypeRepo.GetOrganizationTypePgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOrganizationTypeById(string organizationTypeID)
        {
            var data = await OrganizationTypeRepo.GetOrganizationTypeById(organizationTypeID);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateOrganizationType(OrganizationTypeModel organizationType)
        {
            bool organizationTypeExists = await OrganizationTypeRepo.OrganizationTypeExists(organizationType.OrganizationTypeID);
            if (organizationTypeExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar OrganizationTypeID exists.");
            }

            bool res = await OrganizationTypeRepo.CreateOrganizationType(organizationType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateOrganizationType(OrganizationTypeModel organizationType)
        {
            bool res = await OrganizationTypeRepo.UpdateOrganizationType(organizationType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteOrganizationType(string organizationTypeID)
        {
			// place item in use validator here

            bool res = await OrganizationTypeRepo.DeleteOrganizationType(organizationTypeID);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
