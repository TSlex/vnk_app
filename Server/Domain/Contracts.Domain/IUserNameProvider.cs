namespace Contracts.Domain
{
    public interface IUserNameProvider
    {
        string CurrentUserName { get;  }
    }
}