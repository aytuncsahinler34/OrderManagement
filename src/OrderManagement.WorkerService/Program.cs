using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.Interfaces;
using OrderManagement.Infrastructure.Data;
using OrderManagement.WorkerService;
using OrderManagement.WorkerService.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("OrderManagementDb"));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.Configure<RabbitMQOptions>(
    builder.Configuration.GetSection(RabbitMQOptions.RabbitMQ));

builder.Services.AddHostedService<OrderProcessingWorker>();

var host = builder.Build();
host.Run();