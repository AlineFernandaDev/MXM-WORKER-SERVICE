### Microsserviço de disparo de emails

Esse projeto foi proposto pela empresa MXM Sistemas, onde eu e mais dois colegas faríamos serviços que se complementassem (um formulário para envio de emails, uma API para pegar esses emails e alimentar uma fila utilizando o RabbitMQ e por fim o meu serviço que é obter esses emails através da fila e efetuar os disparos.


Ele pode ser utilizado por qualquer outro sistema que utilize filas com o RabbitMQ, basta apenas alterar as configurações de acesso (host, usuario, senha e etc)  no appsettings.json.
Sendo assim, pode ser usado localmente ou na web.


Para o envio dos emails é necessario informar também no appsettings.json as informações do seu provedor de email.
