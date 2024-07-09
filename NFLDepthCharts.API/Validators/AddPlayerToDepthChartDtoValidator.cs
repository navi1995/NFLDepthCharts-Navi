using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.DTOs;
using NFLDepthCharts.API.Exceptions;

namespace NFLDepthCharts.API.Validators
{
    public class AddPlayerToDepthChartDtoValidator : IAddPlayerToDepthChartDtoValidator
    {
        public void Validate(AddPlayerToDepthChartDto dto)
        {
            if (dto == null)
                throw new ValidationException(ErrorMessages.AddPlayerDtoNull);

            if (string.IsNullOrWhiteSpace(dto.Position))
                throw new ValidationException(ErrorMessages.PositionRequired);

            if (dto.PlayerNumber < 1 || dto.PlayerNumber > 99) // What is NFL max jersey number? What about other sports for future?
                throw new ValidationException(ErrorMessages.PlayerNumberInvalid);

            if (string.IsNullOrWhiteSpace(dto.PlayerName))
                throw new ValidationException(ErrorMessages.PlayerNameRequired);

            if (dto.PlayerName.Length > 100)
                throw new ValidationException(ErrorMessages.PlayerNameTooLong);

            if (dto.PositionDepth.HasValue && dto.PositionDepth.Value < 0)
                throw new ValidationException(ErrorMessages.PositionDepthNegative);
        }
    }
}
