using P2P.Domains;

namespace P2P.Application.DTOs;

public class SignUpDto
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Gender Gender { get; set; }
    public DateTime? DOB { get; set; }
  
    public string PhoneNumber { get; set; }
    public string Password { get; set; }

}