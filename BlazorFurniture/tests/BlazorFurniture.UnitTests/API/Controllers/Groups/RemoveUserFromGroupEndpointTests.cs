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
    public async Task RemoveUserFromGroupEndpoint_OnSucessResult_Returns204()
    {
        // Arrange
        var request = fixture.Create<RemoveUserFromGroupRequest>();
        var httpContext = new DefaultHttpContext();
        var endpoint = Factory.Create<RemoveUserFromGroupEndpoint>(httpContext, commandDispatcher);

        commandDispatcher
           .Dispatch<RemoveUserFromGroupCommand, Result<EmptyResult>>(
               new RemoveUserFromGroupCommand(request),
               Arg.Any<CancellationToken>())
           .Returns(Result<EmptyResult>.Succeeded(new EmptyResult()));

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await commandDispatcher.Received(1)
            .Dispatch<RemoveUserFromGroupCommand, Result<EmptyResult>>(
                Arg.Any<RemoveUserFromGroupCommand>(),
                Arg.Any<CancellationToken>());

        Assert.Equal(StatusCodes.Status204NoContent, httpContext.Response.StatusCode);
    }
}
