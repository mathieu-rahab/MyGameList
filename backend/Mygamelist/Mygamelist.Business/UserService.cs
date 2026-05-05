using System.Net;
using Mygamelist.Contracts.DTOs.User;
using Mygamelist.Core.Business;
using Mygamelist.Core.Exceptions;
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
        
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        
        private static UserResponseDto MapToDto(User user) => new UserResponseDto
        {
            Id = user.Id,
            Pseudo = user.Pseudo,
            Email = user.Email,
            SteamId = user.SteamId,
            ProfilePicturePath = user.ProfilePicturePath
        };

        public UserResponseDto Add(CreateUserDto dto)
        {
            if (_userRepository.EmailExists(dto.Email))
                throw new BusinessException(HttpStatusCode.Conflict, "EMAIL_ALREADY_EXISTS");
            
            if (_userRepository.PseudoExists(dto.Pseudo))
                throw new BusinessException(HttpStatusCode.Conflict, "USERNAME_ALREADY_EXISTS");

            var user = new User
            {
                Pseudo = dto.Pseudo,
                Email = dto.Email,
                // Hashage du mot de passe
                PasswordHash = HashPassword(dto.Password)
            };
            return MapToDto(_userRepository.Insert(user));
        }

        public bool Remove(int id)
        {
            var deleted = _userRepository.Delete(id);
            if (!deleted)
            {
                throw new BusinessException(HttpStatusCode.NotFound, "USER_NOT_FOUND");
            }
            return true;
        }

        public IEnumerable<UserResponseDto> RetrieveAll()
        {
            return _userRepository.SelectAll().Select(MapToDto).ToList();
        }

        public UserResponseDto RetrieveById(int id)
        {
            var user = _userRepository.SelectById(id);
            return user is null
                ? throw new BusinessException(HttpStatusCode.NotFound, "USER_NOT_FOUND")
                : MapToDto(user);
        }


        public User Update(int id, User user)
        {
            return _userRepository.Update(id, user);
        }
        

    }
}