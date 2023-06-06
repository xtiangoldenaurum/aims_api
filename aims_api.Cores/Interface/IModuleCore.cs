using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IModuleCore
    {
        Task<RequestResponse> GetModulePg(int pageNum, int pageItem);
        Task<RequestResponse> GetModulePgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetModuleById(string moduleId);
        Task<RequestResponse> CreateModule(ModuleModel module);
        Task<RequestResponse> UpdateModule(ModuleModel module);
        Task<RequestResponse> DeleteModule(string moduleId);
        Task<RequestResponse> GetUserModules(string accessRightId);
    }
}
