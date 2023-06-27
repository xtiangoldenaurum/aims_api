using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvMoveRepository
    {
        Task<InvMoveCreateTranResult> CreateInvMoveMod(InvMoveModelMod invMove);
        Task<bool> CreateInvMove(IDbConnection db, InvMoveModel invMove);
    }
}
