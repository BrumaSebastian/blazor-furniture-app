using AutoFixture;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Queries;
using BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;
using BlazorFurniture.Core.Shared.Errors;
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
    private readonly Fixture fixture;

    public GetGroupsQueryHandlerTests()
    {
        clientMock = new Mock<IGroupManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        handler = new GetGroupsQueryHandler(clientMock.Object, errorMapper);
        fixture = new Fixture();
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyPaginatedResponse_WhenNoGroupsExist()
    {
        // Arrange
        var filters = new GroupQueryFilters { Page = 1, PageSize = 10 };
        var query = new GetGroupsQuery(filters);

        clientMock.Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 0 }));

        clientMock.Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Succeeded([]));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

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
        var filters = new GroupQueryFilters
        {
            Page = 0,
            PageSize = 2
        };
        var query = new GetGroupsQuery(filters);

        var groups = fixture.Build<GroupRepresentation>()
            .CreateMany(2)
            .ToList();

        clientMock.Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 10 }));

        clientMock.Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Succeeded(groups));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

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
        var groupName = fixture.Create<string>();
        var filters = fixture.Create<GroupQueryFilters>();
        var query = new GetGroupsQuery(filters);

        var groups = new List<GroupRepresentation>
        {
            new() { Id = groupId, Name = groupName }
        };

        clientMock.Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 1 }));

        clientMock.Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Succeeded(groups));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var groupResponse = result.Value.Results.First();
        Assert.Equal(groupId, groupResponse.Id);
        Assert.Equal(groupName, groupResponse.Name);
    }

    [Fact]
    public async Task HandleAsync_PropagatesFailure_WhenGetGroupsCountFails()
    {
        // Arrange
        var filters = fixture.Create<GroupQueryFilters>();
        var query = new GetGroupsQuery(filters);
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "server_error")
            .With(e => e.Description, "Internal server error")
            .Without(e => e.Errors)
            .Create();

        clientMock.Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Failed(
                errorRepresentation,
                HttpStatusCode.InternalServerError));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

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
        var filters = fixture.Create<GroupQueryFilters>();
        var query = new GetGroupsQuery(filters);
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "forbidden")
            .With(e => e.Description, "Access denied")
            .Without(e => e.Errors)
            .Create();

        clientMock.Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 5 }));

        clientMock
            .Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Failed(
                errorRepresentation,
                HttpStatusCode.Forbidden));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

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
        var filters = fixture.Create<GroupQueryFilters>();
        var query = new GetGroupsQuery(filters);
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "unauthorized")
            .Without(e => e.Errors)
            .Create();

        clientMock.Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Failed(
                errorRepresentation,
                HttpStatusCode.Unauthorized));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<GenericError>(result.Error);
    }

    [Fact]
    public async Task HandleAsync_AppliesPaginationFilters()
    {
        // Arrange
        var filters = fixture.Build<GroupQueryFilters>()
            .With(f => f.Page, 2)
            .With(f => f.PageSize, 5)
            .With(f => f.Name, "Admin")
            .Create();
        var query = new GetGroupsQuery(filters);

        var groups = fixture.Build<GroupRepresentation>()
            .With(g => g.Name, "Admins")
            .CreateMany(1)
            .ToList();

        clientMock.Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 10 }));

        clientMock.Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Succeeded(groups));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

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

        clientMock.Setup(c => c.Get(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<List<GroupRepresentation>, ErrorRepresentation>.Succeeded(
                new List<GroupRepresentation>()));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

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

        clientMock.Setup(c => c.GetGroupsCount(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await handler.HandleAsync(query, cts.Token));
    }
}
