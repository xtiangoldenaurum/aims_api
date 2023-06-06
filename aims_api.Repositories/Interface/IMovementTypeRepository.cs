using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IMovementTypeRepository
    {
        Task<IEnumerable<MovementTypeModel>> GetMovementTypePg(int pageNum, int pageItem);
        Task<IEnumerable<MovementTypeModel>> GetMovementTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<MovementTypeModel> GetMovementTypeById(string movementTypeId);
        Task<bool> MovementTypeExists(string movementTypeId);
        Task<bool> CreateMovementType(MovementTypeModel movementType);
        Task<bool> UpdateMovementType(MovementTypeModel movementType);
        Task<bool> DeleteMovementType(string movementTypeId);
    }
}
