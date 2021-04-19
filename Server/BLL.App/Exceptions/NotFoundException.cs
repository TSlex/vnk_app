using System;

namespace BLL.App.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string? message) : base(message)
        {
        }
    }
}