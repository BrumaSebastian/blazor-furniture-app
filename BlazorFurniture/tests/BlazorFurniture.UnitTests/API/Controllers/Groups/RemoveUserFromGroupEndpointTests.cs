using AutoFixture;
using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Controllers.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace BlazorFurniture.UnitTests.API.Controllers.Groups;

public sealed class RemoveUserFromGroupEndpointTests
{
    private readonly ICommandDispatcher commandDispatcher;
    private readonly Fixture fixture;

    public RemoveUserFromGroupEndpointTests()
    {
        commandDispatcher = Substitute.For<ICommandDispatcher>();
        fixture = new Fixture();
    }

    [Fact]
    public async Task RemoveUserFromGroupEndpoint_OnSuccessResult_Returns204()
    {
        // Arrange
        var (request, httpContext, endpoint) = CreateTestContext();

        commandDispatcher
           .Dispatch<RemoveUserFromGroupCommand, Result<EmptyResult>>(
                Arg.Any<RemoveUserFromGroupCommand>(),
                Arg.Any<CancellationToken>())
           .Returns(Result<EmptyResult>.Succeeded(new EmptyResult()));

        // Act
        await endpoint.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        await VerifyDispatcherCalled();
        Assert.Equal(StatusCodes.Status204NoContent, httpContext.Response.StatusCode);
    }

    private (RemoveUserFromGroupRequest request, DefaultHttpContext httpContext, RemoveUserFromGroupEndpoint endpoint) CreateTestContext()
    {
        var request = fixture.Create<RemoveUserFromGroupRequest>();
        var httpContext = new DefaultHttpContext();
        var endpoint = Factory.Create<RemoveUserFromGroupEndpoint>(httpContext, commandDispatcher);
        return (request, httpContext, endpoint);
    }

    private async Task VerifyDispatcherCalled()
    {
        await commandDispatcher.Received(1)
           .Dispatch<RemoveUserFromGroupCommand, Result<EmptyResult>>(
                Arg.Any<RemoveUserFromGroupCommand>(),
                Arg.Any<CancellationToken>());
    }
}
