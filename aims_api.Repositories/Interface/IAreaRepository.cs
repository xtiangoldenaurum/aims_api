using aims_api.Models;

namespace aims_api.Repositories.Interface
{
    public interface IAreaRepository
    {
        Task<IEnumerable<AreaModel>> GetAllArea();
        Task<IEnumerable<AreaModel>> GetAreaPg(int pageNum, int pageItem);
        Task<AreaPagedMdl?> GetAreaPaged(int pageNum, int pageItem);
        Task<IEnumerable<AreaModel>> GetAreaPgSrch(string searchKey, int pageNum, int pageItem);
        Task<AreaPagedMdl?> GetAreaSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<AreaModel> GetAreaById(string areaId);
        Task<bool> AreaExists(string areaId);
        Task<bool> CreateArea(AreaModel area);
        Task<bool> UpdateArea(AreaModel area);
        Task<bool> AreaInUse(string areaId);
        Task<bool> DeleteArea(string areaId, string userAccountId);
    }
}
