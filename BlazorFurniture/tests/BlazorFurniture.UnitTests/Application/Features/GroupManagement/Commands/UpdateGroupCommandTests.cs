using AutoFixture;
using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using Moq;

namespace BlazorFurniture.UnitTests.Application.Features.GroupManagement.Commands;

public class UpdateGroupCommandTests
{
    private readonly Fixture fixture;

    public UpdateGroupCommandTests()
    {
        fixture = new Fixture();
    }
    [Fact]
    public async Task Dispatcher_ExecutesUpdateCommand_WithCorrectParameters()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();

        UpdateGroupCommand? capturedCommand = null;
        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher
            .Setup(d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()))
            .Callback<UpdateGroupCommand, CancellationToken>(( cmd, ct ) => capturedCommand = cmd)
            .ReturnsAsync(Result<EmptyResult>.Succeeded(new EmptyResult()));

        var command = new UpdateGroupCommand(request);

        // Act
        var result = await mockDispatcher.Object.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
            command,
            TestContext.Current.CancellationToken);

        // Assert - Verify command execution and parameters
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedCommand);
        Assert.Equal(request.Id, capturedCommand.Request.Id);
        Assert.Equal(request.Name, capturedCommand.Request.Name);
        mockDispatcher.Verify(
            d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dispatcher_ReturnsSuccess_WhenUpdateSucceeds()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();

        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher
            .Setup(d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.Is<UpdateGroupCommand>(cmd =>
                    cmd.Request.Id == request.Id &&
                    cmd.Request.Name == request.Name),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmptyResult>.Succeeded(new EmptyResult()));

        var command = new UpdateGroupCommand(request);

        // Act
        var result = await mockDispatcher.Object.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
            command,
            TestContext.Current.CancellationToken);

        // Assert - Verify successful result
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.IsType<EmptyResult>(result.Value);

        mockDispatcher.Verify(
            d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dispatcher_ReturnsNotFoundError_WhenGroupDoesNotExist()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        var notFoundError = new NotFoundError(request.Id, typeof(UpdateGroupRequest));

        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher
            .Setup(d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmptyResult>.Failed(notFoundError));

        var command = new UpdateGroupCommand(request);

        // Act
        var result = await mockDispatcher.Object.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
            command,
            TestContext.Current.CancellationToken);

        // Assert - Verify NotFound error
        Assert.False(result.IsSuccess);
        Assert.IsType<NotFoundError>(result.Error);
        Assert.True(result.Error.Description.Contains(request.Id.ToString()));

        mockDispatcher.Verify(
            d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dispatcher_ReturnsConflictError_WhenGroupNameAlreadyExists()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        var conflictError = new ConflictError("name", request.Name, typeof(UpdateGroupRequest));

        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher
            .Setup(d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmptyResult>.Failed(conflictError));

        var command = new UpdateGroupCommand(request);

        // Act
        var result = await mockDispatcher.Object.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
            command,
            TestContext.Current.CancellationToken);

        // Assert - Verify Conflict error
        Assert.False(result.IsSuccess);
        Assert.IsType<ConflictError>(result.Error);
        Assert.Contains("conflict", result.Error.Title.ToLower());

        mockDispatcher.Verify(
            d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dispatcher_ReturnsValidationError_WhenValidationFails()
    {
        // Arrange
        var request = fixture.Build<UpdateGroupRequest>()
            .With(r => r.Name, string.Empty) // Invalid empty name
            .Create();

        var validationErrors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required", "Name must not be empty" } }
        };
        var validationError = new ValidationError(validationErrors);

        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher
            .Setup(d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmptyResult>.Failed(validationError));

        var command = new UpdateGroupCommand(request);

        // Act
        var result = await mockDispatcher.Object.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
            command,
            TestContext.Current.CancellationToken);

        // Assert - Verify Validation error
        Assert.False(result.IsSuccess);
        Assert.IsType<ValidationError>(result.Error);

        var validationErr = result.Error as ValidationError;
        Assert.NotNull(validationErr);
        Assert.Contains("Name", validationErr.Errors.Keys);
        Assert.Equal(2, validationErr.Errors["Name"].Length);
        Assert.Contains("Name is required", validationErr.Errors["Name"]);

        mockDispatcher.Verify(
            d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dispatcher_PropagatesCancellationToken()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        CancellationToken capturedToken = default;

        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher
            .Setup(d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()))
            .Callback<UpdateGroupCommand, CancellationToken>(( cmd, ct ) => capturedToken = ct)
            .ReturnsAsync(Result<EmptyResult>.Succeeded(new EmptyResult()));

        var command = new UpdateGroupCommand(request);

        // Act
        await mockDispatcher.Object.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
            command,
            cancellationToken);

        // Assert - Verify cancellation token propagation
        Assert.Equal(cancellationToken, capturedToken);
        Assert.False(capturedToken.IsCancellationRequested);
    }

    [Fact]
    public async Task Dispatcher_ReturnsGenericError_ForUnexpectedErrors()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        var unexpectedError = new GenericError("An unexpected error occurred");

        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher
            .Setup(d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmptyResult>.Failed(unexpectedError));

        var command = new UpdateGroupCommand(request);

        // Act
        var result = await mockDispatcher.Object.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
            command,
            TestContext.Current.CancellationToken);

        // Assert - Verify generic error handling
        Assert.False(result.IsSuccess);
        Assert.IsType<GenericError>(result.Error);
        Assert.Contains("unexpected", result.Error.Description.ToLower());

        mockDispatcher.Verify(
            d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void UpdateGroupCommand_CreatesCorrectlyWithRequest()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();

        // Act
        var command = new UpdateGroupCommand(request);

        // Assert
        Assert.NotNull(command);
        Assert.NotNull(command.Request);
        Assert.Equal(request.Id, command.Request.Id);
        Assert.Equal(request.Name, command.Request.Name);
    }

    [Fact]
    public void UpdateGroupRequest_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var request = fixture.Create<UpdateGroupRequest>();

        // Assert
        Assert.NotEqual(Guid.Empty, request.Id);
        Assert.False(string.IsNullOrEmpty(request.Name));
    }

    [Fact]
    public async Task Dispatcher_VerifiesCommandMatchesRequest()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        UpdateGroupCommand? capturedCommand = null;

        var mockDispatcher = new Mock<ICommandDispatcher>();
        mockDispatcher
            .Setup(d => d.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
                It.IsAny<UpdateGroupCommand>(),
                It.IsAny<CancellationToken>()))
            .Callback<UpdateGroupCommand, CancellationToken>(( cmd, ct ) => capturedCommand = cmd)
            .ReturnsAsync(Result<EmptyResult>.Succeeded(new EmptyResult()));

        var command = new UpdateGroupCommand(request);

        // Act
        await mockDispatcher.Object.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(
            command,
            TestContext.Current.CancellationToken);

        // Assert - Verify command contains exact request data
        Assert.NotNull(capturedCommand);
        Assert.Same(request, capturedCommand.Request);
        Assert.Equal(request.Id, capturedCommand.Request.Id);
        Assert.Equal(request.Name, capturedCommand.Request.Name);
    }
}
