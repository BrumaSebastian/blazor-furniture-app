using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.IntegrationTests.Controllers.Setup;

public class ProgramSut : AppFixture<Program>
{
    protected override void ConfigureApp( IWebHostBuilder a )
    {
        a.UseContentRoot(Directory.GetCurrentDirectory());
    }

    protected override void ConfigureServices( IServiceCollection serviceCollection )
    {
        // Override services for testing if needed
        // Example: Replace ICommandDispatcher with a mock or test implementation
    }
}
