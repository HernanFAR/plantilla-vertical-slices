var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.Local.json");
}   

builder.Services
    .AddDomainDependencies()
    .AddCoreDependencies()
    .AddCrossCuttingDependencies(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseAuthorization();
app.UseAuthentication();

app.UseEndpointDefinitions();

app.Run();
