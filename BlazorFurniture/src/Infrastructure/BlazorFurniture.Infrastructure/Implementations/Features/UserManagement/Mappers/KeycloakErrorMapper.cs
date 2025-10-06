using BlazorFurniture.Core.Shared.Models.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using System.Net;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;

public static class KeycloakErrorMapper
{
    extension( BasicError basicError )
    {
        public static BasicError Map(
            HttpStatusCode status,
            ErrorRepresentation kcError,
            Guid resourceId,
            Type? resourceType = null,
            string conflictField = "id" )
        {
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

                _ => new GenericError(kcError?.Description ?? kcError?.Error ?? "Unexpected error from Keycloak")
            };
        }

        public static BasicError MapFor<TResource>(
            HttpStatusCode status,
            ErrorRepresentation kcError,
            Guid resourceId,
            string conflictField = "id" )
        => Map(status, kcError, resourceId, typeof(TResource), conflictField);
    }
}
