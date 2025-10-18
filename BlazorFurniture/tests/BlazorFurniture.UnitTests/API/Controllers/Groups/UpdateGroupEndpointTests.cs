using AutoFixture;
using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Controllers.Groups;
using BlazorFurniture.Core.Shared.Errors;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace BlazorFurniture.UnitTests.API.Controllers.Groups;

public class UpdateGroupEndpointTests
{
    private readonly ICommandDispatcher commandDispatcher;
    private readonly IFixture fixture;

    public UpdateGroupEndpointTests()
    {
        commandDispatcher = Substitute.For<ICommandDispatcher>();
        fixture = new Fixture();
    }

    [Fact]
    public async Task HandleAsync_WithHttpContext_ShouldWork()
    {
        // Arrange
        var endpoint = Factory.Create<UpdateGroupEndpoint>(
            ctx =>
            {
                // Setup HTTP context
                ctx.Request.Method = "PUT";
                ctx.Request.Path = "/api/groups/123";
            },
            commandDispatcher);

        var request = fixture.Create<UpdateGroupRequest>();

        commandDispatcher
            .Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                Arg.Any<UpdateGroupCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Succeeded(new EmptyResult()));

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await commandDispatcher.Received(1)
            .Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                Arg.Any<UpdateGroupCommand>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithHttpContext_ShouldSetCorrectStatusCode()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var endpoint = Factory.Create<UpdateGroupEndpoint>(
            httpContext,
            commandDispatcher);

        var request = fixture.Create<UpdateGroupRequest>();

        commandDispatcher
            .Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                Arg.Any<UpdateGroupCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Succeeded(new EmptyResult()));

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status204NoContent, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task HandleAsync_WhenNotFound_ShouldSetNotFoundStatusCode()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var endpoint = Factory.Create<UpdateGroupEndpoint>(
            httpContext,
            commandDispatcher);

        var request = fixture.Create<UpdateGroupRequest>();
        var notFoundError = new NotFoundError(request.Id, typeof(UpdateGroupRequest));

        commandDispatcher
            .Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                Arg.Any<UpdateGroupCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Failed(notFoundError));

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
    }
}
