using Moq;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;
using UserManagementAPI.Services;
using Xunit;

namespace UserManagementAPI.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUserResponse()
    {
        var userId = 1;
        var user = new User
        {
            Id = userId,
            FirstName = "test",
            LastName = "user",
            Email = "test-user@example.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = null
        };
        _mockRepository
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _userService.GetByIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("test", result.FirstName);
        Assert.Equal("user", result.LastName);
        Assert.Equal("test-user@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var userId = 999;
        _mockRepository
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _userService.GetByIdAsync(userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesUserAndReturnsResponse()
    {
        var request = new CreateUserRequest("test2", "user", "test2-user@example.com");
        User? capturedUser = null;
        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) =>
            {
                capturedUser = u;
                u.Id = 1; 
            })
            .ReturnsAsync((User u, CancellationToken _) => u);

        var result = await _userService.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("test2", result.FirstName);
        Assert.Equal("user", result.LastName);
        Assert.Equal("test2-user@example.com", result.Email);
        Assert.True(result.Id > 0);
        Assert.NotNull(capturedUser);
        Assert.Equal(request.FirstName, capturedUser.FirstName);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserExists_UpdatesAndReturnsResponse()
    {
        var userId = 1;
        var existingUser = new User
        {
            Id = userId,
            FirstName = "Old",
            LastName = "Name",
            Email = "old@example.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = null
        };
        var request = new UpdateUserRequest("New", "Name", "new@example.com");
        _mockRepository
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken _) => u);

        var result = await _userService.UpdateAsync(userId, request);

        Assert.NotNull(result);
        Assert.Equal("New", result.FirstName);
        Assert.Equal("Name", result.LastName);
        Assert.Equal("new@example.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var userId = 999;
        var request = new UpdateUserRequest("New", "Name", "new@example.com");
        _mockRepository
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _userService.UpdateAsync(userId, request);

        Assert.Null(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WhenUsersExist_ReturnsAllUserResponses()
    {
        var users = new List<User>
    {
        new() { Id = 1, FirstName = "test1", LastName = "user", Email = "test1-user@example.com", CreatedAt = DateTime.UtcNow, UpdatedAt = null },
        new() { Id = 2, FirstName = "test2", LastName = "user", Email = "test2-user@example.com", CreatedAt = DateTime.UtcNow, UpdatedAt = null }
    };
        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await _userService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("test1", result[0].FirstName);
        Assert.Equal("test2-user@example.com", result[1].Email);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoUsers_ReturnsEmptyList()
    {
        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        var result = await _userService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
