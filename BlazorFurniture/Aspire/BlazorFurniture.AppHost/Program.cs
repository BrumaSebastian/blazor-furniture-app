using BlazorFurniture.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BlazorFurniture>("blazorfurniture")
    .WithUrlForEndpoint("https", e => e.DisplayText = "Base Url")
    .WithEndpoint(scheme: "https", name: "scalar")
    .WithUrlForEndpoint("scalar", e =>
    {
        e.Url += "/scalar/v1";
        e.DisplayText = "Scalar API Docs";
    });

builder.Resources.First(r => r.Name.Equals("blazorfurniture")).TryGetEndpoints(out var endpoints);
endpoints!.First(e => e.Name.Equals("scalar")).Port = endpoints!.First(e => e.Name.Equals("https")).Port;

builder.AddKeycloak();
builder.AddMaildev();

builder.Build().Run();
