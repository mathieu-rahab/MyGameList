using Mygamelist.Entity;

namespace Mygamelist.Core.Business
{
    public interface IUserService
    {
        IEnumerable<User> RetrieveAll();
        User? RetrieveById(int id);

        User Add(User bet);
        User Update(int id, User user);
        bool Remove(int id);
        bool EmailExists(string email);
        bool PseudoExists(string pseudo);
    }
}