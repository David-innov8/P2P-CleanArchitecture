using P2P.Application.DTOs;
using P2P.Domains.Entities;

namespace P2P.Application.UseCases.Interfaces;

public interface IRegisterUserUseCase
{
    Task<ApiResponse<string>> UserSignUp (SignUpDto signUpDto);
}