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
    internal class SODetailAudit
    {
        public async Task<AuditTrailModel> BuildSODtlAuditADD(SODetailModel soDetail, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(soDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = soDetail.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = soDetail.SoLineId,
                    Data = data,
                    Remarks = "SO Detail Created"
                };
            });
        }

        public async Task<AuditTrailModel> BuildSODtlAuditMOD(SODetailModel soDetail, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(soDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = soDetail.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = soDetail.SoLineId,
                    Data = data,
                    Remarks = "SO Detail Modified"
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(SODetailModel soDetail, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(soDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = soDetail.SoLineId,
                    Data = data,
                    Remarks = "SO Detail Deleted"
                };
            });
        }
    }
}
