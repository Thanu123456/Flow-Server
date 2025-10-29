namespace Flow_Api.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base(message, 401) { }
    }
}
