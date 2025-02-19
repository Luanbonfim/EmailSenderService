namespace ProductEmailService
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public string QueueName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}