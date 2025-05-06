using FluentValidation;
using P2P.Application.DTOs.USerDTOs;

namespace P2P.Application.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserDTO>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone Number is required").Length(10, 13)
            .WithMessage("Phone Number must between 11 to 13 digits").Matches(@"^\d+$")
            .WithMessage("Phone Number must contain only numbers");

        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required").
            Length(4, 30).WithMessage("Last name must be between 4 and 30 characters");

    }
}