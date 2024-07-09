using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Validators
{
    public interface IDepthChartEntryValidator
    {
        void Validate(DepthChartEntry entry);
    }
}
