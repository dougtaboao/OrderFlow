namespace OrderFlow.Application.Security
{
    public interface ICurrentUser
    {
        Guid UserId { get; }

        string Name { get; }

        string Role { get; }

        bool IsAuthenticated { get; }
    }
}