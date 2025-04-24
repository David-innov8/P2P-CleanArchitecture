namespace P2P.Application.DTOs;

public interface SignUpResponse
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string? ProfileImage {
        get;
        set;
    }
}