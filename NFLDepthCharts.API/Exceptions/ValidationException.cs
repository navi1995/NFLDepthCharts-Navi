namespace NFLDepthCharts.API.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException (string message) : base (message) { }
        // Could introduce StatusCode here to be used by Middleware
    }
}
