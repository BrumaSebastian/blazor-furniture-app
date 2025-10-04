namespace BlazorFurniture.Core.Shared.Models.Errors;

public class NotFoundError( string resourceId, object resource ) 
    : BasicError("resource-not-found", $"Resource of type {resource.GetType} with id {resourceId} was not found")
{
}
