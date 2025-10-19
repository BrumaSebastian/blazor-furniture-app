using BlazorFurniture.Core.Shared.Errors;
using FastEndpoints;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace BlazorFurniture.Extensions.Endpoints;

public static class SendExtensions
{
    extension( IResponseSender sender )
    {
        public Task SendErrorAsync( BasicError error )
        {
            var problemDetails = new ProblemDetails
            {
                Title = error.Title,
                Detail = error.Description,
                Status = error switch
                {
                    NotFoundError => StatusCodes.Status404NotFound,
                    ConflictError => StatusCodes.Status409Conflict,
                    ValidationError => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                }
            };

            // Add validation errors if present
            if (error is ValidationError validationError)
            {
                problemDetails.Extensions["errors"] = validationError.Errors;
            }

            return sender.HttpContext.Response
                .SendAsync(problemDetails, problemDetails.Status ?? StatusCodes.Status500InternalServerError);
        }
    }
}
