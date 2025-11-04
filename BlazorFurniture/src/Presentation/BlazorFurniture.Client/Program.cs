using BlazorFurniture.Client.Extensions;
using BlazorFurniture.Client.Services;
using BlazorFurniture.Client.Services.API;
using BlazorFurniture.Client.Services.Interfaces;
using BlazorFurniture.Core.Shared.Constants;
using BlazorFurniture.Shared.Extensions;
using BlazorFurniture.Shared.Security.Authorization;
using BlazorFurniture.Shared.Services.Security;
using BlazorFurniture.Shared.Services.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddLocalization();
builder.Services.AddTransient<AuthenticatedHttpClientHandler>();
builder.Services.AddServerApis(builder.HostEnvironment.BaseAddress);
builder.Services.AddScoped<IPermissionsService, PermissionsService>();
builder.Services.AddSingleton<IThemeService, ThemeService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IBreadCrumbsService, BreadcrumbsService>();

var host = builder.Build();

const string defaultCulture = Cultures.ENGLISH;
var defaultCultureInfo = new CultureInfo(defaultCulture);
CultureInfo.DefaultThreadCurrentCulture = defaultCultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = defaultCultureInfo;

var js = host.Services.GetRequiredService<IJSRuntime>();
var result = await js.InvokeAsync<string>("blazorCulture.get");
var culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);

if (result is null)
{
    await js.InvokeVoidAsync("blazorCulture.set", defaultCulture);
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
