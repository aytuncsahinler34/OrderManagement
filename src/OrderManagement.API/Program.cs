using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.Interfaces;
using OrderManagement.Infrastructure.Data;
using OrderManagement.Infrastructure.Message;
using OrderManagement.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Order Management API",
        Version = "v1",
        Description = "Sipariş Yönetim Sistemi REST API"
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("OrderManagementDb"));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.Configure<RabbitMQOptions>(
    builder.Configuration.GetSection(RabbitMQOptions.RabbitMQ));

builder.Services.AddSingleton<IMessagePublisher>(sp =>
{
    var config = builder.Configuration.GetSection("RabbitMQ");

    var host = config.GetValue<string>("Host") ?? "localhost";
    var port = config.GetValue<int>("Port");
    var username = config.GetValue<string>("Username") ?? "guest";
    var password = config.GetValue<string>("Password") ?? "Qweasdzxc123";

    return new RabbitMQPublisher(host, port, username, password);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

