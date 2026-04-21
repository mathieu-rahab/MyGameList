using Mygamelist.Core.Business;
using Mygamelist.Core.Repository;
using Mygamelist.Entity;


namespace Mygamelist.Business
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Add(User user)
        {
            return _userRepository.Insert(user);
        }

        public bool Remove(int id)
        {
            return _userRepository.Delete(id);
        }

        public IEnumerable<User> RetrieveAll()
        {
            
            return (_userRepository.SelectAll()
                .ToList());
        }

        public User? RetrieveById(int id)
        {
            return _userRepository.SelectById(id);
        }

        public User Update(int id, User user)
        {
            return _userRepository.Update(id, user);
        }
        
        public bool EmailExists(string email)
        {
            return _userRepository.EmailExists(email);
        }

        public bool PseudoExists(string pseudo)
        {
            return _userRepository.PseudoExists(pseudo);
        }

    }
}