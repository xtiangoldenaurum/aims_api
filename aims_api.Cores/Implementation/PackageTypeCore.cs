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
    public class PackageTypeCore : IPackageTypeCore
    {
        private IPackageTypeRepository PackageTypeRepo { get; set; }
        public PackageTypeCore(IPackageTypeRepository packageTypeRepo)
        {
            PackageTypeRepo = packageTypeRepo;
        }

        public async Task<RequestResponse> GetPackageTypePg(int pageNum, int pageItem)
        {   
            var data = await PackageTypeRepo.GetPackageTypePg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPackageTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await PackageTypeRepo.GetPackageTypePgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPackageTypeById(string packageTypeId)
        {
            var data = await PackageTypeRepo.GetPackageTypeById(packageTypeId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreatePackageType(PackageTypeModel packageType)
        {
            bool packageTypeExists = await PackageTypeRepo.PackageTypeExists(packageType.PackageTypeId);
            if (packageTypeExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar PackageTypeId exists.");
            }

            bool res = await PackageTypeRepo.CreatePackageType(packageType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdatePackageType(PackageTypeModel packageType)
        {
            bool res = await PackageTypeRepo.UpdatePackageType(packageType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeletePackageType(string packageTypeId)
        {
			// place item in use validator here

            bool res = await PackageTypeRepo.DeletePackageType(packageTypeId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
