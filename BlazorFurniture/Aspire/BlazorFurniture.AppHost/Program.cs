using BlazorFurniture.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BlazorFurniture>("blazorfurniture");

builder.AddKeycloak();

builder.Build().Run();
