using aims_api.Enums;
using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ILocationRepository
    {
        Task<IEnumerable<LocationModel>> GetLocationPg(int pageNum, int pageItem);
        Task<LocationPagedMdl?> GetLocationPaged(int pageNum, int pageItem);
        Task<LocationPagedMdl?> GetInbStatingLocationPaged(int pageNum, int pageItem);
        Task<LocationPagedMdl?> GetOutStatingLocationPaged(int pageNum, int pageItem);
        Task<IEnumerable<LocationModel>> GetLocationPgSrch(string searchKey, int pageNum, int pageItem);
        Task<LocationPagedMdl?> GetLocationSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<LocationModel> GetLocationById(string locationId);
        Task<LocationModel> GetLocationByVCode(string locVCode);
        Task<bool> LocationExists(string locationId);
        Task<IEnumerable<string>> GetDIstinctAisle();
        Task<IEnumerable<string>> GetDIstinctBay();
        Task<IEnumerable<LocationModel>> GetLocationPgFiltered(LocationFilterMdl filter, int pageNum, int pageItem);
        Task<LocationPagedMdl?> GetLocationFltrPaged(LocationFilterMdl filter, int pageNum, int pageItem);
        Task<bool> CreateLocation(LocationModel location);
        Task<bool> UpdateLocation(LocationModel location);
        Task<bool> LocationInUse(string locationId);
        Task<bool> DeleteLocation(string locationId, string userAccountId);
        Task<TargetLocModelMod> DefineTargetLocation(string locVCode);
        Task<TargetLocModelMod> DefineTargetLocByLocId(string locationId);
        Task<IEnumerable<TargetLocModel>?> GetLocUsedLPNs(string locationId);
        Task<TargetLocModelMod> BuildFixedTargetLoc(string locVCode, LocationModel location, string lpnTo);
        Task<TargetLocModelMod> BuildAnyLPNTargetLoc(string locVCode, LocationModel location);
        Task<TargetLocModelMod> BuildNoLPNTargetLoc(string locVCode, LocationModel location);
        Task<TargetLocModelMod> DefineLPNPutawayLoc(string lpnId, string locVCode);
    }
}
