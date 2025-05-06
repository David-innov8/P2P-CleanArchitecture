using System.ComponentModel.DataAnnotations;
using P2P.Application.DTOs.USerDTOs;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.UserAccounts;
using P2P.Application.Validators;
using P2P.Domains.Entities;
using P2P.Domains.Exceptions;

namespace P2P.Application.UseCases;

public class UpdateUserDetailsCase:IUpdateUserUseCase
{

    public readonly IUserRepository _userRepository;

    public UpdateUserDetailsCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


   public async  Task<ApiResponse<UpdateUserDTO>> ExecuteAsync(UpdateUserDTO dto, Guid userId)
    {
        var validator = new UpdateUserValidator().Validate(dto);

        if (!validator.IsValid)
        {
            throw new ValidationException();
        }
        
        
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (!string.IsNullOrWhiteSpace(dto.LastName))
        {
            user.UpdateProfileLastnaem(dto.LastName);
        }

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            user.UpdatePhoneNumber(dto.PhoneNumber);
        }

        await _userRepository.UpdateUserAsync(user);

        return ApiResponse<UpdateUserDTO>.SuccessResponse(dto, "Updated");
        
    }

}