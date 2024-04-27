using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace EmailDispatching.Repositories.Contracts
{
    public interface IRabbitMQConnection
    {
        Task<string> ConsumerRabbitMQ(string queueName);
        Task<string> ProcessarEmails();
    }
}