using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IVehicleTypeCore
    {
        Task<RequestResponse> GetVehicleTypePg(int pageNum, int pageItem);
        Task<RequestResponse> GetVehicleTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetVehicleTypeById(string vehicleTypeId);
        Task<RequestResponse> CreateVehicleType(VehicleTypeModel vehicleType);
        Task<RequestResponse> UpdateVehicleType(VehicleTypeModel vehicleType);
        Task<RequestResponse> DeleteVehicleType(string vehicleTypeId, string userAccountId);
    }
}
