using Common.Model.Map;
using System.Threading.Tasks;

namespace Common.Utilities.Map
{
    public interface IMapUtil
    {
        Task<Coordinate> GetCoordinateFromAddressAsync(string address);
    }
}
