namespace Flow_Api.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message) : base(message, 403) { }
    }
}
