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

public sealed class UpdateGroupEndpointTests
{
    private readonly ICommandDispatcher commandDispatcher;
    private readonly Fixture fixture;

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

        SetupSuccessfulDispatch();

        // Act
        await endpoint.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await VerifyDispatcherCalled();
    }

    [Fact]
    public async Task HandleAsync_WithHttpContext_ShouldSetCorrectStatusCode()
    {
        // Arrange
        var (request, httpContext, endpoint) = CreateTestContext();

        SetupSuccessfulDispatch();

        // Act
        await endpoint.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(StatusCodes.Status204NoContent, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task HandleAsync_WhenNotFound_ShouldSetNotFoundStatusCode()
    {
        // Arrange
        var (request, httpContext, endpoint) = CreateTestContext();
        var notFoundError = new NotFoundError(request.GroupId, typeof(UpdateGroupRequest));

        commandDispatcher
            .DispatchCommand<UpdateGroupCommand, Result<EmptyResult>>(
                Arg.Any<UpdateGroupCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Failed(notFoundError));

        // Act
        await endpoint.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
    }

    private (UpdateGroupRequest request, DefaultHttpContext httpContext, UpdateGroupEndpoint endpoint) CreateTestContext()
    {
        var request = fixture.Create<UpdateGroupRequest>();
        var httpContext = new DefaultHttpContext();
        var endpoint = Factory.Create<UpdateGroupEndpoint>(httpContext, commandDispatcher);
        return (request, httpContext, endpoint);
    }

    private void SetupSuccessfulDispatch()
    {
        commandDispatcher
            .DispatchCommand<UpdateGroupCommand, Result<EmptyResult>>(
                Arg.Any<UpdateGroupCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Succeeded(new EmptyResult()));
    }

    private async Task VerifyDispatcherCalled()
    {
        await commandDispatcher.Received(1)
            .DispatchCommand<UpdateGroupCommand, Result<EmptyResult>>(
                Arg.Any<UpdateGroupCommand>(),
                Arg.Any<CancellationToken>());
    }
}
