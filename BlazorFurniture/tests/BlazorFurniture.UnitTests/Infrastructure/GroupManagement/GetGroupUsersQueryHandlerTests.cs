using AutoFixture;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Queries;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Domain.Enums;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Queries;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.GroupManagement;

public class GetGroupUsersQueryHandlerTests
{
    private readonly IGroupManagementClient clientMock;
    private readonly KeycloakHttpErrorMapper errorMapper;
    private readonly GetGroupUsersQueryHandler handler;
    private readonly Fixture fixture;

    public GetGroupUsersQueryHandlerTests()
    {
        clientMock = Substitute.For<IGroupManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        handler = new GetGroupUsersQueryHandler(clientMock, errorMapper);
        fixture = new Fixture();
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyPaginatedResponse_WhenNoUsersExist()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest { GroupId = groupId, Page = 1, PageSize = 10 };
        var query = new GetGroupUsersQuery(request);

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
            .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 0 }));

        clientMock.GetUsers(groupId, Arg.Is<GroupUsersQueryFilter>(f => f.Page == 1 && f.PageSize == 10), Arg.Any<CancellationToken>())
            .Returns(HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>.Succeeded([]));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.Total);
        Assert.Empty(result.Value.Results);
        await clientMock.Received(1).GetUsersCount(groupId, null, Arg.Any<CancellationToken>());
        await clientMock.Received(1).GetUsers(groupId,
            Arg.Is<GroupUsersQueryFilter>(f => f.Page == 1 && f.PageSize == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsPaginatedUsers_WhenUsersExist()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest
        {
            GroupId = groupId,
            Page = 0,
            PageSize = 2
        };
        var query = new GetGroupUsersQuery(request);

        var users = fixture.Build<GroupUserRepresentation>()
            .With(u => u.Email, "user@example.com")
            .With(u => u.FirstName, "John")
            .With(u => u.LastName, "Doe")
            .CreateMany(2)
            .ToList();

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
            .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 10 }));

        clientMock.GetUsers(groupId,
                Arg.Is<GroupUsersQueryFilter>(f => f.Page == 0 && f.PageSize == 2),
                Arg.Any<CancellationToken>())
            .Returns(HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>.Succeeded(users));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value.Total);
        Assert.Equal(2, result.Value.Results.Count());
        users.ForEach(u => Assert.Contains(result.Value.Results, ru => ru.Email == u.Email));
        await clientMock.Received(1).GetUsersCount(groupId, null, Arg.Any<CancellationToken>());
        await clientMock.Received(1).GetUsers(groupId, Arg.Is<GroupUsersQueryFilter>(f => f.Page == 0 && f.PageSize == 2), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_MapsUsersCorrectly()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var firstName = "Jane";
        var lastName = "Smith";
        var role = GroupRoles.GroupAdmin;

        var request = new GetGroupUsersRequest { GroupId = groupId, Page = 0, PageSize = 10 };
        var query = new GetGroupUsersQuery(request);

        var users = new List<GroupUserRepresentation>
         {
 new()
       {
      Id = userId,
Email = email,
     FirstName = firstName,
    LastName = lastName,
        Enabled = true,
     Role = role
     }
        };

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
  .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
              new CountRepresentation { Count = 1 }));

        clientMock.GetUsers(groupId, Arg.Is<GroupUsersQueryFilter>(f => f.Page == 0 && f.PageSize == 10),
     Arg.Any<CancellationToken>())
          .Returns(HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>.Succeeded(users));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var userResponse = result.Value.Results.First();
        Assert.Equal(userId, userResponse.Id);
        Assert.Equal(email, userResponse.Email);
        Assert.Equal(firstName, userResponse.FirstName);
        Assert.Equal(lastName, userResponse.LastName);
        Assert.Equal(role, userResponse.Role);
    }

    [Fact]
    public async Task HandleAsync_VerifiesCountMatchesTotalInResponse()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var expectedCount = 25;
        var request = new GetGroupUsersRequest
        {
            GroupId = groupId,
            Page = 0,
            PageSize = 10
        };
        var query = new GetGroupUsersQuery(request);

        var users = fixture.Build<GroupUserRepresentation>()
            .With(u => u.Email, "user@example.com")
            .With(u => u.FirstName, "User")
            .With(u => u.LastName, "Test")
            .CreateMany(10)
            .ToList();

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
        .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
          new CountRepresentation { Count = expectedCount }));

        clientMock.GetUsers(groupId, Arg.Any<GroupUsersQueryFilter>(), Arg.Any<CancellationToken>())
