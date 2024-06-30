using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailDispatching.Models
{
    public class ConfiguracaoRabbitMQ
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string HostName { get; set; } = null!;
        public string VirtualHost { get; set; } = null!;
        public int Port { get; set; }
    }
}