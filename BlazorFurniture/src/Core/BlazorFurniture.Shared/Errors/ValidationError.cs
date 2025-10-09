namespace BlazorFurniture.Core.Shared.Errors;

public class ValidationError( IDictionary<string, string[]> errors ) 
    : BasicError("validation-error", "One or more validation errors occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}
