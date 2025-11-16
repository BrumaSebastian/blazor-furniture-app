using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorFurniture.Infrastructure.External;

internal sealed class HttpRequestMessageBuilder
{
    private readonly HttpRequestMessage requestMessage;
    private readonly UriBuilder uriBuilder;
    private readonly QueryBuilder queryBuilder;
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
        queryBuilder = [];
    }

    public static HttpRequestMessageBuilder Create( HttpClient httpClient, HttpMethod httpMethod ) => new(httpClient, httpMethod);

    public HttpRequestMessage Build()
    {
        if (queryBuilder.Any())
        {
            uriBuilder.Query = queryBuilder.ToQueryString().Value;
        }

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
                MediaTypeNames.Application.Json);

        return this;
    }

    public HttpRequestMessageBuilder WithFormParams( IDictionary<string, string> formParams )
    {
        requestMessage.Content = new FormUrlEncodedContent(formParams);

        return this;
    }

    public HttpRequestMessageBuilder AddQueryParam( string key, object? value )
    {
        if (value is null)
            return this;

        queryBuilder.Add(key, value.ToString()
            ?? throw new Exception($"There was a problem adding query param key:{key} value:{value}"));

        return this;
    }

    public HttpRequestMessageBuilder WithAuthorization( AccessTokenResponse accessTokenResponse )
    {
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessTokenResponse.AccessToken);

        return this;
    }

    public HttpRequestMessageBuilder WithAuthorization( string accessToken )
    {
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken);

        return this;
    }

    public HttpRequestMessageBuilder WithBearerAuthorization( string accessToken )
    {
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

        return this;
    }
}
