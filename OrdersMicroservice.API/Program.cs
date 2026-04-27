using System.Text.Json.Serialization;
using OrdersMicroservice.API.Middlewares;
using OrdersMicroservice.Core;
using OrdersMicroservice.Core.HttpClients;
using OrdersMicroservice.Core.Policies.PoliciesContracts;
using OrdersMicroservice.Core.Policies.PoliciesImplementations;
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

// Register policies for Http clients
builder.Services.AddTransient<IPollyPolicies, PollyPolicies>();
builder.Services.AddTransient<IUsersMicroservicePolicies,UsersMicroservicePolicies>();
builder.Services.AddTransient<IProductsMicroservicePolicies, ProductsMicroservicePolicies>();

// Add UsersMicroservice Http client
builder.Services.AddHttpClient<UsersMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"http://{builder.Configuration["UsersMicroserviceName"]}" +
        $":{builder.Configuration["UsersMicroservicePort"]}");
}).AddPolicyHandler(
    builder.Services.BuildServiceProvider()
    .GetRequiredService<IUsersMicroservicePolicies>().GetCombinedPolicy()
);
// Add ProductsMicroservice Http client
builder.Services.AddHttpClient<ProductsMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"http://{builder.Configuration["ProductsMicroserviceName"]}" +
        $":{builder.Configuration["ProductsMicroservicePort"]}");
}).AddPolicyHandler(
    builder.Services.BuildServiceProvider()
    .GetRequiredService<IProductsMicroservicePolicies>().GetCombinedPolicy()
    );


var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.MapControllers();

app.UseRouting();

//Cors
app.UseCors();

//Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.Run();