using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorFurniture.Infrastructure.External;

internal sealed class HttpRequestMessageBuilder
{
    private readonly HttpRequestMessage requestMessage;
    private readonly UriBuilder uriBuilder;
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private HttpRequestMessageBuilder( HttpClient httpClient, HttpMethod httpMethod )
    {
        requestMessage = new HttpRequestMessage
        {
            Method = httpMethod
        };

        uriBuilder = new UriBuilder(httpClient.BaseAddress!);
    }

    public static HttpRequestMessageBuilder Create( HttpClient httpClient, HttpMethod httpMethod ) => new(httpClient, httpMethod);

    public HttpRequestMessage Build()
    {
        requestMessage.RequestUri = uriBuilder.Uri;

        return requestMessage;
    }

    public HttpRequestMessageBuilder WithPath( string path )
    {
        uriBuilder.Path = path;
        return this;
    }

    public HttpRequestMessageBuilder WithContent( object content, JsonSerializerOptions? serializerOptions = null )
    {
        requestMessage.Content = new StringContent(
                JsonSerializer.Serialize(content, serializerOptions ?? jsonSerializerOptions),
                System.Text.Encoding.UTF8,
                "application/json");

        return this;
    }

    public HttpRequestMessageBuilder WithFormParams( IDictionary<string, string> formParams )
    {
        requestMessage.Content = new FormUrlEncodedContent(formParams);

        return this;
    }

    public HttpRequestMessageBuilder WithQueryParams( IDictionary<string, string[]> queryParams )
    {
        var query = string.Join("&", queryParams.SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}={Uri.EscapeDataString(v)}")));
        uriBuilder.Query = query;

        return this;
    }

    public HttpRequestMessageBuilder WithQueryParams( IDictionary<string, string> queryParams )
    {
        var query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        uriBuilder.Query = query;

        return this;
    }

    public HttpRequestMessageBuilder WithAuthorization( AccessTokenResponse accessTokenResponse )
    {
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessTokenResponse.AccessToken);

        return this;
    }
}
