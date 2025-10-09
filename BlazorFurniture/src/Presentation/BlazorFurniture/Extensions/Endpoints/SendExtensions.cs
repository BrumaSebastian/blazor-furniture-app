using BlazorFurniture.Core.Shared.Errors;
using FastEndpoints;

namespace BlazorFurniture.Extensions.Endpoints;

public static class SendExtensions
{
    extension( IResponseSender sender )
    {
        public Task NotFoundAsync<TResponse>( TResponse responseDto ) where TResponse : NotFoundError
            => sender.HttpContext.Response.SendAsync(responseDto, 404);

        public Task ConflictAsync<TResponse>( TResponse responseDto ) where TResponse : ConflictError
            => sender.HttpContext.Response.SendAsync(responseDto, 409);
    }
}
