using System;
using System.Threading.Tasks;
using DM.Services.Core.Configuration;
using DM.Services.MessageQueuing;
using DM.Services.MessageQueuing.Processing;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Polly;
using Polly.Retry;

namespace DM.Services.Mail.Sender.Consumer
{
    /// <inheritdoc />
    public class MailSendingProcessor : IMessageProcessor<MailLetter>
    {
        private readonly ILogger<MailSendingProcessor> logger;
        private readonly EmailConfiguration configuration;
        private readonly Lazy<IMailTransport> client;
        private readonly AsyncRetryPolicy retryPolicy;

        /// <inheritdoc />
        public MailSendingProcessor(
            IOptions<EmailConfiguration> configuration,
            ILogger<MailSendingProcessor> logger)
        {
            this.logger = logger;
            this.configuration = configuration.Value;
            client = new Lazy<IMailTransport>(() =>
            {
                var smtpClient = new SmtpClient();
                smtpClient.Connect(this.configuration.ServerHost, this.configuration.ServerPort,
                    SecureSocketOptions.StartTls);
                smtpClient.Authenticate(this.configuration.Username, this.configuration.Password);
                smtpClient.NoOp();
                return smtpClient;
            });
            retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(5,
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (exception, _, __) => logger.LogWarning(exception, "Something is wrong with mail sending"));
        }

        /// <inheritdoc />
        public async Task<ProcessResult> Process(MailLetter message)
        {
            logger.LogInformation($"Sending letter to {message.Address.Obfuscate()}");
            var policyResult = await retryPolicy.ExecuteAndCaptureAsync(() => client.Value.SendAsync(new MimeMessage
            {
                From = {new MailboxAddress(configuration.FromAddress)},
                ReplyTo = {new MailboxAddress(configuration.ReplyToAddress)},
                To = {new MailboxAddress(message.Address)},
                Subject = message.Subject,
                Body = new TextPart(TextFormat.Html) {Text = message.Body}
            }));
            return policyResult.Outcome == OutcomeType.Successful
                ? ProcessResult.Success
                : ProcessResult.Fail;
        }
    }
}