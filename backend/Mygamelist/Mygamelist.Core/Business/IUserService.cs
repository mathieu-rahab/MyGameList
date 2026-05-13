using Mygamelist.Contracts.DTOs.User;
using Mygamelist.Entity;

namespace Mygamelist.Core.Business
{
    public interface IUserService
    {
        IEnumerable<UserResponseDto> RetrieveAll();
        UserResponseDto RetrieveById(int id);

        UserResponseDto Add(CreateUserDto dto);
        User Update(int id, User user);
        bool Remove(int id);
    }
}