using AutoFixture;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Commands;
using Moq;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.GroupManagement;

public class UpdateGroupCommandHandlerTests
{
    private readonly Mock<IGroupManagementClient> clientMock;
    private readonly KeycloakHttpErrorMapper errorMapper;
    private readonly Fixture fixture;
    private readonly UpdateGroupCommandHandler handler;

    public UpdateGroupCommandHandlerTests()
    {
        clientMock = new Mock<IGroupManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        fixture = new Fixture();
        handler = new UpdateGroupCommandHandler(clientMock.Object, errorMapper);
    }

    [Fact]
    public async Task HandleAsync_UpdatesGroup_OnSuccess()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var updateRequest = fixture.Build<UpdateGroupRequest>()
            .With(f => f.Id, groupId)
            .Create();
        var command = new UpdateGroupCommand(updateRequest);
        var groupRepresentation = new GroupRepresentation
        {
            Id = groupId,
            Name = "OldGroupName"
        };

        clientMock.Setup(c => c.Get(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<GroupRepresentation, ErrorRepresentation>.Succeeded(groupRepresentation));

        clientMock.Setup(c => c.Update(groupId, groupRepresentation, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<EmptyResult, ErrorRepresentation>.Succeeded(new EmptyResult()));

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(updateRequest.Name, groupRepresentation.Name);
        clientMock.Verify(c => c.Update(groupId, groupRepresentation, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PropagatesNotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var updateRequest = fixture.Build<UpdateGroupRequest>()
            .With(f => f.Id, groupId)
            .Create();
        var command = new UpdateGroupCommand(updateRequest);
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "not_found")
            .Without(e => e.Errors)
            .Create();
        clientMock.Setup(c => c.Get(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<GroupRepresentation, ErrorRepresentation>.Failed(
                errorRepresentation,
                HttpStatusCode.NotFound));

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
        clientMock.Verify(c => c.Get(groupId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PropagatesError_OnConflictingName()
    {
        // Arrange
        var updateRequest = fixture.Create<UpdateGroupRequest>();
        var command = new UpdateGroupCommand(updateRequest);
        var groupRepresentation = fixture.Build<GroupRepresentation>()
            .With(f => f.Id, updateRequest.Id)
            .Create();

        clientMock.Setup(c => c.Get(updateRequest.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<GroupRepresentation, ErrorRepresentation>.Succeeded(groupRepresentation));

        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "name-conflict")
            .Without(e => e.Errors)
            .Create();

        clientMock.Setup(c => c.Update(updateRequest.Id, groupRepresentation, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<EmptyResult, ErrorRepresentation>.Failed(
                errorRepresentation,
                HttpStatusCode.Conflict));

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<ConflictError>(result.Error);
        clientMock.Verify(c => c.Update(updateRequest.Id, groupRepresentation, It.IsAny<CancellationToken>()), Times.Once);
    }
}
