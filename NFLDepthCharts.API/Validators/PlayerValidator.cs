using NFLDepthCharts.API.Constants;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Models;

namespace NFLDepthCharts.API.Validators
{
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
}
