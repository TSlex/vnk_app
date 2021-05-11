namespace BLL.Base.Exceptions
{
    public static class ExceptionExecutors
    {
        public static void ValidationFailed(string message)
        {
            throw new ValidationException(message);
        }
        
        public static void NotFound(string message)
        {
            throw new NotFoundException(message);
        }
    }
}