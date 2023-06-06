using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IPackageTypeCore
    {
        Task<RequestResponse> GetPackageTypePg(int pageNum, int pageItem);
        Task<RequestResponse> GetPackageTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetPackageTypeById(string packageTypeId);
        Task<RequestResponse> CreatePackageType(PackageTypeModel packageType);
        Task<RequestResponse> UpdatePackageType(PackageTypeModel packageType);
        Task<RequestResponse> DeletePackageType(string packageTypeId);
    }
}
