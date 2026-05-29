using Mygamelist.Entity;

namespace Mygamelist.Core.Repository
{
    public interface IUserRepository
    {
        bool Delete(int id);
        User Insert(User user);
        public IEnumerable<User> SelectAll();
        User? SelectById(int id);
        User? SelectByEmail(string email);
        User Reset(int id, User user);
        User Update(int id, User user);
        bool EmailExists(string email);
        bool PseudoExists(string pseudo);
    }
}