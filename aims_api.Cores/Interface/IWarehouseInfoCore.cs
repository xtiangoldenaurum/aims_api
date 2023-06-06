using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IWarehouseInfoCore
    {
        Task<RequestResponse> GetWarehouseInfoPg(int pageNum, int pageItem);
        Task<RequestResponse> GetWarehouseInfoPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetWarehouseInfoById(string warehouseId);
        Task<RequestResponse> CreateWarehouseInfo(WarehouseInfoModel warehouseInfo);
        Task<RequestResponse> UpdateWarehouseInfo(WarehouseInfoModel warehouseInfo);
        Task<RequestResponse> DeleteWarehouseInfo(string warehouseId);
    }
}
