using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
    .AddCrossCuttingDependencies()
    .AddCoreDependencies();

await builder.Build().RunAsync();
