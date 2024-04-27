using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace EmailDispatching.Repositories.Contracts
{
    public interface IRabbitMQConnection
    {
        public Task<string> RabbitMQ(string queueName);
    }
}