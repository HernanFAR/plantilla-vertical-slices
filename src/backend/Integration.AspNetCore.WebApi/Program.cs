var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDomainDependencies()
    .AddCoreDependencies()
    .AddCrossCuttingDependencies(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.UseAuthentication();

app.UseEndpointDefinitions();

app.Run();
