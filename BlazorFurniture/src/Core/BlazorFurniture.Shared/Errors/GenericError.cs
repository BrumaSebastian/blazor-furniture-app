namespace BlazorFurniture.Core.Shared.Errors;

public class GenericError( string description ) : BasicError("generic-error", description)
{
}
