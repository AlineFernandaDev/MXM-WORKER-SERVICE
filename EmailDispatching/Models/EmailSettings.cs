using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailDispatching.Models
{
    public class EmailSettings
    {
        public string ServerSmtp { get; set; } = null!;
        public int Port { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool EnableSsl { get; set; }
        public string EmailFromAddress { get; set; } = null!;
    }
}