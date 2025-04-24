// using FluentAssertions;
// using Moq;
// using P2P.Application.DTOs;
// using P2P.Application.UseCases;
// using P2P.Application.UseCases.Interfaces;
// using P2P.Application.Validators;
// using P2P.Domains.Entities;
// using Xunit;
//
// namespace P2p.Application.Tests.Tests;
//
// public class SignUpCaseTests
// {
//     private readonly Mock<IUserRepository> _userRepoMock;
//     private readonly Mock<IPasswordHasher> _passwordHasherMock;
//     private readonly SignUpValidator _validator;
//
//     private readonly SignUpCase _sut;
//
//     public SignUpCaseTests()
//     {
//         
//         // we are basically mocking/ creating fake versions of the repository and hasher
//         _userRepoMock = new Mock<IUserRepository>();
//         _passwordHasherMock = new Mock<IPasswordHasher>();
//         _validator = new SignUpValidator();
//         _sut = new SignUpCase(_userRepoMock.Object, _passwordHasherMock.Object, _validator);
//         
//     }
//
//     [Fact]
//
//     public async Task UserSignUp_ShouldReturnSuccess_WhenValidInput()
//     {
//         var dto = new SignUpDto()
//         {
//             Username = "test",
//             Email = "test@test.com",
//             FirstName = "test",
//             LastName = "    Test",
//             PhoneNumber = "0077226633",
//             Password = "password",
//         };
//         
//         // Tell the userRepo mock: when GetUserByEmailAsync is called, return null (user not found)
//
//         _userRepoMock.Setup(repo=> repo.GetUserByEmailAsync(dto.Email)).ReturnsAsync((User)null);
//         
//         
//         // Mock the password hasher to return a fake hashed password
//         byte[] salt = new byte[] { 1, 2 };
//         byte[] hash = new byte[] { 3, 4 };
//         _passwordHasherMock.Setup(h => h.HashPassword(dto.Password, out salt, out hash))
//             .Returns("hashedPassword");
//         // Act = run the method under test
//         var result = await _sut.UserSignUp(dto);
//         
//         result.Success.Should().BeTrue();
//         result.Message.Should().Be("User successfully signed up.");
//         
//         _userRepoMock.Verify((repo) => repo.GetUserByEmailAsync(dto.Email), Times.Once);
//
//     }
// }