.Returns(HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>.Succeeded(users));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCount, result.Value.Total);
        Assert.Equal(10, result.Value.Results.Count());
    }

    [Fact]
    public async Task HandleAsync_PropagatesFailure_WhenGetUsersCountFails()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest { GroupId = groupId, Page = 0, PageSize = 10 };
        var query = new GetGroupUsersQuery(request);
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
  .With(e => e.Error, "server_error")
        .With(e => e.Description, "Internal server error")
       .Without(e => e.Errors)
.Create();

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
      .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Failed(
     errorRepresentation, HttpStatusCode.InternalServerError));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        await clientMock.Received(1).GetUsersCount(groupId, null, Arg.Any<CancellationToken>());
        await clientMock.DidNotReceive().GetUsers(Arg.Any<Guid>(), Arg.Any<GroupUsersQueryFilter>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_PropagatesFailure_WhenGetUsersFails()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest { GroupId = groupId, Page = 1, PageSize = 20, Search = "John" };
        var query = new GetGroupUsersQuery(request);
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
  .With(e => e.Error, "forbidden")
       .With(e => e.Description, "Access denied")
        .Without(e => e.Errors)
.Create();

        clientMock.GetUsersCount(groupId, "John", Arg.Any<CancellationToken>())
        .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
           new CountRepresentation { Count = 5 }));

        clientMock.GetUsers(groupId, Arg.Is<GroupUsersQueryFilter>(f => f.Page == 1 && f.PageSize == 20 && f.Search == "John"),
           Arg.Any<CancellationToken>())
