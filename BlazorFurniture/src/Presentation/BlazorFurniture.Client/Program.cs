using BlazorFurniture.Client.Services;
using BlazorFurniture.Client.Services.API;
using BlazorFurniture.Client.Services.Interfaces;
using BlazorFurniture.Core.Shared.Constants;
using BlazorFurniture.Shared.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddLocalization();
builder.Services.AddTransient<AuthenticatedHttpClientHandler>();
builder.Services.AddApiClients(builder.HostEnvironment.BaseAddress)
    .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();
builder.Services.AddScoped<IPermissionsService, PermissionsService>();
builder.Services.AddSingleton<IThemeService, ThemeService>();
builder.Services.AddScoped<ISearchService, SearchService>();

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
