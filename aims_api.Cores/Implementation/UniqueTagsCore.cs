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
    public class UniqueTagsCore : IUniqueTagsCore
    {
        private IUniqueTagsRepository UniqueTagsRepo { get; set; }
        public UniqueTagsCore(IUniqueTagsRepository uniqueTagsRepo)
        {
            UniqueTagsRepo = uniqueTagsRepo;
        }

        public async Task<RequestResponse> GetUniqueTagsPg(int pageNum, int pageItem)
        {   
            var data = await UniqueTagsRepo.GetUniqueTagsPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUniqueTagsPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await UniqueTagsRepo.GetUniqueTagsPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUniqueTagsById(int uniqueTagId)
        {
            var data = await UniqueTagsRepo.GetUniqueTagsById(uniqueTagId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateUniqueTags(UniqueTagsModel uniqueTags)
        {
            bool uniqueTagsExists = await UniqueTagsRepo.UniqueTagsExists(uniqueTags.UniqueTagId);
            if (uniqueTagsExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar UniqueTagId exists.");
            }

            bool res = await UniqueTagsRepo.CreateUniqueTags(uniqueTags);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateUniqueTags(UniqueTagsModel uniqueTags)
        {
            bool res = await UniqueTagsRepo.UpdateUniqueTags(uniqueTags);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteUniqueTags(int uniqueTagId)
        {
			// place item in use validator here

            bool res = await UniqueTagsRepo.DeleteUniqueTags(uniqueTagId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
