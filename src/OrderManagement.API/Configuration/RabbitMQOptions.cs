namespace OrderManagement.API.Configuration
{
    public class RabbitMQOptions
    {
        public const string RabbitMQ = "RabbitMQ";

        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "Qweasdzxc123";
    }
}
