using System;

namespace BLL.Base.Exceptions
{
    public class ValidationException: Exception
    {
        public ValidationException(string? message) : base(message)
        {
        }
    }
}