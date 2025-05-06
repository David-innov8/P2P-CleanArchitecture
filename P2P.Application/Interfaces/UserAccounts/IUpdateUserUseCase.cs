using P2P.Application.DTOs.USerDTOs;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces.UserAccounts;

public interface IUpdateUserUseCase
{
    Task<ApiResponse<UpdateUserDTO>> ExecuteAsync(UpdateUserDTO dto, Guid userId);
}