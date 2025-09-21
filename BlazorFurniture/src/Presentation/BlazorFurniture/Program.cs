using BlazorFurniture;
using BlazorFurniture.Components;
using BlazorFurniture.ServiceDefaults;
using FastEndpoints;
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
builder.Services.AddOpenApi();
builder.Services.AddSerilog();

builder.Services
    .AddAppAuthentication(builder.Configuration)
    .AddCascadingAuthenticationState()
    .AddAppAuthorization();

//builder.Services.AddCqrs();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("https://localhost:7128")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.MapOpenApi();
//app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    //TODO: understand how to use this
    app.MapScalarApiReference(options => options
        .AddPreferredSecuritySchemes("OAuth2")
        .AddAuthorizationCodeFlow("OAuth2", flow =>
        {
            flow.ClientId = "scalar-client";
            flow.Pkce = Pkce.Sha256;
            flow.SelectedScopes = ["read", "write", "admin"];
        }));
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
app.UseAntiforgery();

app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints();

//app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorFurniture.Client._Imports).Assembly);

app.Run();
