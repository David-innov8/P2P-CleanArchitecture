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
        // Fetch the user from the repository
        var user = await _userRepository.GetUserFromClaimsAsync();
        if (user == null)
            throw new InvalidOperationException("User not found.");

        // Hash the raw PIN using the PasswordHasher
        byte[] pinSalt, pinHash;
        _passwordHasher.HashPassword(command, out pinSalt, out pinHash);

        // Create a PinHash value object
        var hashedPin = new PinHash(pinSalt, pinHash);

        // Call the domain method to set the PIN
        user.SetPin(hashedPin);

        // Persist the updated user
        await _userRepository.UpdateUserAsync(user);
    }
}