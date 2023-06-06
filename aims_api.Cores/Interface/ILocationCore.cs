using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ILocationCore
    {
        Task<RequestResponse> GetLocationSpecial(LocationFilterMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetLocationPg(int pageNum, int pageItem);
        Task<RequestResponse> GetLocationPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetInbStatingLocationPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetOutStatingLocationPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetLocationPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetLocationSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetLocationById(string locationId);
        Task<RequestResponse> GetDIstinctAisle();
        Task<RequestResponse> GetDIstinctBay();
        Task<RequestResponse> GetLocationPgFiltered(LocationFilterMdl filter, int pageNum, int pageItem);
        Task<RequestResponse> GetLocationFltrPaged(LocationFilterMdl filter, int pageNum, int pageItem);
        Task<RequestResponse> CreateLocation(LocationModel location);
        Task<RequestResponse> UpdateLocation(LocationModel location);
        Task<RequestResponse> DeleteLocation(string locationId, string userAccountId);
        Task<RequestResponse> DefineTargetLocation(string locVCode);
        Task<RequestResponse> DefineLPNPutawayLoc(string lpnId, string locVCode);
    }
}
