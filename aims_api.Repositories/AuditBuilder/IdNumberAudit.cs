using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aims_api.Repositories.AuditBuilder
{
    internal class IdNumberAudit
    {
        public async Task<AuditTrailModel> BuildTranAuditMOD(IdNumberModel idNumber)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(idNumber);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = idNumber.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = idNumber.TransactionTypeId,
                    Data = data,
                    Remarks = "Document IdNumber modfied",
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDocNum(string tranTypeId, string nxtDocNum, string userAccountId)
        {
            // build data
            IDictionary<string, object> tempData = new Dictionary<string, object>();
            tempData.Add("Table", "idnumber");
            tempData.Add("Info", "Generate next document number.");
            tempData.Add("TranTypeId", tranTypeId);
            tempData.Add("NxtDocNum", nxtDocNum);

            string actTyp = "MOD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(tempData);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = tranTypeId,
                    Data = data,
                    Remarks = $"Next document number aquired for: {tranTyp}, NxtDocNum: {nxtDocNum}",
                };
            });
        }
    }
}
