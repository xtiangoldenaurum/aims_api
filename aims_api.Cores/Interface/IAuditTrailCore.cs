using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IAuditTrailCore
    {
        Task<RequestResponse> GetAuditTrailSpecial(AuditTrailFilterMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetAuditTrailPg(int pageNum, int pageItem);
        Task<RequestResponse> GetAuditTrailPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetAuditTrailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetAuditTrailSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetAuditTrailById(int auditId);
        Task<RequestResponse> GetAuditTrailPgFiltered(AuditTrailFilterMdl filter, int pageNum, int pageItem);
        Task<RequestResponse> GetAuditTrailFltrPaged(AuditTrailFilterMdl filter, int pageNum, int pageItem);
        Task<RequestResponse> CreateAuditTrail(AuditTrailModel auditTrail);
        Task<RequestResponse> UpdateAuditTrail(AuditTrailModel auditTrail);
        Task<RequestResponse> DeleteAuditTrail(int auditId);
    }
}
