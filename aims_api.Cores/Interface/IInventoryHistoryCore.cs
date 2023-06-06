using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInventoryHistoryCore
    {
        Task<RequestResponse> GetInventoryHistoryPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInventoryHistoryPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInventoryHistoryById(string inventoryId);
        Task<RequestResponse> CreateInventoryHistory(InventoryHistoryModel inventoryHistory);
        Task<RequestResponse> UpdateInventoryHistory(InventoryHistoryModel inventoryHistory);
        Task<RequestResponse> DeleteInventoryHistory(string inventoryId);
    }
}
