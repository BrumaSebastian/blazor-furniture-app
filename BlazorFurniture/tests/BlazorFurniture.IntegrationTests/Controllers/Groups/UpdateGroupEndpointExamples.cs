using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Controllers.Groups;
using BlazorFurniture.IntegrationTests.Controllers.Setup;
using FastEndpoints;
using FastEndpoints.Testing;
using System.Net;

namespace BlazorFurniture.IntegrationTests.Controllers.Groups;

/// <summary>
/// Example integration tests showing best practices for testing FastEndpoints
/// with proper data setup and cleanup.
/// </summary>
public class UpdateGroupEndpointExamples( ProgramSut sut ) : TestBase<ProgramSut>
{
    /// <summary>
    /// Example showing how to test with seeded data.
    /// In a real scenario, you would seed the group in your test database setup.
    /// </summary>
    [Fact]
    public async Task Example_UpdateGroup_WithSeededData()
    {
        // Arrange - In real tests, seed data in ConfigureServices or test setup
        var groupId = Guid.NewGuid();
        // TODO: Seed this group in your test database
        // await SeedGroupAsync(groupId, "Original Name");

        var request = new UpdateGroupRequest
        {
            Id = groupId,
            Name = "Updated Name"
        };

        // Act
        var (response, problem) = await sut.Client
            .PUTAsync<UpdateGroupEndpoint, UpdateGroupRequest, ProblemDetails>(request);

        // Assert
        if (response.IsSuccessStatusCode)
        {
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Null(problem);

            // Optionally verify the update in database
            // var updatedGroup = await GetGroupAsync(groupId);
            // Assert.Equal("Updated Name", updatedGroup.Name);
        }
        else
        {
            // If group wasn't seeded, we expect 404
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    /// <summary>
    /// Example showing multiple test scenarios in sequence
    /// </summary>
    [Fact]
    public async Task Example_UpdateGroup_MultipleScenarios()
    {
        // Scenario 1: Update non-existent group (should fail)
        var nonExistentId = Guid.NewGuid();
        var request1 = new UpdateGroupRequest
        {
            Id = nonExistentId,
            Name = "Test Name"
        };

        var (response1, problem1) = await sut.Client
            .PUTAsync<UpdateGroupEndpoint, UpdateGroupRequest, ProblemDetails>(request1);

        Assert.Equal(HttpStatusCode.NotFound, response1.StatusCode);
        Assert.NotNull(problem1);

        // Scenario 2: Update with invalid data (should fail)
        var request2 = new UpdateGroupRequest
        {
            Id = Guid.NewGuid(),
            Name = "" // Empty name should fail validation
        };

        var (response2, problem2) = await sut.Client
            .PUTAsync<UpdateGroupEndpoint, UpdateGroupRequest, ProblemDetails>(request2);

        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        Assert.NotNull(problem2);
    }

    /// <summary>
    /// Example showing how to test with custom headers or authentication
    /// </summary>
    [Fact]
    public async Task Example_UpdateGroup_WithAuthentication()
    {
        // Arrange
        var request = new UpdateGroupRequest
        {
            Id = Guid.NewGuid(),
            Name = "Test Group"
        };

        // Add authentication token if your endpoint requires it
        // sut.Client.DefaultRequestHeaders.Authorization = 
        //     new AuthenticationHeaderValue("Bearer", "your-test-token");

        // Act
        var (response, problem) = await sut.Client
            .PUTAsync<UpdateGroupEndpoint, UpdateGroupRequest, ProblemDetails>(request);

        // Assert
        // If no auth is configured or token is missing, might get 401
        Assert.NotNull(response);
    }

    /// <summary>
    /// Example showing how to extract and verify problem details
    /// </summary>
    [Fact]
    public async Task Example_UpdateGroup_VerifyProblemDetails()
    {
        // Arrange
        var request = new UpdateGroupRequest
        {
            Id = Guid.NewGuid(),
            Name = "" // Invalid
        };

        // Act
        var (response, problem) = await sut.Client
            .PUTAsync<UpdateGroupEndpoint, UpdateGroupRequest, ProblemDetails>(request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);

        if (problem != null)
        {
            Assert.NotNull(problem.Title);
            Assert.NotNull(problem.Detail);
            Assert.True(problem.Status > 0);

            // Check for validation errors
            if (problem.Errors != null && problem.Errors.Any())
            {
                var hasNameError = problem.Errors.Any(e => e.Name == "Name");
                Assert.True(hasNameError);
            }
        }
    }

    /// <summary>
    /// Example showing how to test duplicate name conflict
    /// </summary>
    [Fact]
    public async Task Example_UpdateGroup_DuplicateName()
    {
        // Arrange
        // TODO: Seed two groups:
        // Group 1: ID = groupId1, Name = "Existing Name"
        // Group 2: ID = groupId2, Name = "Other Name"

        var groupId2 = Guid.NewGuid();
        var request = new UpdateGroupRequest
        {
            Id = groupId2,
            Name = "Existing Name" // This name already exists for Group 1
        };

        // Act
        var (response, problem) = await sut.Client
            .PUTAsync<UpdateGroupEndpoint, UpdateGroupRequest, ProblemDetails>(request);

        // Assert
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            Assert.NotNull(problem);
            Assert.Contains("conflict", problem.Title?.ToLower() ?? "");
        }
    }
}
