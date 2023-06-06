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
    public class OrganizationCore : IOrganizationCore
    {
        private IOrganizationRepository OrganizationRepo { get; set; }
        public OrganizationCore(IOrganizationRepository organizationRepo)
        {
            OrganizationRepo = organizationRepo;
        }

        public async Task<RequestResponse> GetOrgSpecial(OrganizationFilterMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            OrganizationPagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.OrganizationTypeId) ||
                filter.Inactive != null)
            {
                data = await OrganizationRepo.GetOrgFilteredPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await OrganizationRepo.GetOrgSearchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await OrganizationRepo.GetOrgPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOrganizationPg(int pageNum, int pageItem)
        {   
            var data = await OrganizationRepo.GetOrganizationPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOrganizationPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await OrganizationRepo.GetOrganizationPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOrgPaged(int pageNum, int pageItem)
        {
            var data = await OrganizationRepo.GetOrgPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOrgSearchPaged(string searchKey, int pageNum, int pageItem)
        {
            var data = await OrganizationRepo.GetOrgSearchPaged(searchKey, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOrganizationById(string organizationId)
        {
            var data = await OrganizationRepo.GetOrganizationById(organizationId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOrgPgByOrgTypId(string organizationTypeId, int pageNum, int pageItem)
        {
            var data = await OrganizationRepo.GetOrgPgByOrgTypId(organizationTypeId, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOrgPgFiltered(OrganizationFilterMdl filter, int pageNum, int pageItem)
        {
            var data = await OrganizationRepo.GetOrgPgFiltered(filter, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetOrgFilteredPaged(OrganizationFilterMdl filter, int pageNum, int pageItem)
        {
            var data = await OrganizationRepo.GetOrgFilteredPaged(filter, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateOrganization(OrganizationModel organization)
        {
            bool organizationExists = await OrganizationRepo.OrganizationExists(organization.OrganizationId);
            if (organizationExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar OrganizationId exists.");
            }

            bool res = await OrganizationRepo.CreateOrganization(organization);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateOrganization(OrganizationModel organization)
        {
            bool res = await OrganizationRepo.UpdateOrganization(organization);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteOrganization(string organizationId, string userAccountId)
        {
            // place item in use validator here
            var inUse = await OrganizationRepo.OrganizationInUse(organizationId);
            if (inUse)
            {
                return new RequestResponse(ResponseCode.FAILED, "Delete failed. Organization is in use.");
            }

            bool res = await OrganizationRepo.DeleteOrganization(organizationId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
