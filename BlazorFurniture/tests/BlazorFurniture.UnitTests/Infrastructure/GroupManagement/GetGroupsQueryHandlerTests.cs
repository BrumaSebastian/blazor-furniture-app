using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Queries;
using BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Queries;
using Moq;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.GroupManagement;

public class GetGroupsQueryHandlerTests
{
    private readonly Mock<IGroupManagementClient> clientMock;
    private readonly KeycloakHttpErrorMapper errorMapper;
    private readonly GetGroupsQueryHandler handler;

    public GetGroupsQueryHandlerTests()
    {
        clientMock = new Mock<IGroupManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        handler = new GetGroupsQueryHandler(clientMock.Object, errorMapper);
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyPaginatedResponse_WhenNoGroupsExist()
    {
        // Arrange
        var filters = new GroupQueryFilters { Page = 1, PageSize = 10 };
        var query = new GetGroupsQuery(filters);

        clientMock
            .Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 0 }));

        clientMock
            .Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Succeeded([]));

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.Total);
        Assert.Empty(result.Value.Results);
        clientMock.Verify(c => c.GetGroupsCount(It.IsAny<CancellationToken>()), Times.Once);
        clientMock.Verify(c => c.Get(filters, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ReturnsPaginatedGroups_WhenGroupsExist()
    {
        // Arrange
        var filters = new GroupQueryFilters { Page = 1, PageSize = 2 };
        var query = new GetGroupsQuery(filters);

        var groups = new List<GroupRepresentation>
        {
            new() { Id = Guid.NewGuid(), Name = "Group 1" },
            new() { Id = Guid.NewGuid(), Name = "Group 2" }
        };

        clientMock
            .Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 10 }));

        clientMock
            .Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Succeeded(groups));

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value.Total);
        Assert.Equal(2, result.Value.Results.Count());
        groups.ForEach(g => Assert.Contains(result.Value.Results, rg => rg.Name == g.Name));
        clientMock.Verify(c => c.GetGroupsCount(It.IsAny<CancellationToken>()), Times.Once);
        clientMock.Verify(c => c.Get(filters, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_MapsGroupsCorrectly()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var filters = new GroupQueryFilters { Page = 1, PageSize = 10 };
        var query = new GetGroupsQuery(filters);

        var groups = new List<GroupRepresentation>
        {
            new() { Id = groupId, Name = "Test Group" }
        };

        clientMock
            .Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 1 }));

        clientMock
            .Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Succeeded(groups));

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        var groupResponse = result.Value.Results.First();
        Assert.Equal(groupId, groupResponse.Id);
        Assert.Equal("Test Group", groupResponse.Name);
    }

    [Fact]
    public async Task HandleAsync_PropagatesFailure_WhenGetGroupsCountFails()
    {
        // Arrange
        var filters = new GroupQueryFilters { Page = 1, PageSize = 10 };
        var query = new GetGroupsQuery(filters);

        clientMock
            .Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Failed(
                new ErrorRepresentation { Error = "server_error", Description = "Internal server error" },
                HttpStatusCode.InternalServerError));

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        clientMock.Verify(c => c.GetGroupsCount(It.IsAny<CancellationToken>()), Times.Once);
        clientMock.Verify(c => c.Get(It.IsAny<GroupQueryFilters>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_PropagatesFailure_WhenGetGroupsFails()
    {
        // Arrange
        var filters = new GroupQueryFilters { Page = 1, PageSize = 10 };
        var query = new GetGroupsQuery(filters);

        clientMock
            .Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 5 }));

        clientMock
            .Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Failed(
                new ErrorRepresentation { Error = "forbidden", Description = "Access denied" },
                HttpStatusCode.Forbidden));

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        clientMock.Verify(c => c.GetGroupsCount(It.IsAny<CancellationToken>()), Times.Once);
        clientMock.Verify(c => c.Get(filters, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_HandlesUnauthorizedError()
    {
        // Arrange
        var filters = new GroupQueryFilters { Page = 1, PageSize = 10 };
        var query = new GetGroupsQuery(filters);

        clientMock
            .Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Failed(
                new ErrorRepresentation { Error = "unauthorized" },
                HttpStatusCode.Unauthorized));

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<BlazorFurniture.Core.Shared.Errors.GenericError>(result.Error);
    }

    [Fact]
    public async Task HandleAsync_AppliesPaginationFilters()
    {
        // Arrange
        var filters = new GroupQueryFilters { Page = 2, PageSize = 5, Name = "Admin" };
        var query = new GetGroupsQuery(filters);

        var groups = new List<GroupRepresentation>
        {
            new() { Id = Guid.NewGuid(), Name = "Admins" }
        };

        clientMock
            .Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 10 }));

        clientMock
            .Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Succeeded(groups));

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        clientMock.Verify(c => c.Get(
            It.Is<GroupQueryFilters>(f =>
                f.Page == 2 &&
                f.PageSize == 5 &&
                f.Name == "Admin"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_RespectsDefaultPaginationValues()
    {
        // Arrange
        var filters = new GroupQueryFilters(); // Using defaults
        var query = new GetGroupsQuery(filters);

        clientMock
            .Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 0 }));

        clientMock
            .Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Succeeded(
                new List<GroupRepresentation>()));

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        clientMock.Verify(c => c.Get(
            It.Is<GroupQueryFilters>(f =>
                f.Page == 0 &&
                f.PageSize == 10),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_HandlesCancellation()
    {
        // Arrange
        var filters = new GroupQueryFilters { Page = 1, PageSize = 10 };
        var query = new GetGroupsQuery(filters);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        clientMock
            .Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await handler.HandleAsync(query, cts.Token));
    }
}
