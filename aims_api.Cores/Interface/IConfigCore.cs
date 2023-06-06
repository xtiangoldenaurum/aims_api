using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IConfigCore
    {
        Task<RequestResponse> GetAllConfig();
        Task<RequestResponse> GetConfigPg(int pageNum, int pageItem);
        Task<RequestResponse> GetConfigPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetConfigUOMs();
        Task<RequestResponse> GetConfigById(string configType, string configName);
        Task<RequestResponse> CreateConfig(ConfigModel config);
        Task<RequestResponse> UpdateConfig(ConfigModel config);
        Task<RequestResponse> DeleteConfig(string configType, string configName, string userAccountId);
        Task<RequestResponse> DeleteUOMConfig(string configType, string configName, string userAccountId);
    }
}
