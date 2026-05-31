using Mygamelist.Core.Repository;
using Mygamelist.DatabaseRepository.Context;
using Mygamelist.Entity;

namespace Mygamelist.DatabaseRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        
        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public bool Delete(int id)
        {
            var userToDelete = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            if (userToDelete == null) return false;
            _dbContext.Users.Remove(userToDelete);
            _dbContext.SaveChanges();

            return true;
        }

        public User Insert(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return user;
        }

        public IEnumerable<User> SelectAll()
        {
            var users = _dbContext.Users;
            return users;

        }

        public User? SelectById(int id) => _dbContext.Users.FirstOrDefault(u => u.Id == id);
        public User? SelectByEmail(string email) => _dbContext.Users.FirstOrDefault(u => u.Email == email);

        public User Reset(int id, User user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();

            return user;
        }
        
        public User Update(int id, User user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();

            return user;
        }
        
        public bool EmailExists(string email)
        {
            return _dbContext.Users.Any(u => u.Email == email);
        }

        public bool PseudoExists(string pseudo)
        {
            return _dbContext.Users.Any(u => u.Pseudo == pseudo);
        }

    }
}

