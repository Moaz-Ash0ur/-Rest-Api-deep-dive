using SampleWebApi.CustomMiddleware;
using SampleWebApi.Repos;
using System.Configuration;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//builder.Services.AddControllers(options =>
//{
//    options.ReturnHttpNotAcceptable = true;//402 Format Not Supported
//})
//.AddXmlSerializerFormatters();

builder.Services.AddControllers()
    //.AddNewtonsoftJson(options =>
    //{
    //    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    //})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<ProductRepository>();  


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello from my Web API with Swagger!");

app.UseRequestLogging();

app.Run();
