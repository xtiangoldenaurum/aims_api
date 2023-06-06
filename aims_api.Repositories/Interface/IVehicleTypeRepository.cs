using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IVehicleTypeRepository
    {
        Task<IEnumerable<VehicleTypeModel>> GetVehicleTypePg(int pageNum, int pageItem);
        Task<IEnumerable<VehicleTypeModel>> GetVehicleTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<VehicleTypeModel> GetVehicleTypeById(string vehicleTypeId);
        Task<bool> VehicleTypeExists(string vehicleTypeId);
        Task<bool> CreateVehicleType(VehicleTypeModel vehicleType);
        Task<bool> UpdateVehicleType(VehicleTypeModel vehicleType);
        Task<bool> DeleteVehicleType(string vehicleTypeId);
    }
}
