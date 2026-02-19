using CMSBlog.API;
using CMSBlog.API.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IConfiguration configuration = builder.Configuration;
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Configuration Services extensions
builder.Services.ConfigureCors();
builder.Services.ConfigureSqlContext(configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureAutoMapper();

// For API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomOperationIds(apiDesc =>
    {
        return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
    });
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "CMSBlog API for Administrator",
        Description = "An ASP.NET Core Web API for managing CMSBlog content.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "CMSBlog Team",
            Email = "cms@gmail.com"
        },
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CMSBlog API v1");
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Migrate database and seed data
app.MigrateDatabase();

app.Run();
