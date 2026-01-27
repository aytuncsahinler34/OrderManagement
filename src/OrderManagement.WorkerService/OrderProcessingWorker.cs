using System.Text;
using Newtonsoft.Json;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Enums;
using OrderManagement.Core.Interfaces;
using OrderManagement.WorkerService.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderManagement.WorkerService;

public class OrderProcessingWorker : BackgroundService
{
    private readonly ILogger<OrderProcessingWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private IConnection? _connection;
    private IModel? _channel;
    private const string ORDER_QUEUE = "order-queue";
    private readonly RabbitMQOptions _options;

    public OrderProcessingWorker(
        ILogger<OrderProcessingWorker> logger,
        IServiceProvider serviceProvider,
        RabbitMQOptions options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        InitializeRabbitMQ();
        _options = options;
    }

    private void InitializeRabbitMQ()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: ORDER_QUEUE,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            _logger.LogInformation("RabbitMQ bağlantısı başarılı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ bağlantısı kurulamadı!");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
        {
            _logger.LogError("RabbitMQ kanalı başlatılamadı!");
            return;
        }

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var order = JsonConvert.DeserializeObject<Order>(message);

                if (order != null)
                {
                    _logger.LogInformation("Sipariş işleniyor: {OrderId}", order.Id);

                    await ProcessOrderAsync(order);

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    _logger.LogInformation("Sipariş başarıyla işlendi: {OrderId}", order.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş işlenirken hata oluştu: {Message}", message);
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _channel.BasicConsume(
            queue: ORDER_QUEUE,
            autoAck: false,
            consumer: consumer
        );

        _logger.LogInformation("Worker servis başlatıldı, siparişler dinleniyor...");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessOrderAsync(Order order)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        // Simüle edilmiş işlem süresi
        await Task.Delay(TimeSpan.FromSeconds(2));

        var existingOrder = await repository.GetByIdAsync(order.Id);

        if (existingOrder != null)
        {
            existingOrder.Status = OrderStatus.Processing;
            await repository.UpdateAsync(existingOrder);

            await Task.Delay(TimeSpan.FromSeconds(1));

            existingOrder.Status = OrderStatus.Completed;
            await repository.UpdateAsync(existingOrder);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}

