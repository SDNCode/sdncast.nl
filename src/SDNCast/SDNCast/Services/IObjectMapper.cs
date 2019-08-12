using System.Threading.Tasks;

namespace SDNCast.Services
{
    public interface IObjectMapper
    {
        TDest Map<TSource, TDest>(TSource source, TDest dest);
    }
}
