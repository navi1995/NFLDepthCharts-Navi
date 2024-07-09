using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Validators
{
    public interface IPositionValidator
    {
        void Validate(Position position);
    }
}
