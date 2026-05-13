using Mygamelist.Entity;

namespace Mygamelist.Core.Business
{
    public interface IAuthService
    {
        User Authenticate(string email, string password);
    }
}
