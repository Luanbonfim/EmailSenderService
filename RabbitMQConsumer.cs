using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Mail;
using System.Text;

namespace ProductEmailService
{
    public class RabbitMQConsumer
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly EmailSettings _emailSettings;

        public RabbitMQConsumer(
            ILogger<RabbitMQConsumer> logger,
            IOptions<RabbitMQSettings> rabbitMQSettings,
            IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _rabbitMQSettings = rabbitMQSettings.Value;
            _emailSettings = emailSettings.Value;
        }

        public void StartConsuming(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMQSettings.HostName,
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _rabbitMQSettings.QueueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation("Received message: {0}", message);

                    // Construct formal subject and body
                    string subject = "New Product Created – Notification";
                    string formattedBody = $@"
                                                Dear Customer,

                                                We are pleased to inform you that a new product has been created: {message}. 

                                                We hope this addition brings value to your experience with us. Should you have any questions, please do not hesitate to contact us.

                                                Best regards,
                                                Your Company Name";

                    // Send the email with the formal subject and body
                    SendEmail(_emailSettings.ToEmail, subject, formattedBody);

                    //WriteTextFile(_emailSettings.ToEmail, formattedBody);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: _rabbitMQSettings.QueueName,
                                     autoAck: false,
                                     consumer: consumer);

                _logger.LogInformation("Waiting for messages...");

                while (!stoppingToken.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void SendEmail(string to, string subject, string body)
        {
            try
            {
                using (var mailMessage = new MailMessage())
                using (var smtpClient = new SmtpClient(_emailSettings.SmtpHost))
                {
                    mailMessage.From = new MailAddress(_emailSettings.FromEmail);
                    mailMessage.To.Add(to);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    smtpClient.Port = _emailSettings.SmtpPort;
                    smtpClient.Credentials = new System.Net.NetworkCredential(
                        _emailSettings.UserName,
                        _emailSettings.Password);
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mailMessage);
                    _logger.LogInformation("Email sent successfully to {0}", to);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send email: {0}", ex.Message);
            }
        }

        private void WriteTextFile(string fileName, string text)
        {
            var solutionRoot = Directory.GetCurrentDirectory();

            var filePath = Path.Combine(solutionRoot, fileName) + ".txt";

            File.WriteAllText(filePath, text);
        }
    }
}