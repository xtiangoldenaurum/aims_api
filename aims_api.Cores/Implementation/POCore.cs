using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Implementation
{
    public class POCore : IPOCore
    {
        private IPORepository PORepo { get; set; }
        private IPOUserFieldRepository POUFieldRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public POCore(IPORepository poRepo, IPOUserFieldRepository pOUFieldRepo, EnumHelper enumHelper)
        {
            PORepo = poRepo;
            POUFieldRepo = pOUFieldRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetPOSpecial(POFilteredMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            POPagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.SupplierId) || 
                !string.IsNullOrEmpty(filter.CarrierId) || 
                filter.OrderDate != null ||
                !string.IsNullOrEmpty(filter.PoStatusId))
            {
                data = await PORepo.GetPOFilteredPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await PORepo.GetPOSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await PORepo.GetPOPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOForRcvPaged(int pageNum, int pageItem)
        {
            var data = await PORepo.GetPOForRcvPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOPg(int pageNum, int pageItem)
        {   
            var data = await PORepo.GetPOPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await PORepo.GetPOPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOById(string poId)
        {
            var data = await PORepo.GetPOById(poId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOByIdMod(string poId)
        {
            var poHeader = await PORepo.GetPOById(poId);
            var userFields = await POUFieldRepo.GetPOUserFieldById(poId);

            if (poHeader != null)
            {
                var data = new POModelMod()
                {
                    POHeader = poHeader,
                    POUfields = userFields
                };

                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreatePOMod(POModelMod po)
        {
            var res = await PORepo.CreatePOMod(po);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == POTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.POId);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res.ResultCode).ToString());
        }

        public async Task<RequestResponse> UpdatePOMod(POModelMod po)
        {
            var res = await PORepo.UpdatePOMod(po);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == POTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        //public async Task<RequestResponse> CreatePO(POModel po)
        //{
        //    bool poExists = await PORepo.POExists(po.PoId);
        //    if (poExists)
        //    {
        //        return new RequestResponse(ResponseCode.FAILED, "Similar PoId exists.");
        //    }

        //    bool res = await PORepo.CreatePO(po);
        //    if (res)
        //    {
        //        return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
        //    }

        //    return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        //}

        //public async Task<RequestResponse> UpdatePO(POModel po)
        //{
        //    bool res = await PORepo.UpdatePO(po);
        //    if (res)
        //    {
        //        return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
        //    }

        //    return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        //}

        public async Task<RequestResponse> DeletePO(string poId)
        {
			// place item in use validator here

            bool res = await PORepo.DeletePO(poId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> CancelPO(string poId, string userAccountId)
        {
            var res = await PORepo.CancelPO(poId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelPOResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> ForceCancelPO(string poId, string userAccountId)
        {
            var res = await PORepo.ForceCancelPO(poId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelPOResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<string> DownloadPOTemplate()
        {
            await Task.Delay(1000);

            return @"E:\Mark\AIMS\aims_api-main\aims_api-main\aims_api.Utilities\template\Inbound\PO_Template.xlsx";
        }

        public async Task<RequestResponse> ExportPO()
        {
            var data = await PORepo.ExportPO();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");

            //var data = await PORepo.ExportPO();

            //using (var package = new ExcelPackage())
            //{
            //    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            //    // Set headers
            //    worksheet.Cells[1, 1].Value = "PO No.";
            //    worksheet.Cells[1, 2].Value = "Order Date";
            //    worksheet.Cells[1, 3].Value = "Reference No.";
            //    worksheet.Cells[1, 4].Value = "Supplier";
            //    worksheet.Cells[1, 5].Value = "Order Status";
            //    // Add more headers as needed

            //    // Populate data
            //    var row = 2;
            //    foreach (var item in data)
            //    {
            //        worksheet.Cells[row, 1].Value = item.PoId;
            //        worksheet.Cells[row, 2].Value = item.OrderDate;
            //        worksheet.Cells[row, 3].Value = item.RefNumber;
            //        worksheet.Cells[row, 4].Value = item.SupplierName;
            //        worksheet.Cells[row, 5].Value = item.PoStatusId;
            //        // Add more properties as needed

            //        row++;
            //    }

            //    // Generate the file
            //    var stream = new MemoryStream(package.GetAsByteArray());

            //    // Return the file as an attachment
            //    var response = File(stream, "application/octet-stream");
            //    response.FileDownloadName = "POList.xlsx";
            //    return response;
            //}
        }

        public async Task<RequestResponse> ImportPOData(IFormFile file)
        {
            var res = await PORepo.ImportPOData(file);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == POTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }
    }
}
