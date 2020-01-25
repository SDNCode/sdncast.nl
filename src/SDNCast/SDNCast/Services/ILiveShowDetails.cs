using System.Threading.Tasks;

using SDNCast.Models;

namespace SDNCast.Services
{
    public interface ILiveShowDetailsService
    {
        Task<LiveShowDetailsModel> LoadAsync();

        Task SaveAsync(LiveShowDetailsModel liveShowDetails);

        void ClearCache();
    }
}
