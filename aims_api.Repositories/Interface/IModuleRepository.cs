using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IModuleRepository
    {
        Task<IEnumerable<ModuleModel>> GetModulePg(int pageNum, int pageItem);
        Task<IEnumerable<ModuleModel>> GetModulePgSrch(string searchKey, int pageNum, int pageItem);
        Task<ModuleModel> GetModuleById(string moduleId);
        Task<bool> ModuleExists(string moduleId);
        Task<bool> CreateModule(ModuleModel module);
        Task<bool> UpdateModule(ModuleModel module);
        Task<bool> DeleteModule(string moduleId);
        Task<IEnumerable<ModuleModel>> GetUserModules(string accessRightId);
    }
}
