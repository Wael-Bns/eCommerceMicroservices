using System.Text.Json.Serialization;
using FluentValidation;
using ProductsService.API.ApiEndpoints;
using ProductsService.API.Middlewares;
using ProductsService.API.Validators;
using ProductsService.Core;
using ProductsService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Add FluentValidation validators to the DI container
builder.Services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();

// add a json serialized for string to enum conversion
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseExceptionHandlingMiddleware();

app.UseHttpsRedirection();

// adding the minimal api
app.UseCors();
app.AddProductMinimalApiEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
