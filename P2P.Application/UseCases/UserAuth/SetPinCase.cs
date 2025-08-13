using P2P.Application.UseCases.Interfaces;
using P2P.Domains.ValueObjects;

namespace P2P.Application.UseCases;

public class SetPinCase: ISetPinCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public SetPinCase(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher; 
    }

    public async Task Handle(string command)
    {
        var user = await _userRepository.GetUserFromClaimsAsync();
        if (user == null)
            throw new InvalidOperationException("User not found.");
        
        byte[] pinSalt, pinHash;
        _passwordHasher.HashPassword(command, out pinSalt, out pinHash);

        
        var hashedPin = new PinHash(pinSalt, pinHash);

        
        user.SetPin(hashedPin);

       
        await _userRepository.UpdateUserAsync(user);
    }
}