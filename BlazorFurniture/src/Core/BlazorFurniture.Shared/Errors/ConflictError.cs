namespace BlazorFurniture.Core.Shared.Errors;

public class ConflictError : BasicError
{
    public ConflictError( string field, string fieldValue, object resource )
        : base("resource-conflict", $"A resource of type {resource.GetType} with {field} and value {fieldValue} already exists")
    {

    }

    public ConflictError( string message ) : base("resource-conflict", message)
    {

    }
}
