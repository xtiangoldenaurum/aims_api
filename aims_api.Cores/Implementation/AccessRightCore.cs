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
    public class AccessRightCore : IAccessRightCore
    {
        private IAccessRightRepository AccessRightRepo { get; set; }
        private IAccessRightDetailRepository AccessRightDetailRepo { get; set; }
        public AccessRightCore(IAccessRightRepository accessRightRepo, IAccessRightDetailRepository accessRightDetailRepo)
        {
            AccessRightRepo = accessRightRepo;
            AccessRightDetailRepo = accessRightDetailRepo;
        }

        public async Task<RequestResponse> GetAccessRightsSpecial(string? searchKey, int pageNum, int pageItem)
        {
            AccessRightPagedMdl? data = null;
            bool skip = false;

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await AccessRightRepo.GetAccessRightSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await AccessRightRepo.GetAccessRightPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAllAccessRight()
        {
            var data = await AccessRightRepo.GetAllAccessRight();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAccessRightPg(int pageNum, int pageItem)
        {
            var data = await AccessRightRepo.GetAccessRightPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAccessRightPaged(int pageNum, int pageItem)
        {
            var data = await AccessRightRepo.GetAccessRightPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAccessRightPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await AccessRightRepo.GetAccessRightPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAccessRightSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            var data = await AccessRightRepo.GetAccessRightSrchPaged(searchKey, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAccessRightById(string accessRightId)
        {
            var data = await AccessRightRepo.GetAccessRightById(accessRightId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateAccessRightHeader(AccessRightModel accessRight)
        {
            // check if similar accessright exists
            var headerExists = await AccessRightRepo.AccessRightExists(accessRight.AccessRightId);
            if (headerExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar AccessRighId exists.");
            }

            var result = await AccessRightRepo.CreateAccessRightHeader(accessRight);
            if (result)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateAccessRight(AccessRightHDModel accessRightHD)
        {
            if (accessRightHD.Header != null && accessRightHD.Details != null)
            {
                // limit edit of admin account
                if (!string.IsNullOrEmpty(accessRightHD.Header.AccessRightId))
                {
                    if (accessRightHD.Header.AccessRightId.ToUpper() == "ADMIN")
                    {
                        return new RequestResponse(ResponseCode.FAILED, "Admin account modification is not allow.");
                    }
                }

                var result = await AccessRightRepo.UpdateAccessRight(accessRightHD.Header, accessRightHD.Details);
                if (result)
                {
                    return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
                }
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteAccessRight(string accessRightId, string userAccountId)
        {
            // limit edit of admin account
            if (accessRightId.ToUpper() == "ADMIN")
            {
                return new RequestResponse(ResponseCode.FAILED, "Admin account modification is not allow.");
            }

            // check if accessright is in use
            var inUse = await AccessRightRepo.AccessRightInUse(accessRightId);
            if (inUse)
            {
                return new RequestResponse(ResponseCode.FAILED, "Delete failed. AccessRight is in use.");
            }

            // proceed deletion of accessright
            var del = await AccessRightRepo.DeleteAccessRight(accessRightId, userAccountId);
            if (del)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
