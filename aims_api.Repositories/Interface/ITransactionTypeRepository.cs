    using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ITransactionTypeRepository
    {
        Task<IEnumerable<TransactionTypeModel>> GetAllTranType();
    }
}
