using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IPackageTypeRepository
    {
        Task<IEnumerable<PackageTypeModel>> GetPackageTypePg(int pageNum, int pageItem);
        Task<IEnumerable<PackageTypeModel>> GetPackageTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<PackageTypeModel> GetPackageTypeById(string packageTypeId);
        Task<bool> PackageTypeExists(string packageTypeId);
        Task<bool> CreatePackageType(PackageTypeModel packageType);
        Task<bool> UpdatePackageType(PackageTypeModel packageType);
        Task<bool> DeletePackageType(string packageTypeId);
    }
}
