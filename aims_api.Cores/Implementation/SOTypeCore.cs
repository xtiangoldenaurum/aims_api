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
    public class SOTypeCore : ISOTypeCore
    {
        private ISOTypeRepository SOTypeRepo { get; set; }
        public SOTypeCore(ISOTypeRepository soTypeRepo)
        {
            SOTypeRepo = soTypeRepo;
        }

        public async Task<RequestResponse> GetSOTypePg(int pageNum, int pageItem)
        {   
            var data = await SOTypeRepo.GetSOTypePg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSOTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await SOTypeRepo.GetSOTypePgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSOTypeById(string soTypeId)
        {
            var data = await SOTypeRepo.GetSOTypeById(soTypeId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateSOType(SOTypeModel soType)
        {
            bool soTypeExists = await SOTypeRepo.SOTypeExists(soType.SoTypeId);
            if (soTypeExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar SoTypeId exists.");
            }

            bool res = await SOTypeRepo.CreateSOType(soType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateSOType(SOTypeModel soType)
        {
            bool res = await SOTypeRepo.UpdateSOType(soType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteSOType(string soTypeId)
        {
			// place item in use validator here

            bool res = await SOTypeRepo.DeleteSOType(soTypeId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
