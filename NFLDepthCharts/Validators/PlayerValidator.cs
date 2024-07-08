using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.DTOs;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Validators
{
    public interface IPlayerValidator
    {
        void Validate(Player player);
    }

    public interface IPositionValidator
    {
        void Validate(Position position);
    }

    public interface IDepthChartEntryValidator
    {
        void Validate(DepthChartEntry entry);
    }

    public interface IAddPlayerToDepthChartDtoValidator
    {
        void Validate(AddPlayerToDepthChartDto dto);
    }

    public class PlayerValidator : IPlayerValidator
    {
        public void Validate(Player player)
        {
            if (player == null)
                throw new ValidationException(ErrorMessages.PlayerNotFound);

            if (string.IsNullOrWhiteSpace(player.Name))
                throw new ValidationException(ErrorMessages.PlayerNameRequired);

            if (player.Name.Length > 100)
                throw new ValidationException(ErrorMessages.PlayerNameTooLong);

            if (player.Number < 1 || player.Number > 99)
                throw new ValidationException(ErrorMessages.PlayerNumberInvalid);
        }
    }

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
