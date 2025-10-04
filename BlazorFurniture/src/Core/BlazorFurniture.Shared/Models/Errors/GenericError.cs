namespace BlazorFurniture.Core.Shared.Models.Errors;

public class GenericError( string description ) : BasicError("generic-error", description)
{
}
