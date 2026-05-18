using System.Net;
using Mygamelist.Contracts.DTOs.Steam;
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
        private readonly ISteamService _steamService;
        
        public UserService(IUserRepository userRepository, ISteamService steamService)
        {
            _userRepository = userRepository;
            _steamService = steamService;
        }
        
        private static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        
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
                throw new BusinessException(HttpStatusCode.Conflict, "PSEUDO_ALREADY_EXISTS");

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
            return (!deleted)
                ? throw new BusinessException(HttpStatusCode.NotFound, "USER_NOT_FOUND")
                : true;
        }

        public IEnumerable<UserResponseDto> RetrieveAll() => _userRepository.SelectAll().Select(MapToDto).ToList();

        public UserResponseDto RetrieveById(int id)
        {
            var user = _userRepository.SelectById(id);
            return user is null
                ? throw new BusinessException(HttpStatusCode.NotFound, "USER_NOT_FOUND")
                : MapToDto(user);
        }


        public UserResponseDto Update(int id, UpdateUserDto dto)
        {
            var user = _userRepository.SelectById(id);

            if (user is null)
                throw new BusinessException(HttpStatusCode.NotFound, "USER_NOT_FOUND");

            if (dto.Email is not null && dto.Email != user.Email && _userRepository.EmailExists(dto.Email))
                throw new BusinessException(HttpStatusCode.Conflict, "EMAIL_ALREADY_EXISTS");

            if (dto.Pseudo is not null && dto.Pseudo != user.Pseudo && _userRepository.PseudoExists(dto.Pseudo))
                throw new BusinessException(HttpStatusCode.Conflict, "USERNAME_ALREADY_EXISTS");

            if (dto.Pseudo is not null)
                user.Pseudo = dto.Pseudo;

            if (dto.Email is not null)
                user.Email = dto.Email;

            if (dto.SteamId is not null)
                user.SteamId = dto.SteamId;

            if (dto.ProfilePicturePath is not null)
                user.ProfilePicturePath = dto.ProfilePicturePath;

            return MapToDto(_userRepository.Update(id, user));
        }
        
        
        public async Task<List<GameDto>> GetUserGames(int id)
        {
            var user = _userRepository.SelectById(id);

            if (user is null)
                throw new BusinessException(HttpStatusCode.NotFound, "USER_NOT_FOUND");

            if (string.IsNullOrWhiteSpace(user.SteamId))
                throw new BusinessException(HttpStatusCode.BadRequest, "USER_STEAM_ID_NOT_SET");

            return await _steamService.UserGames(user.SteamId);
        }

    }
}