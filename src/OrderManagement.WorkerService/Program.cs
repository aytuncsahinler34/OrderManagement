using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.Interfaces;
using OrderManagement.Infrastructure.Data;
using OrderManagement.WorkerService;

var builder = Host.CreateApplicationBuilder(args);

// Database - InMemory (API ile ayný instance'ý kullanmak için)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("OrderManagementDb"));

// Repository
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Worker Service
builder.Services.AddHostedService<OrderProcessingWorker>();

var host = builder.Build();
host.Run();