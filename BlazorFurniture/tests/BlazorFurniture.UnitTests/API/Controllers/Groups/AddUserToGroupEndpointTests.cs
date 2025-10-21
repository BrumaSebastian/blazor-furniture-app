using AutoFixture;
using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Controllers.Groups;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace BlazorFurniture.UnitTests.API.Controllers.Groups;

public sealed class AddUserToGroupEndpointTests
{
    private readonly ICommandDispatcher commandDispatcher;
    private readonly Fixture fixture;

    public AddUserToGroupEndpointTests()
    {
        commandDispatcher = Substitute.For<ICommandDispatcher>();
        fixture = new Fixture();
    }

    [Fact]
    public async Task AddUserToGroupEndpoint_OnSuccessResult_Returns204()
    {
        // Arrange
        var request = fixture.Create<AddUserToGroupRequest>();
        var httpContext = new DefaultHttpContext();
        var endpoint = Factory.Create<AddUserToGroupEndpoint>(httpContext, commandDispatcher);

        commandDispatcher
            .Dispatch<AddUserToGroupCommand, Result<EmptyResult>>(
                new AddUserToGroupCommand(request),
                Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Succeeded(new EmptyResult()));
        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await commandDispatcher.Received(1)
            .Dispatch<AddUserToGroupCommand, Result<EmptyResult>>(
                Arg.Any<AddUserToGroupCommand>(),
                Arg.Any<CancellationToken>());

        Assert.Equal(StatusCodes.Status204NoContent, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task AddUserToGroupEndpoint_OnUserNotFoundError_Returns404()
    {
        // Arrange
        var request = fixture.Create<AddUserToGroupRequest>();
        var httpContext = new DefaultHttpContext();
        var endpoint = Factory.Create<AddUserToGroupEndpoint>(httpContext, commandDispatcher);
        var notFoundError = new NotFoundError(request.UserId, typeof(UserRepresentation));
        commandDispatcher
            .Dispatch<AddUserToGroupCommand, Result<EmptyResult>>(
                Arg.Any<AddUserToGroupCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Failed(notFoundError));

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await commandDispatcher.Received(1)
            .Dispatch<AddUserToGroupCommand, Result<EmptyResult>>(
                Arg.Any<AddUserToGroupCommand>(),
                Arg.Any<CancellationToken>());
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task AddUserToGroupEndpoint_OnGroupNotFoundError_Returns404()
    {
        // Arrange
        var request = fixture.Create<AddUserToGroupRequest>();
        var httpContext = new DefaultHttpContext();
        var endpoint = Factory.Create<AddUserToGroupEndpoint>(httpContext, commandDispatcher);
        var notFoundError = new NotFoundError(request.GroupId, typeof(GroupRepresentation));
        commandDispatcher
            .Dispatch<AddUserToGroupCommand, Result<EmptyResult>>(
                Arg.Any<AddUserToGroupCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Failed(notFoundError));

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await commandDispatcher.Received(1)
            .Dispatch<AddUserToGroupCommand, Result<EmptyResult>>(
                Arg.Any<AddUserToGroupCommand>(),
                Arg.Any<CancellationToken>());
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
    }
}
