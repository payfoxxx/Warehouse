using Application;
using Application.Mapping;
using AutoMapper;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
    
});

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(AutoMapperProfile).Assembly);
});

// builder.Services.AddMediatR(cfg =>
//     cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyReference).Assembly));

// builder.Services.Configure<JsonOptions>(options => {
//     options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
// });



builder.Services.AddApplication();
var dbConnection = builder.Configuration.GetConnectionString("dbConnection");
builder.Services.AddInfrastructure(dbConnection);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("ReactAppPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
