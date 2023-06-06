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
    public class ModuleCore : IModuleCore
    {
        private IModuleRepository ModuleRepo { get; set; }
        public ModuleCore(IModuleRepository moduleRepo)
        {
            ModuleRepo = moduleRepo;
        }

        public async Task<RequestResponse> GetModulePg(int pageNum, int pageItem)
        {   
            var data = await ModuleRepo.GetModulePg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetModulePgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ModuleRepo.GetModulePgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetModuleById(string moduleId)
        {
            var data = await ModuleRepo.GetModuleById(moduleId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateModule(ModuleModel module)
        {
            bool moduleExists = await ModuleRepo.ModuleExists(module.ModuleId);
            if (moduleExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar ModuleId exists.");
            }

            bool res = await ModuleRepo.CreateModule(module);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateModule(ModuleModel module)
        {
            bool res = await ModuleRepo.UpdateModule(module);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteModule(string moduleId)
        {
			// place item in use validator here

            bool res = await ModuleRepo.DeleteModule(moduleId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> GetUserModules(string accessRightId)
        {
            var data = await ModuleRepo.GetUserModules(accessRightId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }
    }
}
