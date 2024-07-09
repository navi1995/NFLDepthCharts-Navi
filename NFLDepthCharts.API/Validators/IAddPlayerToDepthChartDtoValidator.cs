using NFLDepthCharts.API.DTOs;

namespace NFLDepthCharts.API.Validators
{
    public interface IAddPlayerToDepthChartDtoValidator
    {
        void Validate(AddPlayerToDepthChartDto dto);
    }
}
