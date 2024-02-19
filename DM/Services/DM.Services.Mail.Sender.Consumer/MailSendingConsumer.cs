﻿using Jamq.Client.Abstractions.Consuming;
using Jamq.Client.Rabbit;
using Jamq.Client.Rabbit.Consuming;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DM.Services.Mail.Sender.Consumer;

internal class MailSendingConsumer : BackgroundService
{
    private readonly ILogger<MailSendingConsumer> logger;
    private readonly IConsumerBuilder consumerBuilder;
    private readonly RetryPolicy consumeRetryPolicy;

    public MailSendingConsumer(
        ILogger<MailSendingConsumer> logger,
        IConsumerBuilder consumerBuilder)
    {
        this.logger = logger;
        this.consumerBuilder = consumerBuilder;
        consumeRetryPolicy = Policy.Handle<Exception>().WaitAndRetry(5,
            attempt => TimeSpan.FromSeconds(1 << attempt),
            (exception, _) => logger.LogWarning(exception, "Could not subscribe to the queue"));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogDebug("[🚴] Starting mail sending consumer");

        var parameters = new RabbitConsumerParameters("dm.mail.sender", "dm.mail.sending", ProcessingOrder.Sequential)
        {
            ExchangeName = "dm.mail.sending",
            RoutingKeys = new[] { "#" },
            DeadLetterExchange = new RabbitConsumerParameters("dm.mail.sender", "dm.mail.unsent-dlq", ProcessingOrder.Sequential)
            {
                ExchangeName = "dm.mail.unsent",
                ExchangeType = ExchangeType.Fanout,
                AdditionalQueueArguments = new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "dm.mail.sending" },
                    { "x-message-ttl", 60000 }
                }
            }
        };
        var consumer = consumerBuilder.BuildRabbit<MailLetter, MailSendingProcessor>(parameters);
        consumeRetryPolicy.Execute(consumer.Subscribe);

        logger.LogDebug("[👂] Mail sending consumer is listening to {QueueName} queue", parameters.QueueName);
        return Task.CompletedTask;
    }
}