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
    public class SOCore : ISOCore
    {
        private ISORepository SORepo { get; set; }
        private ISOUserFieldRepository SOUFieldRepo { get; set; }
        public SOCore(ISORepository soRepo, ISOUserFieldRepository sOUFieldRepo)
        {
            SORepo = soRepo;
            SOUFieldRepo = sOUFieldRepo;
        }

        public async Task<RequestResponse> GetSOPg(int pageNum, int pageItem)
        {   
            var data = await SORepo.GetSOPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSOPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await SORepo.GetSOPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSOById(string soId)
        {
            var data = await SORepo.GetSOById(soId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSOByIdMod(string soId)
        {
            var soHeader = await SORepo.GetSOById(soId);
            var userFields = await SOUFieldRepo.GetSOUserFieldById(soId);

            if (soHeader != null)
            {
                var data = new SOModelMod()
                {
                    SOHeader = soHeader,
                    SOUfields = userFields
                };

                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateSO(SOModel so)
        {
            bool soExists = await SORepo.SOExists(so.SoId);
            if (soExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar SoId exists.");
            }

            bool res = await SORepo.CreateSO(so);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateSO(SOModel so)
        {
            bool res = await SORepo.UpdateSO(so);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteSO(string soId)
        {
			// place item in use validator here

            bool res = await SORepo.DeleteSO(soId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<string> DownloadSOTemplate()
        {
            await Task.Delay(1000);

            return @"E:\Mark\AIMS\aims_api-main\aims_api-main\aims_api.Utilities\template\Outbound\SO_Template.xlsx";
        }
        public async Task<RequestResponse> ExportSO()
        {
            var data = await SORepo.ExportSO();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }
    }
}
