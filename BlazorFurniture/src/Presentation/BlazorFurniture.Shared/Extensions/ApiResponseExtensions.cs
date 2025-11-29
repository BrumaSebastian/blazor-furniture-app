using BlazorFurniture.Shared.Models;
using Refit;
using System.Text.Json;

namespace BlazorFurniture.Shared.Extensions;

public static class ApiResponseExtensions
{
    extension<T>( IApiResponse<T> response )
    {
        public ApiResult<T> ToApiResult()
        {
            if (response.IsSuccessStatusCode)
            {
                return new ApiResult<T>
                {
                    IsSuccess = true,
                    Data = response.Content,
                    HttpStatusCode = response.StatusCode
                };
            }

            // Try to parse RFC 7807 problem details if server returns them
            string? message = null;
            var stringContent = response.Content?.ToString();

            if (!string.IsNullOrWhiteSpace(stringContent))
            {
                try
                {
                    var problemDetails =
                        JsonSerializer.Deserialize<ProblemDetails>(stringContent);

                    message = problemDetails?.Detail
                              ?? problemDetails?.Title
                              ?? stringContent;
                }
                catch
                {
                    message = stringContent;
                }
            }

            return new ApiResult<T>
            {
                IsSuccess = false,
                HttpStatusCode = response.StatusCode,
                ErrorMessage = message ?? response.Error?.Message ?? "Unknown error"
            };
        }
    }
}

