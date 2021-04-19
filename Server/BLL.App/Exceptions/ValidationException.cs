using System;

namespace BLL.App.Exceptions
{
    public class ValidationException: Exception
    {
        public ValidationException(string? message) : base(message)
        {
        }
    }
}