using System;

namespace BLL.Base.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string? message) : base(message)
        {
        }
    }
}