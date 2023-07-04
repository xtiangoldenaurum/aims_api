using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aims_api.Repositories.AuditBuilder
{
    public class InvMoveUserFieldAudit
    {
        public async Task<AuditTrailModel> BuildTranAuditADD(string fieldName, string createdBy)
        {
            string actTyp = "ADD";
            string tranTyp = "CONFIG";

            // build data
            Dictionary<string, object> tempData = new Dictionary<string, object>();
            tempData.Add("FieldName", fieldName);
            tempData.Add("DateCreated", DateTime.Now);
            tempData.Add("CreatedBy", createdBy);
            tempData.Add("Remarks", "Inventory Movement user field created.");

            // data to json
            string data = JsonSerializer.Serialize(tempData);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = createdBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = $"UserField - {fieldName}",
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(string oldFieldName, string newFieldName, string modifiedBy)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";

            // build data
            Dictionary<string, object> tempData = new Dictionary<string, object>();
            tempData.Add("OldFieldName", oldFieldName);
            tempData.Add("NewFieldName", newFieldName);
            tempData.Add("DateModified", DateTime.Now);
            tempData.Add("ModifiedBy", modifiedBy);
            tempData.Add("Remarks", "Inventory Movement user field renamed.");

            // data to json
            string data = JsonSerializer.Serialize(tempData);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = modifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = $"UserField - {newFieldName}",
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(string fieldName, string userAccountId)
        {
            string actTyp = "DEL";
            string tranTyp = "CONFIG";

            // build data
            Dictionary<string, object> tempData = new Dictionary<string, object>();
            tempData.Add("FieldName", fieldName);
            tempData.Add("DateDeleted", DateTime.Now);
            tempData.Add("DeletedBy", userAccountId);
            tempData.Add("Remarks", "Inventory Movement user field deleted.");

            // data to json
            string data = JsonSerializer.Serialize(tempData);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = $"UserField - {fieldName}",
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditADDDyn(string invMoveId, string createdBy, dynamic prodUFeilds)
        {
            string actTyp = "ADD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(prodUFeilds);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = createdBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = $"{invMoveId} - UserFields",
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMODDyn(string invMoveId, string modifiedBy, dynamic prodUFeilds)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(prodUFeilds);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = modifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = $"{invMoveId} - UserFields",
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(string invMoveId, string userAccountId, dynamic prodUFeilds)
        {
            string actTyp = "DEL";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(prodUFeilds);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = $"{invMoveId} - UserFields",
                    Data = data
                };
            });
        }
    }
}
