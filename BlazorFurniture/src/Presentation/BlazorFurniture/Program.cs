using BlazorFurniture.Client.Services;
using BlazorFurniture.Client.Services.Interfaces;
using BlazorFurniture.Components;
using BlazorFurniture.Core.Shared.Configurations;
using BlazorFurniture.Extensions;
using BlazorFurniture.Extensions.Handlers;
using BlazorFurniture.Extensions.ServiceCollection;
using BlazorFurniture.ServiceDefaults;
using BlazorFurniture.Shared.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MudBlazor.Services;
using NSwag;
using Scalar.AspNetCore;
using System.Globalization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;
});

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ForwardAuthHeaderHandler>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddApiClients()
    .ConfigureHttpClient(( serviceProvider, c ) =>
    {
        // TODO: verify if this works correctly when calling different apis 
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

        if (httpContextAccessor is not null)
        {
            var request = httpContextAccessor.HttpContext?.Request;
            c.BaseAddress = new Uri($"{request?.Scheme}://{request?.Host}");
        }
    })
    .AddHttpMessageHandler<ForwardAuthHeaderHandler>();

builder.Services.AddScoped<IPermissionsService, PermissionsService>();
builder.Services.AddSingleton<IThemeService, ThemeService>();
builder.Services.AddScoped<ISearchService, SearchService>();

// Get OIDC configuration for Swagger setup
var openIdConnectOptions = builder.Configuration.GetSection(OpenIdConnectConfigOptions.NAME).Get<OpenIdConnectConfigOptions>()
    ?? throw new Exception($"Missing {nameof(OpenIdConnectConfigOptions)} settings");

var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
    $"{openIdConnectOptions.Authority}/.well-known/openid-configuration",
    new OpenIdConnectConfigurationRetriever(),
    new HttpDocumentRetriever { RequireHttps = false });

var oidcConfiguration = await configManager.GetConfigurationAsync();

builder.Services.AddFastEndpoints()
.SwaggerDocument(options =>
{
    options.ShortSchemaNames = true;
    options.EnableJWTBearerAuth = false;
    options.FlattenSchema = true;
    options.DocumentSettings = o =>
    {
        o.Title = "BlazorFurniture";
        o.Version = "v1";
        o.Description = "BlazorFurniture API with OAuth2/OIDC authentication";

        // Add OAuth2 security scheme with Authorization Code Flow + PKCE
        o.AddAuth(JwtBearerDefaults.AuthenticationScheme, new NSwag.OpenApiSecurityScheme()
        {
            Type = OpenApiSecuritySchemeType.OAuth2,
            Description = "OAuth2 Authorization Code Flow with PKCE",
            Flows = new NSwag.OpenApiOAuthFlows()
            {
                AuthorizationCode = new NSwag.OpenApiOAuthFlow()
                {
                    AuthorizationUrl = oidcConfiguration.AuthorizationEndpoint,
                    TokenUrl = oidcConfiguration.TokenEndpoint,
                    RefreshUrl = oidcConfiguration.TokenEndpoint,
                    Scopes = openIdConnectOptions.DevPublicClient?.Scopes.ToDictionary(s => s, s => s) ?? [],
                },
            }
        });
    };
    options.TagCase = TagCase.LowerCase;
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
//builder.Services.AddOpenApi(options =>
//{
//    options.AddDocumentTransformer<OAuthSecurityTransformer>();
//    options.AddScalarTransformers();
//});

builder.Services.AddAppServices(builder.Configuration);
//builder.Services.AddSerilog();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ro") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en"),
    SupportedCultures = [.. supportedCultures],
    SupportedUICultures = [.. supportedCultures]
};
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async ( context, next ) =>
{
    var cultureCookie = context.Request.Cookies["BlazorCulture"];

    if (!string.IsNullOrEmpty(cultureCookie))
    {
        var culture = new CultureInfo(cultureCookie);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    await next();
});

//app.UseHttpsRedirection();
app.UseCors();
app.UseGlobalExceptionHandler();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(options =>
{
    options.Endpoints.RoutePrefix = "api";
    options.Endpoints.ShortNames = true;
    options.Errors.UseProblemDetails();
})
.UseSwaggerGen(uiConfig: c =>
{
    c.Path = "/swagger";
    c.DocumentPath = "/swagger/{documentName}/swagger.json";

    // Configure Swagger UI OAuth2 with PKCE
    c.ConfigureDefaults(s =>
    {
        s.OAuth2Client = new NSwag.AspNetCore.OAuth2ClientSettings
        {
            ClientId = openIdConnectOptions.DevPublicClient?.ClientId,
            UsePkceWithAuthorizationCodeGrant = true // Enable PKCE for Swagger UI
        };

        // Add scopes to the OAuth2Client
        foreach (var scope in openIdConnectOptions.DevPublicClient?.Scopes ?? [])
        {
            s.OAuth2Client.Scopes.Add(scope);
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");

        // Configure OAuth2 Authorization Code Flow with PKCE
        options.AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme)
            .AddAuthorizationCodeFlow(JwtBearerDefaults.AuthenticationScheme, flow =>
            {
                flow.Pkce = Pkce.Sha256;
                flow.ClientId = openIdConnectOptions.DevPublicClient?.ClientId;
                flow.SelectedScopes = openIdConnectOptions.DevPublicClient?.Scopes;
                flow.AuthorizationUrl = oidcConfiguration.AuthorizationEndpoint;
                flow.TokenUrl = oidcConfiguration.TokenEndpoint;
                flow.RefreshUrl = oidcConfiguration.TokenEndpoint;
            });

        options.PersistentAuthentication = true;
        options.DefaultOpenAllTags = true;
    });
}
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorFurniture.Client._Imports).Assembly);

app.Run();
