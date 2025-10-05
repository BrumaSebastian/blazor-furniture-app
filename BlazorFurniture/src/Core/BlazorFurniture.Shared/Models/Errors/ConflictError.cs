namespace BlazorFurniture.Core.Shared.Models.Errors;

public class ConflictError( string field, string fieldValue, object resource ) 
    : BasicError("resource-conflict", $"A resource of type {resource.GetType} with {field} and value {fieldValue} already exists")
{
}
