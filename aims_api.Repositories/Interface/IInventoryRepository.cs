using aims_api.Enums;
using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<InventoryModel>> GetInventoryPg(int pageNum, int pageItem);
        Task<IEnumerable<InventoryModel>> GetInventoryPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InventoryModel> GetInventoryById(string inventoryId);
        Task<InventoryModel> GetInventoryByIdMod(IDbConnection db, string inventoryId);
        Task<bool> InventoryExists(string inventoryId);
        Task<bool> CreateInventory(InventoryModel inventory);
        Task<bool> CreateInventoryMod(IDbConnection db, InventoryModel inventory, string userAccountId, TranType tranTyp);
        Task<bool> UpdateInventory(InventoryModel inventory);
        Task<bool> SetInventoryStatus(string inventoryId, string invStatus);
        Task<bool> SetInventoryStatus(IDbConnection db, string inventoryId, string invStatus, string userAccountId, TranType tranTyp);
        Task<bool> DeleteInventory(string inventoryId);
        Task<InventoryModel> LockInventoryByInvId(IDbConnection db, string inventoryId);
    }

}
