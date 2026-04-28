using Mygamelist.Core.Business;
using Mygamelist.Core.Repository;
using Mygamelist.Entity;

namespace Mygamelist.Business;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User Authenticate(string email, string password)
    {
        var user = _userRepository.SelectByEmail(email)
                   ?? throw new UnauthorizedAccessException("EMAIL_OR_PASSWORD_INCORRECT");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("EMAIL_OR_PASSWORD_INCORRECT");

        return user;
    }
}