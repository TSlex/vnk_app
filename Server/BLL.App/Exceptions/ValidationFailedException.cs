using System;

namespace BLL.App.Exceptions
{
    public class ValidationFailedException: Exception
    {
        public ValidationFailedException(string? message) : base(message)
        {
        }
    }
}