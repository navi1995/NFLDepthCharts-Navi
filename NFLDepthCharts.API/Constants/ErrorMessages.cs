namespace NFLDepthCharts.API.Constants
{
    public static class ErrorMessages
    {
        public const string PlayerNull = "Player cannot be null.";
        public const string PlayerNameRequired = "Player name is required.";
        public const string PlayerNameTooLong = "Player name cannot exceed 100 characters.";
        public const string PlayerNumberInvalid = "Player number must be between 1 and 99.";
        public const string PlayerNotFound = "Player number could not be found.";
        public const string PlayerNotInDepthChart = "Player is not in the depth chart for this position.";

        public const string PositionNull = "Position cannot be null.";
        public const string PositionNameRequired = "Position name is required.";
        public const string PositionNameTooLong = "Position name cannot exceed 50 characters.";

        public const string DepthChartEntryNull = "Depth chart entry cannot be null.";
        public const string DepthLevelNegative = "Depth level must be non-negative.";
        public const string PlayerIdInvalid = "Player ID must be a positive number.";
        public const string PositionIdInvalid = "Position ID must be a positive number.";

        public const string AddPlayerDtoNull = "AddPlayerToDepthChartDto cannot be null.";
        public const string PositionRequired = "Position is required.";
        public const string PositionDepthNegative = "Position depth must be non-negative.";
        public const string PositionNotFound = "Position could not found.";
    }
}
