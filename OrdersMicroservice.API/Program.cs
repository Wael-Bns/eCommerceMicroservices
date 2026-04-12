using System.Text.Json.Serialization;
using OrdersMicroservice.API.Middlewares;
using OrdersMicroservice.Core;
using OrdersMicroservice.Core.HttpClients;
using OrdersMicroservice.Core.Policies;
using OrdersMicroservice.Infrastructure;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and configure JSON options to use string enums
builder.Services.AddControllers()
    .AddJsonOptions(
    options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

// Register application services and infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCore(builder.Configuration);

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS 
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddTransient<IUsersMicroservicePolicies,UsersMicroservicePolicies>();

// Add Http client
builder.Services.AddHttpClient<UsersMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"http://{builder.Configuration["UsersMicroserviceName"]}" +
        $":{builder.Configuration["UsersMicroservicePort"]}");
}).AddPolicyHandler(
    builder.Services.BuildServiceProvider()
    .GetRequiredService<IUsersMicroservicePolicies>().GetRetryPolicy()
    ).AddPolicyHandler(
    builder.Services.BuildServiceProvider()
    .GetRequiredService<IUsersMicroservicePolicies>().GetCircuitBreakerPolicy()
);
builder.Services.AddHttpClient<ProductsMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"http://{builder.Configuration["ProductsMicroserviceName"]}" +
        $":{builder.Configuration["ProductsMicroservicePort"]}");
});


var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.MapControllers();

app.UseRouting();

//Cors
app.UseCors();

//Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

//Auth
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();