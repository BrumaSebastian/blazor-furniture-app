using AutoFixture;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Commands;
using NSubstitute;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.GroupManagement;

public class UpdateGroupCommandHandlerTests
{
    private readonly IGroupManagementClient clientMock;
    private readonly KeycloakHttpErrorMapper errorMapper;
    private readonly Fixture fixture;
    private readonly UpdateGroupCommandHandler handler;

    public UpdateGroupCommandHandlerTests()
    {
        clientMock = Substitute.For<IGroupManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        fixture = new Fixture();
        handler = new UpdateGroupCommandHandler(clientMock, errorMapper);
    }

    [Fact]
    public async Task HandleAsync_UpdatesGroup_OnSuccess()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var updateRequest = fixture.Build<UpdateGroupRequest>()
            .With(f => f.GroupId, groupId)
            .Create();
        var command = new UpdateGroupCommand(updateRequest);
        var groupRepresentation = new GroupRepresentation
        {
            Id = groupId,
            Name = "OldGroupName"
        };

        clientMock.Get(groupId, Arg.Any<CancellationToken>())
            .Returns(HttpResult<GroupRepresentation, ErrorRepresentation>.Succeeded(groupRepresentation));

        clientMock.Update(groupId, groupRepresentation, Arg.Any<CancellationToken>())
            .Returns(HttpResult<EmptyResult, ErrorRepresentation>.Succeeded(new EmptyResult()));

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(updateRequest.Name, groupRepresentation.Name);
        await clientMock.Received(1).Update(groupId, groupRepresentation, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_PropagatesNotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var updateRequest = fixture.Build<UpdateGroupRequest>()
            .With(f => f.GroupId, groupId)
            .Create();
        var command = new UpdateGroupCommand(updateRequest);
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "not_found")
            .Without(e => e.Errors)
            .Create();

        clientMock.Get(groupId, Arg.Any<CancellationToken>())
            .Returns(HttpResult<GroupRepresentation, ErrorRepresentation>.Failed(
                errorRepresentation,
                HttpStatusCode.NotFound));

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
        await clientMock.Received(1).Get(groupId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_PropagatesError_OnConflictingName()
    {
        // Arrange
        var updateRequest = fixture.Create<UpdateGroupRequest>();
        var command = new UpdateGroupCommand(updateRequest);
        var groupRepresentation = fixture.Build<GroupRepresentation>()
            .With(f => f.Id, updateRequest.GroupId)
            .Create();

        clientMock.Get(updateRequest.GroupId, Arg.Any<CancellationToken>())
            .Returns(HttpResult<GroupRepresentation, ErrorRepresentation>.Succeeded(groupRepresentation));

        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "name-conflict")
            .Without(e => e.Errors)
            .Create();

        clientMock.Update(updateRequest.GroupId, groupRepresentation, Arg.Any<CancellationToken>())
            .Returns(HttpResult<EmptyResult, ErrorRepresentation>.Failed(
                errorRepresentation,
                HttpStatusCode.Conflict));

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<ConflictError>(result.Error);
        await clientMock.Received(1).Update(updateRequest.GroupId, groupRepresentation, Arg.Any<CancellationToken>());
    }
}
