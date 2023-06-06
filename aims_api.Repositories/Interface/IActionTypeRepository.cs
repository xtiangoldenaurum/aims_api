using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IActionTypeRepository
    {
        Task<IEnumerable<ActionTypeModel>> GetActionTypePg(int pageNum, int pageItem);
        Task<IEnumerable<ActionTypeModel>> GetActionTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<ActionTypeModel> GetActionTypeById(string actionTypeId);
        Task<bool> ActionTypeExists(string actionTypeId);
        Task<bool> CreateActionType(ActionTypeModel actionType);
        Task<bool> UpdateActionType(ActionTypeModel actionType);
        Task<bool> DeleteActionType(string actionTypeId);
    }
}
