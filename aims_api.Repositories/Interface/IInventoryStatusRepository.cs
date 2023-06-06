using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInventoryStatusRepository
    {
        Task<IEnumerable<InventoryStatusModel>> GetInventoryStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<InventoryStatusModel>> GetInventoryStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InventoryStatusModel> GetInventoryStatusById(string inventoryStatusId);
        Task<bool> InventoryStatusExists(string inventoryStatusId);
        Task<bool> CreateInventoryStatus(InventoryStatusModel inventoryStatus);
        Task<bool> UpdateInventoryStatus(InventoryStatusModel inventoryStatus);
        Task<bool> DeleteInventoryStatus(string inventoryStatusId);
    }
}
