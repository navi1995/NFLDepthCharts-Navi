using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Validators
{
    public class DepthChartEntryValidator : IDepthChartEntryValidator
    {
        public void Validate(DepthChartEntry entry)
        {
            if (entry == null)
                throw new ValidationException(ErrorMessages.DepthChartEntryNull);

            if (entry.DepthLevel < 0)
                throw new ValidationException(ErrorMessages.DepthLevelNegative);

            if (entry.PlayerId <= 0)
                throw new ValidationException(ErrorMessages.PlayerIdInvalid);

            if (entry.PositionId <= 0)
                throw new ValidationException(ErrorMessages.PositionIdInvalid);
        }
    }
}
