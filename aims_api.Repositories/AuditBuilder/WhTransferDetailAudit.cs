using aims_api.Enums;
using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aims_api.Repositories.AuditBuilder
{
    internal class WhTransferDetailAudit
    {
        public async Task<AuditTrailModel> BuildWHTransDtlAuditADD(WhTransferDetailModel whTransDetail, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(whTransDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = whTransDetail.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = whTransDetail.WhTransferLineId,
                    Data = data,
                    Remarks = "Warehouse Transfer Detail Created"
                };
            });
        }

        public async Task<AuditTrailModel> BuildWhTransDtlAuditMOD(WhTransferDetailModel whTransDetail, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(whTransDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = whTransDetail.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = whTransDetail.WhTransferLineId,
                    Data = data,
                    Remarks = "Warehouse Transfer Modified"
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(WhTransferDetailModel whTransDetail, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(whTransDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = whTransDetail.WhTransferLineId,
                    Data = data,
                    Remarks = "Warehouse Transfer Detail Deleted"
                };
            });
        }
    }
}
