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
    public interface IInventoryHistoryRepository
    {
        Task<IEnumerable<InventoryHistoryModel>> GetInventoryHistoryPg(int pageNum, int pageItem);
        Task<IEnumerable<InventoryHistoryModel>> GetInventoryHistoryPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InventoryHistoryModel> GetInventoryHistoryById(string inventoryId);
        Task<bool> InventoryHistoryExists(string inventoryId);
        Task<bool> CreateInventoryHistory(InventoryHistoryModel inventoryHistory);
        Task<InventoryHistoryModel> GetTopInvHistDetail(IDbConnection db, string inventoryId);
        Task<InventoryHistoryModel> LockInvHistDetail(IDbConnection db, string inventoryId, int seqNum);
        Task<bool> CreateInventoryHistoryMod(IDbConnection db, InventoryHistoryModel inventoryHistory, TranType tranTyp);
        Task<bool> UpdateInventoryHistory(InventoryHistoryModel inventoryHistory);
        Task<bool> DeleteInventoryHistory(string inventoryId);
        Task<InventoryHistoryModel> GetInvHistoryByTrackId(IDbConnection db, string trackId);
        Task<bool> CheckTrackIdExists(IDbConnection db, string trackId);
        Task<bool> ChkLPNIsUsedInStorage(IDbConnection db, string lpnId);
        Task<bool> ChkLPNIsUsedInStorage(string lpnId);
        Task<InventoryHistoryModel> GetInvHistoryMaxSeqByInvId(IDbConnection db, string inventoryId);
    }
}
