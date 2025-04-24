namespace P2P.Application.DTOs;

public class LoginResponse
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public byte[]? ProfilePicture { get; set; }
}