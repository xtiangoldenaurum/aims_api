using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities;

namespace aims_api.Cores.Implementation
{
    public class AreaCore : IAreaCore
    {
        private IAreaRepository AreaRepo { get; set; }
        private IAuditTrailRepository AuditRepo { get; set; }
        public AreaCore(IAreaRepository areaRepo, IAuditTrailRepository auditRepo)
        {
            AreaRepo = areaRepo;
            AuditRepo = auditRepo;
        }

        public async Task<RequestResponse> GetAreaSpecial(string? searchKey, int pageNum, int pageItem)
        {
            AreaPagedMdl? data = null;
            bool skip = false;

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await AreaRepo.GetAreaSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await AreaRepo.GetAreaPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAllArea()
        {
            var data = await AreaRepo.GetAllArea();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAreaPg(int pageNum, int pageItem)
        {
            var data = await AreaRepo.GetAreaPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAreaPaged(int pageNum, int pageItem)
        {
            var data = await AreaRepo.GetAreaPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAreaPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await AreaRepo.GetAreaPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAreaSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            var data = await AreaRepo.GetAreaSrchPaged(searchKey, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAreaById(string areaId)
        {
            var data = await AreaRepo.GetAreaById(areaId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateArea(AreaModel area)
        {
            bool areaExists = await AreaRepo.AreaExists(area.AreaId);
            if (areaExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar AreaId exists.");
            }

            bool res = await AreaRepo.CreateArea(area);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateArea(AreaModel area)
        {
            bool res = await AreaRepo.UpdateArea(area);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteArea(string areaId, string userAccountId)
        {
            bool inUse = await AreaRepo.AreaInUse(areaId);
            if (inUse)
            {
                return new RequestResponse(ResponseCode.FAILED, "Failed to delete. AreaId is in use.");
            }

            bool res = await AreaRepo.DeleteArea(areaId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
