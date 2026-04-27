namespace Mygamelist.Contracts.DTOs.User;

public class CreateUserDto
{
    public required string Pseudo { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}