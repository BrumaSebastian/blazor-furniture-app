using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BlazorFurniture.Infrastructure.External;

internal sealed class HttpRequestMessageBuilder
{
    private readonly HttpRequestMessage requestMessage = new();
    private string relativePath;

    private HttpRequestMessageBuilder( HttpMethod httpMethod, string path )
    {
        requestMessage.Method = httpMethod;
        relativePath = path;
    }

    public static HttpRequestMessageBuilder Create( HttpMethod httpMethod, string path ) => new( httpMethod, path );

    public HttpRequestMessage Build()
    {
        requestMessage.RequestUri = new Uri(relativePath, UriKind.Relative);
        return requestMessage;
    }

    public HttpRequestMessageBuilder WithContent( object content )
    {
        requestMessage.Content = new StringContent(
                JsonSerializer.Serialize(content),
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
        relativePath = $"{relativePath}?{query}";

        return this;
    }

    public HttpRequestMessageBuilder WithQueryParams( IDictionary<string, string> queryParams )
    {
        var query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        relativePath = $"{relativePath}?{query}";

        return this;
    }

    public HttpRequestMessageBuilder WithAuthorization( AccessTokenResponse accessTokenResponse )
    {
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessTokenResponse.AccessToken);
        return this;
    }
}
