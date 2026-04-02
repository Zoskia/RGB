using FluentAssertions;
using Moq;
using RedGreenBlue.Models;
using RedGreenBlue.Repositories;
using RedGreenBlue.Services;
using RedGreenBlue.Services.Interfaces;

namespace RedGreenBlue.UnitTests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var userRepository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        userRepository
            .Setup(r => r.GetByUsernameAsync("missing"))
            .ReturnsAsync((User?)null);

        var sut = new AuthService(userRepository.Object, passwordHasher.Object);

        var result = await sut.LoginAsync("missing", "pw12345");

        result.Should().BeNull();
        passwordHasher.Verify(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordDoesNotMatch()
    {
        var dbUser = new User
        {
            Id = 5,
            Username = "Alice",
            Password = "storedHash",
            Salt = "salt",
            Team = TeamColor.Red
        };

        var userRepository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        userRepository
            .Setup(r => r.GetByUsernameAsync("Alice"))
            .ReturnsAsync(dbUser);
        passwordHasher
            .Setup(h => h.VerifyPassword("storedHash", "wrong"))
            .Returns(false);

        var sut = new AuthService(userRepository.Object, passwordHasher.Object);

        var result = await sut.LoginAsync("Alice", "wrong");

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUser_WhenCredentialsAreValid()
    {
        var dbUser = new User
        {
            Id = 7,
            Username = "Alice",
            Password = "storedHash",
            Salt = "salt",
            Team = TeamColor.Green
        };

        var userRepository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        userRepository
            .Setup(r => r.GetByUsernameAsync("Alice"))
            .ReturnsAsync(dbUser);
        passwordHasher
            .Setup(h => h.VerifyPassword("storedHash", "pw12345"))
            .Returns(true);

        var sut = new AuthService(userRepository.Object, passwordHasher.Object);

        var result = await sut.LoginAsync("Alice", "pw12345");

        result.Should().BeSameAs(dbUser);
    }

    [Fact]
    public async Task RegisterAsync_ShouldHashPasswordAndPersistUser()
    {
        var userRepository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();

        var inputUser = new User
        {
            Username = "NewUser",
            Password = "plainPassword",
            Team = TeamColor.Blue
        };

        passwordHasher
            .Setup(h => h.GenerateSalt())
            .Returns("generatedSalt");
        passwordHasher
            .Setup(h => h.HashPassword("plainPassword", "generatedSalt"))
            .Returns("hashedPassword");
        userRepository
            .Setup(r => r.AddNewUserAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var sut = new AuthService(userRepository.Object, passwordHasher.Object);

        var result = await sut.RegisterAsync(inputUser);

        result.Should().NotBeNull();
        result!.Password.Should().Be("hashedPassword");
        result.Salt.Should().Be("generatedSalt");
        passwordHasher.Verify(h => h.HashPassword("plainPassword", "generatedSalt"), Times.Once);
        userRepository.Verify(r => r.AddNewUserAsync(It.Is<User>(u => u.Password == "hashedPassword" && u.Salt == "generatedSalt")), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnNull_WhenRepositoryRejectsUsername()
    {
        var userRepository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();

        passwordHasher.Setup(h => h.GenerateSalt()).Returns("salt");
        passwordHasher.Setup(h => h.HashPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("hash");
        userRepository.Setup(r => r.AddNewUserAsync(It.IsAny<User>())).ReturnsAsync((User?)null);

        var sut = new AuthService(userRepository.Object, passwordHasher.Object);

        var result = await sut.RegisterAsync(new User
        {
            Username = "TakenUser",
            Password = "pw12345",
            Team = TeamColor.Red
        });

        result.Should().BeNull();
    }
}
