var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BlazorFurniture_Web>("blazorfurniture-web");

builder.Build().Run();
