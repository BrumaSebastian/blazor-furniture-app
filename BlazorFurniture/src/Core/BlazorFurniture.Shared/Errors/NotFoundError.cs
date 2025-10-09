namespace BlazorFurniture.Core.Shared.Errors;

public class NotFoundError( Guid resourceId, Type type ) 
    : BasicError("resource-not-found", $"Resource of type {type.Name} with id {resourceId} was not found")
{
}
