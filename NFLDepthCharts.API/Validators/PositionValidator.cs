using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Validators
{
    public class PositionValidator : IPositionValidator
    {
        public void Validate(Position position)
        {
            if (position == null)
                throw new ValidationException(ErrorMessages.PositionNotFound);

            if (string.IsNullOrWhiteSpace(position.Name))
                throw new ValidationException(ErrorMessages.PositionNameRequired);

            if (position.Name.Length > 50)
                throw new ValidationException(ErrorMessages.PositionNameTooLong);
        }
    }
}
