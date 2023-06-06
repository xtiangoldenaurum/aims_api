using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInventoryStatusCore
    {
        Task<RequestResponse> GetInventoryStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInventoryStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInventoryStatusById(string inventoryStatusId);
        Task<RequestResponse> CreateInventoryStatus(InventoryStatusModel inventoryStatus);
        Task<RequestResponse> UpdateInventoryStatus(InventoryStatusModel inventoryStatus);
        Task<RequestResponse> DeleteInventoryStatus(string inventoryStatusId);
    }
}
