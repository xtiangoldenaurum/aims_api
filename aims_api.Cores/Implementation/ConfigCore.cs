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
    public class ConfigCore : IConfigCore
    {
        private IConfigRepository ConfigRepo { get; set; }
        public ConfigCore(IConfigRepository configRepo)
        {
            ConfigRepo = configRepo;
        }

        public async Task<RequestResponse> GetAllConfig()
        {
            var data = await ConfigRepo.GetAllConfig();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetConfigPg(int pageNum, int pageItem)
        {
            var data = await ConfigRepo.GetConfigPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetConfigPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ConfigRepo.GetConfigPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetConfigUOMs()
        {
            var data = await ConfigRepo.GetConfigUOMs();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetConfigById(string configType, string configName)
        {
            var data = await ConfigRepo.GetConfigById(configType, configName);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateConfig(ConfigModel config)
        {
            bool areaExists = await ConfigRepo.ConfigExists(config);
            if (areaExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar Config detail exists.");
            }

            bool res = await ConfigRepo.CreateConfig(config);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateConfig(ConfigModel config)
        {
            bool res = await ConfigRepo.Updateconfig(config);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteConfig(string configType, string configName, string userAccountId)
        {
            bool res = await ConfigRepo.Deleteconfig(configType, configName, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> DeleteUOMConfig(string configType, string configName, string userAccountId)
        {
            // check if config is inUse
            bool inUse = await ConfigRepo.ConfigInUse(configType);

            if (inUse)
            {
                return new RequestResponse(ResponseCode.FAILED, "Delete failed. UOM config is in use.");
            }

            bool res = await ConfigRepo.Deleteconfig(configType, configName, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
