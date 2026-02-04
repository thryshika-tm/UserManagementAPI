using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using UserManagementAPI.DTOs;

namespace UserManagementAPI.Tests.Controllers;

public class UsersControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public UsersControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ReturnsOk_WithUserResponse()
    {
        var request = new CreateUserRequest("Test", "User", $"test-{Guid.NewGuid():N}@example.com");

        var response = await _client.PostAsJsonAsync("/api/users", request);

        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);
        Assert.NotNull(user);
        Assert.Equal(request.FirstName, user.FirstName);
        Assert.Equal(request.LastName, user.LastName);
        Assert.Equal(request.Email, user.Email);
        Assert.True(user.Id > 0);
    }

    [Fact]
    public async Task Get_AfterCreate_ReturnsUser()
    {
        var createRequest = new CreateUserRequest("Get", "User", $"get-{Guid.NewGuid():N}@example.com");
        var createResponse = await _client.PostAsJsonAsync("/api/users", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/users/{created.Id}");

        getResponse.EnsureSuccessStatusCode();
        var user = await getResponse.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);
        Assert.NotNull(user);
        Assert.Equal(created.Id, user.Id);
        Assert.Equal("Get", user.FirstName);
    }

    [Fact]
    public async Task Get_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/users/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenUserExists_ReturnsOk_WithUpdatedUser()
    {
        var createRequest = new CreateUserRequest("Original", "Name", $"orig-{Guid.NewGuid():N}@example.com");
        var createResponse = await _client.PostAsJsonAsync("/api/users", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);
        Assert.NotNull(created);

        var updateRequest = new UpdateUserRequest("Updated", "Name", "updated@example.com");
        var updateResponse = await _client.PutAsJsonAsync($"/api/users/{created.Id}", updateRequest);

        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.FirstName);
        Assert.Equal("Name", updated.LastName);
        Assert.Equal("updated@example.com", updated.Email);
    }

    [Fact]
    public async Task Update_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var request = new UpdateUserRequest("New", "Name", "new@example.com");
        var response = await _client.PutAsJsonAsync("/api/users/999999", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithAllUsers()
    {
        var createRequest1 = new CreateUserRequest("First", "User", $"first-{Guid.NewGuid():N}@example.com");
        var createRequest2 = new CreateUserRequest("Second", "User", $"second-{Guid.NewGuid():N}@example.com");
        await _client.PostAsJsonAsync("/api/users", createRequest1);
        await _client.PostAsJsonAsync("/api/users", createRequest2);

        var response = await _client.GetAsync("/api/users");

        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>(JsonOptions);
        Assert.NotNull(users);
        Assert.True(users.Count >= 2);
        Assert.Contains(users, u => u.FirstName == "First");
        Assert.Contains(users, u => u.FirstName == "Second");
    }
}
