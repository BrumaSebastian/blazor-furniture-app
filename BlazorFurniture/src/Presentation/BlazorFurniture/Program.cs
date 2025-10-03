using BlazorFurniture.Components;
using BlazorFurniture.Core.Shared.Configurations;
using BlazorFurniture.Extensions.DocumentTransformers;
using BlazorFurniture.Extensions.ServiceCollection;
using BlazorFurniture.ServiceDefaults;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MudBlazor.Services;
using Scalar.AspNetCore;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;
});

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddFastEndpoints();
//builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<OAuthSecurityTransformer>();
});

builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddSerilog();
builder.Services.AddCascadingAuthenticationState();

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy
//            .AllowAnyHeader()
//            .AllowAnyMethod()
//            .AllowCredentials();
//    });
//});

var app = builder.Build();

app.MapOpenApi();
//app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    var openIdConnectOptions = app.Services.GetRequiredService<OpenIdConnectConfigOptions>();
    var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
        $"{openIdConnectOptions.Authority}/.well-known/openid-configuration",
        new OpenIdConnectConfigurationRetriever(),
        new HttpDocumentRetriever { RequireHttps = false });
    var config = await configManager.GetConfigurationAsync();

    app.MapScalarApiReference(options =>
    {
        var securityScheme = "OAuth2";

        options.AddPreferredSecuritySchemes(securityScheme)
            .AddAuthorizationCodeFlow(securityScheme, flow =>
            {
                flow.Pkce = Pkce.Sha256;
                flow.ClientId = openIdConnectOptions.DevPublicClient?.ClientId;
                flow.SelectedScopes = openIdConnectOptions.DevPublicClient?.Scopes;
                flow.AuthorizationUrl = config.AuthorizationEndpoint;
                flow.TokenUrl = config.TokenEndpoint;
            });
        options.PersistentAuthentication = true;
    })
    .RequireAuthorization(new AuthorizeAttribute
    {
        AuthenticationSchemes = string.Join(',', JwtBearerDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme),
    });
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
//app.UseCors();
app.UseAntiforgery();

app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(options =>
    {
        options.Endpoints.RoutePrefix = "api";
    });

//app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorFurniture.Client._Imports).Assembly);

app.Run();
