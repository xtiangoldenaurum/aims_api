using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IConfigRepository
    {
        Task<IEnumerable<ConfigModel>> GetAllConfig();
        Task<IEnumerable<ConfigModel>> GetConfigPg(int pageNum, int pageItem);
        Task<IEnumerable<ConfigModel>> GetConfigPgSrch(string searchKey, int pageNum, int pageItem);
        Task<bool> ConfigExists(ConfigModel config);
        Task<IEnumerable<ConfigModel>> GetConfigUOMs();
        Task<ConfigModel> GetConfigById(string configType, string configName);
        Task<bool> ConfigInUse(string configType);
        Task<bool> CreateConfig(ConfigModel config);
        Task<bool> Updateconfig(ConfigModel config);
        Task<bool> Deleteconfig(string configType, string configName, string userAccountId);
    }
}
