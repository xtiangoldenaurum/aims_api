using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Implementation
{
    public class TransactionTypeCore : ITransactionTypeCore
    {
        private ITransactionTypeRepository TranTypeRepo { get; set; }
        public TransactionTypeCore(ITransactionTypeRepository tranTypeRepo)
        {
            TranTypeRepo = tranTypeRepo;
        }

        public async Task<RequestResponse> GetAllTranType()
        {
            var data = await TranTypeRepo.GetAllTranType();

            if (data != null && data.Count() > 0)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }
    }
}
