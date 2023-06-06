using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IWarehouseInfoRepository
    {
        Task<IEnumerable<WarehouseInfoModel>> GetWarehouseInfoPg(int pageNum, int pageItem);
        Task<IEnumerable<WarehouseInfoModel>> GetWarehouseInfoPgSrch(string searchKey, int pageNum, int pageItem);
        Task<WarehouseInfoModel> GetWarehouseInfoById(string warehouseId);
        Task<bool> WarehouseInfoExists(string warehouseId);
        Task<bool> CreateWarehouseInfo(WarehouseInfoModel warehouseInfo);
        Task<bool> UpdateWarehouseInfo(WarehouseInfoModel warehouseInfo);
        Task<bool> DeleteWarehouseInfo(string warehouseId);
    }
}
