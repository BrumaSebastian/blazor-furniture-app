using AutoFixture;
using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using NSubstitute;

namespace BlazorFurniture.UnitTests.Application.Features.GroupManagement.Commands;

public class UpdateGroupCommandTests
{
    private readonly Fixture fixture;
    private readonly ICommandDispatcher mockCommandDispatcher;

    public UpdateGroupCommandTests()
    {
        fixture = new Fixture();
        mockCommandDispatcher = Substitute.For<ICommandDispatcher>();
    }

    [Fact]
    public async Task Dispatcher_ExecutesUpdateCommand_WithCorrectParameters()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();

        var command = new UpdateGroupCommand(request);
        mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Succeeded(new EmptyResult()));


        // Act
        var result = await mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(command, TestContext.Current.CancellationToken);

        // Assert - Verify command execution and parameters
        Assert.True(result.IsSuccess);
        Assert.NotNull(command);
        Assert.Equal(request.GroupId, command.Request.GroupId);
        Assert.Equal(request.Name, command.Request.Name);
        await mockCommandDispatcher.Received(1).DispatchCommand<UpdateGroupCommand, EmptyResult>(
            command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Dispatcher_ReturnsSuccess_WhenUpdateSucceeds()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        var command = new UpdateGroupCommand(request);

        mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Succeeded(new EmptyResult()));

        // Act
        var result = await mockCommandDispatcher.DispatchCommand<UpdateGroupCommand, EmptyResult>(
            command, TestContext.Current.CancellationToken);

        // Assert - Verify successful result
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.IsType<EmptyResult>(result.Value);

        await mockCommandDispatcher.Received(1)
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Dispatcher_ReturnsNotFoundError_WhenGroupDoesNotExist()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        var notFoundError = new NotFoundError(request.GroupId, typeof(UpdateGroupRequest));
        var command = new UpdateGroupCommand(request);

        mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Failed(notFoundError));


        // Act
        var result = await mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                 command, TestContext.Current.CancellationToken);

        // Assert - Verify NotFound error
        Assert.False(result.IsSuccess);
        Assert.IsType<NotFoundError>(result.Error);
        Assert.Contains(request.GroupId.ToString(), result.Error.Description);

        await mockCommandDispatcher.Received(1)
               .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                    command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Dispatcher_ReturnsConflictError_WhenGroupNameAlreadyExists()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        var conflictError = new ConflictError("name", request.Name, typeof(UpdateGroupRequest));
        var command = new UpdateGroupCommand(request);

        mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Failed(conflictError));


        // Act
        var result = await mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, TestContext.Current.CancellationToken);

        // Assert - Verify Conflict error
        Assert.False(result.IsSuccess);
        Assert.IsType<ConflictError>(result.Error);
        Assert.Contains("conflict", result.Error.Title.ToLower());

        await mockCommandDispatcher.Received(1)
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>());
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
        var command = new UpdateGroupCommand(request);

        mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Failed(validationError));


        // Act
        var result = await mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, TestContext.Current.CancellationToken);

        // Assert - Verify Validation error
        Assert.False(result.IsSuccess);
        Assert.IsType<ValidationError>(result.Error);

        var validationErr = result.Error as ValidationError;
        Assert.NotNull(validationErr);
        Assert.Contains("Name", validationErr.Errors.Keys);
        Assert.Equal(2, validationErr.Errors["Name"].Length);
        Assert.Contains("Name is required", validationErr.Errors["Name"]);

        await mockCommandDispatcher.Received(1)
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Dispatcher_PropagatesCancellationToken()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        CancellationToken capturedToken = default;
        var command = new UpdateGroupCommand(request);

        mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                capturedToken = callInfo.Arg<CancellationToken>();
                return Result<EmptyResult>.Succeeded(new EmptyResult());
            });


        // Act
        await mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, cancellationToken);

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
        var command = new UpdateGroupCommand(request);

        mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>())
            .Returns(Result<EmptyResult>.Failed(unexpectedError));


        // Act
        var result = await mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, TestContext.Current.CancellationToken);

        // Assert - Verify generic error handling
        Assert.False(result.IsSuccess);
        Assert.IsType<GenericError>(result.Error);
        Assert.Contains("unexpected", result.Error.Description.ToLower());

        await mockCommandDispatcher.Received(1)
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>());
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
        Assert.Equal(request.GroupId, command.Request.GroupId);
        Assert.Equal(request.Name, command.Request.Name);
    }

    [Fact]
    public void UpdateGroupRequest_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var request = fixture.Create<UpdateGroupRequest>();

        // Assert
        Assert.NotEqual(Guid.Empty, request.GroupId);
        Assert.False(string.IsNullOrEmpty(request.Name));
    }

    [Fact]
    public async Task Dispatcher_VerifiesCommandMatchesRequest()
    {
        // Arrange
        var request = fixture.Create<UpdateGroupRequest>();
        UpdateGroupCommand? capturedCommand = null;
        var command = new UpdateGroupCommand(request);

        mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, Arg.Any<CancellationToken>())
            .Returns(callInfo =>
                {
                    capturedCommand = callInfo.Arg<UpdateGroupCommand>();
                    return Result<EmptyResult>.Succeeded(new EmptyResult());
                });


        // Act
        await mockCommandDispatcher
            .DispatchCommand<UpdateGroupCommand, EmptyResult>(
                command, TestContext.Current.CancellationToken);

        // Assert - Verify command contains exact request data
        Assert.NotNull(capturedCommand);
        Assert.Same(request, capturedCommand.Request);
        Assert.Equal(request.GroupId, capturedCommand.Request.GroupId);
        Assert.Equal(request.Name, capturedCommand.Request.Name);
    }
}
