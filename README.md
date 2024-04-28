## Serviço de Envio de E-mails com RabbitMQ
Este é um serviço para processamento e envio de e-mails utilizando RabbitMQ.

Pré-requisitos

.NET Core SDK 3.1 ou superior

RabbitMQ instalado e configurado

Conta de e-mail válida para envio de e-mails

## Configuração:

Clone o repositório para a sua máquina local.

Certifique-se de ter preenchido corretamente as configurações do RabbitMQ e das credenciais de e-mail no arquivo appsettings.json:


    
    "RabbitMQ": {
    "UserName": "seu_usuario_rabbitmq",
    "Password": "sua_senha_rabbitmq",
    "HostName": "localhost",
    "VirtualHost": "/",
    "Port": 5672
    }


Execute o comando dotnet build para compilar o projeto.

Execute o comando dotnet run para iniciar o serviço.

### Configurações do RabbitMQ e do E-mail

As configurações do RabbitMQ e do servidor de e-mail devem ser definidas no arquivo appsettings.json. Aqui está um exemplo de como as configurações devem ser definidas:

    
    
    "EmailSettings": {
    "EmailFromAddress": "seu_email@gmail.com",
    "ServerSmtp": "smtp.gmail.com",
    "Port": 587,
    "UserName": "seu_usuario_email",
    "Password": "sua_senha_email"
    }

Certifique-se de substituir os valores de exemplo pelos seus próprios valores de configuração.

Utilização
Este serviço utiliza RabbitMQ para consumir mensagens da fila "queueSendEmails" e envia e-mails com base nas informações recebidas.

Atente-se para a configuração da conexão, ela pode ser usada de várias formas.


    UserName = _configuration["RabbitMQ:UserName"],
    Password = _configuration["RabbitMQ:Password"],
    HostName = _configuration["RabbitMQ:HostName"],
    VirtualHost = _configuration["RabbitMQ:VirtualHost"],
    Port = Convert.ToInt32(_configuration["RabbitMQ:Port"]),

ou:
   
    
    HostName = {"localhost"}

Essa configuração se encontra na classe RabbitMQConection, dentro da pasta Services.

### Funcionamento em conjunto com API

Copie a pasta do serviço de emails para os arquivos de sua API, certifique-se de manter a estrutura e organização da mesma, faça também um back-up, navegue até a classe Program da API e adicione


        
    services.AddHostedService<RabbitMQWorker>();


#### Idealizadora

Aline Vieira

![IMG_20230717_143847736~2](https://github.com/ALM-MXM/ALM-WORKER-SERVICE/assets/127868361/3e414e5e-449e-4da6-a263-189f2dd50e18)
