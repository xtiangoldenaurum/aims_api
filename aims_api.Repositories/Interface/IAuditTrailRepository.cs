using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IAuditTrailRepository
    {
        Task<IEnumerable<AuditTrailModel>> GetAuditTrailPg(int pageNum, int pageItem);
        Task<AuditTrailPagedMdl?> GetAuditTrailPaged(int pageNum, int pageItem);
        Task<IEnumerable<AuditTrailModel>> GetAuditTrailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<AuditTrailPagedMdl?> GetAuditTrailSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<AuditTrailModel> GetAuditTrailById(int auditId);
        Task<bool> AuditTrailExists(int auditId);
        Task<IEnumerable<AuditTrailModel>> GetAuditTrailPgFiltered(AuditTrailFilterMdl filter, int pageNum, int pageItem);
        Task<AuditTrailPagedMdl?> GetAuditTrailFltrPaged(AuditTrailFilterMdl filter, int pageNum, int pageItem);
        Task<bool> CreateAuditTrail(AuditTrailModel auditTrail);
        Task<bool> CreateAuditTrail(IDbConnection db, AuditTrailModel auditTrail);
        Task<bool> UpdateAuditTrail(AuditTrailModel auditTrail);
        Task<bool> DeleteAuditTrail(int auditId);
        Task<bool> AlterRecordId(IDbConnection db, string origRecordId, string newRecordId);
    }
}
