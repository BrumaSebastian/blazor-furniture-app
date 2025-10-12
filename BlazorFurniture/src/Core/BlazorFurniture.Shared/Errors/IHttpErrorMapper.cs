using System.Net;

namespace BlazorFurniture.Core.Shared.Errors;

public interface IHttpErrorMapper
{
    BasicError MapFor<TError>( TError error, HttpStatusCode status, Guid? resourceId, Type? resourceType = null ) where TError : class;
}
