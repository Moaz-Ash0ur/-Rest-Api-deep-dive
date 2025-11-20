using Microsoft.AspNetCore.Mvc;
using SampleWebApi;
using SampleWebApi.Filters;
using SampleWebApi.Repos;
using Scalar.AspNetCore;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();

app.UseAuthorization();

app.UseExceptionHandler();

app.UseStatusCodePages();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
     
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Product API V1");
        options.SwaggerEndpoint("/openapi/v2.json", "Product API V2");

        options.EnableDeepLinking();// if have more one tag
        options.DisplayRequestDuration();
        options.EnableFilter();// enable filter by tag
    });

    app.MapScalarApiReference();
}

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    await AppDbContextInitializer.InitializeAsync(context);
//}

app.Run();


