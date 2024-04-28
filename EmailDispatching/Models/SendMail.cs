using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailDispatching.Models
{
    public class SendMail
    {        
        public string Nome { get; set; } = null!;
        public string EnderecoDestino { get; set; } = null!;
        public string Corpo { get; set; } = null!;
    }
}