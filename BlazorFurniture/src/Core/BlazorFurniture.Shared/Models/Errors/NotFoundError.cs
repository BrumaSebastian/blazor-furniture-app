namespace BlazorFurniture.Core.Shared.Models.Errors;

public class NotFoundError( Guid resourceId, Type type ) 
    : BasicError("resource-not-found", $"Resource of type {type.Name} with id {resourceId} was not found")
{
}
