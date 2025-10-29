﻿namespace Flow_Api.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message, 404) { }
    }
}
