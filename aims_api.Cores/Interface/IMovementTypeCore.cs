using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IMovementTypeCore
    {
        Task<RequestResponse> GetMovementTypePg(int pageNum, int pageItem);
        Task<RequestResponse> GetMovementTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetMovementTypeById(string movementTypeId);
        Task<RequestResponse> CreateMovementType(MovementTypeModel movementType);
        Task<RequestResponse> UpdateMovementType(MovementTypeModel movementType);
        Task<RequestResponse> DeleteMovementType(string movementTypeId);
    }
}
