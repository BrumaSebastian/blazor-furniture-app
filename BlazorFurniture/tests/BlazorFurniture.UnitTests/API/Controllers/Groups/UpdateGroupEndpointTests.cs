using AutoFixture;
using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Controllers.Groups;
using NSubstitute;

namespace BlazorFurniture.UnitTests.API.Controllers.Groups;

public class UpdateGroupEndpointTests
{
    private readonly UpdateGroupEndpoint sut;
    private readonly ICommandDispatcher commandDispatcher;
    private readonly IFixture fixture;

    public UpdateGroupEndpointTests()
    {
        commandDispatcher = Substitute.For<ICommandDispatcher>();
        sut = new UpdateGroupEndpoint(commandDispatcher);
        fixture = new Fixture();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNoContent_WhenUpdateIsSuccessful()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        commandDispatcher
            .Dispatch<UpdateGroupCommand, Result<EmptyResult>>(Arg.Any<UpdateGroupCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Succeeded(new EmptyResult()));

        // Act
        await sut.HandleAsync(request, CancellationToken.None);

        // Assert
        await commandDispatcher.Received(1)
            .Dispatch<UpdateGroupCommand, Result<EmptyResult>>(Arg.Any<UpdateGroupCommand>(), Arg.Any<CancellationToken>());
    }
}
