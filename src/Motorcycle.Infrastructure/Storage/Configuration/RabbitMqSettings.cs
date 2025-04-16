namespace Motorcycle.Infrastructure.Messaging.Configuration;

public class RabbitMqSettings
{
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    public int Port { get; set; } = 5672;
}