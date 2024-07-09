using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Validators
{
    public interface IPlayerValidator
    {
        void Validate(Player player);
    }
}
