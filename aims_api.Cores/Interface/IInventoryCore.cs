using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInventoryCore
    {
        Task<RequestResponse> GetInventoryPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInventoryPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInventoryById(string inventoryId);
        Task<RequestResponse> CreateInventory(InventoryModel inventory);
        Task<RequestResponse> UpdateInventory(InventoryModel inventory);
        Task<RequestResponse> DeleteInventory(string inventoryId);
    }
}
