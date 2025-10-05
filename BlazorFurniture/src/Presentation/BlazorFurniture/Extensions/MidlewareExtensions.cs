using BlazorFurniture.Middlewares;

namespace BlazorFurniture.Extensions;

public static class MidlewareExtensions
{
    extension( IApplicationBuilder applicationBuilder )
    {
        public void UseGlobalExceptionHandler()
        {
            applicationBuilder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
