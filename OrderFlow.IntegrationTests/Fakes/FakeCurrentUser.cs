using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Security;

namespace OrderFlow.IntegrationTests.Fakes
{
    public class FakeCurrentUser : ICurrentUser
    {
        public bool IsAuthenticated => true;

        public string Name => "NameTest";

        public string Role => "RoleTest";

        public Guid UserId =>
            Guid.Parse("11111111-1111-1111-1111-111111111111");
    }
}