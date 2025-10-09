using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using System.Net;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;

internal class KeycloakHttpErrorMapper : IHttpErrorMapper
{
    public BasicError MapFor<TError>(
        TError error, HttpStatusCode status,
        Guid resourceId, Type? resourceType = null )
        where TError : class
    {
        if (error is not ErrorRepresentation errorRepresentation)
        {
            throw new InvalidOperationException("The error type is not supported by this mapper");
        }

        return status switch
        {
            HttpStatusCode.NotFound
                => new NotFoundError(resourceId, resourceType ?? typeof(object)),

            //HttpStatusCode.Conflict
            //    => new ConflictError(conflictField, resourceId, resourceType ?? typeof(object)),

            HttpStatusCode.BadRequest
                => new ValidationError(new Dictionary<string, string[]>()),

            HttpStatusCode.Unauthorized
                => new GenericError("unauthorized"),

            HttpStatusCode.Forbidden
                => new GenericError("forbidden"),

            HttpStatusCode.TooManyRequests
                => new GenericError("too-many-requests"),

            _ => new GenericError(errorRepresentation?.Description ?? errorRepresentation?.Error ?? "Unexpected error from Keycloak")
        };
    }
}