.Returns(HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>.Failed(
    errorRepresentation, HttpStatusCode.Forbidden));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        await clientMock.Received(1).GetUsersCount(groupId, "John", Arg.Any<CancellationToken>());
        await clientMock.Received(1).GetUsers(groupId, Arg.Is<GroupUsersQueryFilter>(f => f.Page == 1 && f.PageSize == 20 && f.Search == "John"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_HandlesUnauthorizedError()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest { GroupId = groupId, Page = 0, PageSize = 10 };
        var query = new GetGroupUsersQuery(request);
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
  .With(e => e.Error, "unauthorized")
   .Without(e => e.Errors)
            .Create();

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
       .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Failed(
       errorRepresentation, HttpStatusCode.Unauthorized));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<GenericError>(result.Error);
    }

    [Fact]
    public async Task HandleAsync_AppliesPaginationAndSearchFilters()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest
        {
            GroupId = groupId,
            Page = 2,
            PageSize = 5,
            Search = "John"
        };
        var query = new GetGroupUsersQuery(request);

        var users = fixture.Build<GroupUserRepresentation>()
        .With(u => u.FirstName, "John")
            .With(u => u.Email, "john@example.com")
      .With(u => u.LastName, "Doe")
       .CreateMany(1)
            .ToList();

        clientMock.GetUsersCount(groupId, "John", Arg.Any<CancellationToken>())
   .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
     new CountRepresentation { Count = 10 }));

        clientMock.GetUsers(groupId, Arg.Is<GroupUsersQueryFilter>(f => f.Page == 2 && f.PageSize == 5 && f.Search == "John"),
        Arg.Any<CancellationToken>())
            .Returns(HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>.Succeeded(users));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        await clientMock.Received(1).GetUsers(groupId,
            Arg.Is<GroupUsersQueryFilter>(f => f.Page == 2 && f.PageSize == 5 && f.Search == "John"),
     Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_RespectsDefaultPaginationValues()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest { GroupId = groupId }; // Using defaults (Page = 0, PageSize = 10)
        var query = new GetGroupUsersQuery(request);

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
    .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
   new CountRepresentation { Count = 0 }));

        clientMock.GetUsers(groupId, Arg.Is<GroupUsersQueryFilter>(f => f.Page == 0 && f.PageSize == 10),
            Arg.Any<CancellationToken>())
            .Returns(HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>.Succeeded([]));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        await clientMock.Received(1).GetUsers(groupId,
      Arg.Is<GroupUsersQueryFilter>(f => f.Page == 0 && f.PageSize == 10),
    Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_PassesCorrectGroupIdToClient()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest { GroupId = groupId, Page = 0, PageSize = 10 };
        var query = new GetGroupUsersQuery(request);

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
            .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
    new CountRepresentation { Count = 0 }));

        clientMock.GetUsers(groupId, Arg.Any<GroupUsersQueryFilter>(), Arg.Any<CancellationToken>())
           .Returns(HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>.Succeeded([]));

        // Act
        await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        await clientMock.Received(1).GetUsersCount(groupId, null, Arg.Any<CancellationToken>());
        await clientMock.Received(1).GetUsers(groupId, Arg.Any<GroupUsersQueryFilter>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_HandlesMultipleUsersWithDifferentRoles()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest { GroupId = groupId, Page = 0, PageSize = 10 };
        var query = new GetGroupUsersQuery(request);

        var users = new List<GroupUserRepresentation>
        {
       new()
      {
                Id = Guid.NewGuid(),
       Email = "admin@example.com",
        FirstName = "Admin",
             LastName = "User",
 Enabled = true,
        Role = GroupRoles.GroupAdmin
   },
       new()
   {
    Id = Guid.NewGuid(),
            Email = "member@example.com",
        FirstName = "Member",
    LastName = "User",
    Enabled = true,
    Role = GroupRoles.GroupMember
},
new()
{
    Id = Guid.NewGuid(),
    Email = "undefined@example.com",
    FirstName = "Undefined",
    LastName = "User",
    Enabled = false,
    Role = GroupRoles.Undefined
}
};

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
            .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                new CountRepresentation { Count = 3 }));

        clientMock.GetUsers(groupId, Arg.Any<GroupUsersQueryFilter>(), Arg.Any<CancellationToken>())
            .Returns(HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>.Succeeded(users));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Results.Count());

        var adminUser = result.Value.Results.First(u => u.Role == GroupRoles.GroupAdmin);
        Assert.Equal("admin@example.com", adminUser.Email);

        var memberUser = result.Value.Results.First(u => u.Role == GroupRoles.GroupMember);
        Assert.Equal("member@example.com", memberUser.Email);

        var undefinedUser = result.Value.Results.First(u => u.Role == GroupRoles.Undefined);
        Assert.Equal("undefined@example.com", undefinedUser.Email);
    }

    [Fact]
    public async Task HandleAsync_HandlesCancellation()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest { GroupId = groupId, Page = 1, PageSize = 10 };
        var query = new GetGroupUsersQuery(request);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await handler.HandleAsync(query, cts.Token));
    }

    [Fact]
    public async Task HandleAsync_HandlesNotFoundError()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest { GroupId = groupId, Page = 0, PageSize = 10 };
        var query = new GetGroupUsersQuery(request);
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "not_found")
            .With(e => e.Description, "Group not found")
            .Without(e => e.Errors)
            .Create();

        clientMock.GetUsersCount(groupId, null, Arg.Any<CancellationToken>())
            .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Failed(
                errorRepresentation, HttpStatusCode.NotFound));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task HandleAsync_AppliesSearchFilterOnly()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var request = new GetGroupUsersRequest
        {
            GroupId = groupId,
            Page = 0,
            PageSize = 10,
            Search = "Jane"
        };
        var query = new GetGroupUsersQuery(request);

        var users = fixture.Build<GroupUserRepresentation>()
            .With(u => u.FirstName, "Jane")
            .With(u => u.Email, "jane@example.com")
            .With(u => u.LastName, "Doe")
            .CreateMany(1)
            .ToList();

        clientMock.GetUsersCount(groupId, "Jane", Arg.Any<CancellationToken>())
            .Returns(HttpResult<CountRepresentation, ErrorRepresentation>.Succeeded(
                 new CountRepresentation { Count = 1 }));

        clientMock.GetUsers(groupId, Arg.Is<GroupUsersQueryFilter>(f => f.Search == "Jane"), Arg.Any<CancellationToken>())
            .Returns(HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>.Succeeded(users));

        // Act
        var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        await clientMock.Received(1).GetUsers(groupId,
            Arg.Is<GroupUsersQueryFilter>(f => f.Search == "Jane"),
            Arg.Any<CancellationToken>());
    }
}
