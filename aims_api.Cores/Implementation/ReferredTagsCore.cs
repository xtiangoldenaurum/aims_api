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
    public class ReferredTagsCore : IReferredTagsCore
    {
        private IReferredTagsRepository ReferredTagsRepo { get; set; }
        public ReferredTagsCore(IReferredTagsRepository referredTagsRepo)
        {
            ReferredTagsRepo = referredTagsRepo;
        }

        public async Task<RequestResponse> GetReferredTagsPg(int pageNum, int pageItem)
        {   
            var data = await ReferredTagsRepo.GetReferredTagsPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReferredTagsPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ReferredTagsRepo.GetReferredTagsPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReferredTagsById(int referredTagId)
        {
            var data = await ReferredTagsRepo.GetReferredTagsById(referredTagId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateReferredTags(ReferredTagsModel referredTags)
        {
            bool referredTagsExists = await ReferredTagsRepo.ReferredTagsExists(referredTags.ReferredTagId);
            if (referredTagsExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar ReferredTagId exists.");
            }

            bool res = await ReferredTagsRepo.CreateReferredTags(referredTags);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateReferredTags(ReferredTagsModel referredTags)
        {
            bool res = await ReferredTagsRepo.UpdateReferredTags(referredTags);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteReferredTags(int referredTagId)
        {
			// place item in use validator here

            bool res = await ReferredTagsRepo.DeleteReferredTags(referredTagId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
