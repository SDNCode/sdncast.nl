using SDNCast.Models;
using System.Threading.Tasks;

namespace SDNCast.Services
{
    public interface ILiveShowDetailsService
    {
        Task<LiveShowDetailsModel> LoadAsync();

        Task SaveAsync(LiveShowDetailsModel liveShowDetails);
    }
}
