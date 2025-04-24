using FluentValidation;
using P2P.Application.DTOs;

namespace P2P.Application.Validators;

public class SignUpValidator: AbstractValidator<SignUpDto>
{
    public SignUpValidator()
    {
        // Validate Username
        RuleFor(dto => dto.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.");

        // Validate First Name
        RuleFor(dto => dto.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Matches(@"^[a-zA-Z]+$").WithMessage("First name must contain only letters.");

        // Validate Last Name
        RuleFor(dto => dto.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Matches(@"^[a-zA-Z]+$").WithMessage("Last name must contain only letters.");

        // Validate Email
        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        // Validate Gender
        RuleFor(dto => dto.Gender)
            .IsInEnum().WithMessage("Invalid gender value.");

        // Validate Date of Birth (optional)
        RuleFor(dto => dto.DOB)
            .Must(dob => dob == null || dob <= DateTime.UtcNow.AddYears(-18))
            .WithMessage("You must be at least 18 years old.");

        // // Validate Profile Image (optional)
        // RuleFor(dto => dto.ProfileImage)
        //     .Must(image => image == null || image.Length > 0)
        //     .WithMessage("Profile image must be a valid byte array.");

        // Validate Phone Number
        RuleFor(dto => dto.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

        // Validate Password
        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}