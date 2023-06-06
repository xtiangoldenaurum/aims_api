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
    public class AuditTrailCore : IAuditTrailCore
    {
        private IAuditTrailRepository AuditTrailRepo { get; set; }
        public AuditTrailCore(IAuditTrailRepository auditTrailRepo)
        {
            AuditTrailRepo = auditTrailRepo;
        }

        public async Task<RequestResponse> GetAuditTrailSpecial(AuditTrailFilterMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            AuditTrailPagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.RecordId) ||
                !string.IsNullOrEmpty(filter.UserAccountId) ||
                !string.IsNullOrEmpty(filter.TransactionTypeId) ||
                filter.AuditDateFrom != null ||
                filter.AuditDateTo != null)
            {
                data = await AuditTrailRepo.GetAuditTrailFltrPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await AuditTrailRepo.GetAuditTrailSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await AuditTrailRepo.GetAuditTrailPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAuditTrailPg(int pageNum, int pageItem)
        {   
            var data = await AuditTrailRepo.GetAuditTrailPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAuditTrailPaged(int pageNum, int pageItem)
        {
            var data = await AuditTrailRepo.GetAuditTrailPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAuditTrailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await AuditTrailRepo.GetAuditTrailPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAuditTrailSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            var data = await AuditTrailRepo.GetAuditTrailSrchPaged(searchKey, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAuditTrailById(int auditId)
        {
            var data = await AuditTrailRepo.GetAuditTrailById(auditId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAuditTrailPgFiltered(AuditTrailFilterMdl filter, int pageNum, int pageItem)
        {
            var data = await AuditTrailRepo.GetAuditTrailPgFiltered(filter, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAuditTrailFltrPaged(AuditTrailFilterMdl filter, int pageNum, int pageItem)
        {
            var data = await AuditTrailRepo.GetAuditTrailFltrPaged(filter, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateAuditTrail(AuditTrailModel auditTrail)
        {
            bool auditTrailExists = await AuditTrailRepo.AuditTrailExists(auditTrail.AuditId);
            if (auditTrailExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar AreaId exists.");
            }

            bool res = await AuditTrailRepo.CreateAuditTrail(auditTrail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateAuditTrail(AuditTrailModel auditTrail)
        {
            bool res = await AuditTrailRepo.UpdateAuditTrail(auditTrail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteAuditTrail(int auditId)
        {
			// place item in use validator here

            bool res = await AuditTrailRepo.DeleteAuditTrail(auditId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
