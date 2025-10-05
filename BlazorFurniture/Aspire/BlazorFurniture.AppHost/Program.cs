using BlazorFurniture.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak();
builder.AddMaildev();

builder.AddProject<Projects.BlazorFurniture>("blazorfurniture")
    .WithUrlForEndpoint("https", e => e.DisplayText = "Base Url")
    .WaitFor(keycloak);

builder.Resources.First(r => r.Name.Equals("blazorfurniture")).TryGetEndpoints(out var endpoints);
endpoints!.First(e => e.Name.Equals("https")).Transport = "https";

builder.Build().Run();
