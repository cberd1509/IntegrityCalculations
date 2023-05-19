using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using WellIntegrityCalculations.Core;
using WellIntegrityCalculations.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ICalculationService,CalculationService>();
builder.Services.AddScoped<ICalculationRulesProvider, CalculationRulesProvider>();
builder.Services.AddScoped<IMawopDataProvider, MawopDataProvider>();
builder.Services.AddScoped<IMaaspDataProvider, MaaspDataProvider>();

// Custom Services

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Well Integrity Calculations API",
        Description = "An ASP.NET Core Web API for performing operative limits calculations",
        Contact = new OpenApiContact
        {
            Name = "Carlos Berdugo",
            Email = "carlos.berdugo2@halliburton.com"            
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.EnableAnnotations();

    options.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseReDoc(options =>
    {
        options.DocumentTitle = "API Documentation";
        options.SpecUrl = "/swagger/v1/swagger.json";
    });
}


app.UseAuthorization();

app.MapControllers();

app.Run();
