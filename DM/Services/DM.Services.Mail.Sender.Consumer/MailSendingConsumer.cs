﻿using Jamq.Client.Abstractions.Consuming;
using Jamq.Client.Rabbit.Consuming;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DM.Services.Mail.Sender.Consumer;

internal class MailSendingConsumer : BackgroundService
{
    private const string ConsumerExchangeName = "dm.mail.sending";
    private const string DeadLetterExchangeName = "dm.mail.unsent";

    private readonly ILogger<MailSendingConsumer> logger;
    private readonly IConsumerBuilder consumerBuilder;
    private readonly IAsyncConnectionFactory rabbitConnectionFactory;
    private readonly RetryPolicy consumeRetryPolicy;

    public MailSendingConsumer(
        ILogger<MailSendingConsumer> logger,
        IConsumerBuilder consumerBuilder,
        IAsyncConnectionFactory rabbitConnectionFactory)
    {
        this.logger = logger;
        this.consumerBuilder = consumerBuilder;
        this.rabbitConnectionFactory = rabbitConnectionFactory;

        consumeRetryPolicy = Policy.Handle<Exception>().WaitAndRetry(5,
            attempt => TimeSpan.FromSeconds(1 << attempt),
            (exception, _) => logger.LogWarning(exception, "Could not subscribe to the queue"));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogDebug("[🚴] Starting mail sending consumer");

        ConfigureDLX();

        var parameters = new RabbitConsumerParameters("dm.mail.sender", "dm.mail.sending", ProcessingOrder.Sequential)
        {
            ExchangeName = ConsumerExchangeName,
            RoutingKeys = new[] { "#" },
            DeadLetterExchange = DeadLetterExchangeName
        };
        var consumer = consumerBuilder.BuildRabbit<MailLetter, MailSendingProcessor>(parameters);
        consumeRetryPolicy.Execute(consumer.Subscribe);

        logger.LogDebug("[👂] Mail sending consumer is listening to {QueueName} queue", parameters.QueueName);
        return Task.CompletedTask;
    }

    private void ConfigureDLX()
    {
        var mailDLXQueue = $"{DeadLetterExchangeName}-dlq";
        var mailDLXRetryTimeoutInMs = 60000;

        using var configuringConnection = rabbitConnectionFactory.CreateConnection();
        using var channel = configuringConnection.CreateModel();

        channel.ExchangeDeclare(DeadLetterExchangeName, ExchangeType.Fanout, true);
        channel.QueueDeclare(mailDLXQueue, true, false, false, new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", ConsumerExchangeName },
            { "x-message-ttl", mailDLXRetryTimeoutInMs }
        });
        channel.QueueBind(mailDLXQueue, DeadLetterExchangeName, string.Empty);
    }
}