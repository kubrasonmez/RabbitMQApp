using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Consumer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace RabbitMQ.Consumer
{
    class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("CloudAMQPUrl");// Cloud AMQP Url yazılır.

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //channel.ExchangeDeclare(exchange: "logs", durable: true, type: ExchangeType.Fanout);
                    //channel.ExchangeDeclare(exchange: "direct-exchange", durable: true, type: ExchangeType.Direct);
                    //channel.ExchangeDeclare(exchange: "topic-exchange", durable: true, type: ExchangeType.Topic);
                    channel.ExchangeDeclare(exchange: "header-exchange", durable: true, type: ExchangeType.Headers);

                    //var queueName = channel.QueueDeclare().QueueName;
                    ////Exchange'e kuyruğu bağlamış olduk.
                    //string routingKey = "#.Warning";

                    //channel.QueueBind(queue: queueName, exchange: "topic-exchange", routingKey: routingKey);

                    ////channel.QueueDeclare("task_queue", durable: true, exclusive: false, autoDelete: false, null);
                    //// tek bir seferde bir mesaj alabilir. Global : false şuanda create edilen instance'ı sadece ilgilendiriyor anlamına gelir. True olsaydı yaratılan tüm instanceler için bu kural tanımlı olacaktı. 1 ise tek seferde kaç tane kuyruktan mesaj alabileceğimizi set etmemizi sağlar.
                    //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, false);

                    //Console.WriteLine("Waiting critical and error logs....");

                    //var consumer = new EventingBasicConsumer(channel);
                    ////autoAck:false mesajın başarılı bir şekilde işlendiği bilgisini (ACK) otomatik gönderme, business'a bağlı olarak gönderilecek. ACK gönderildiğinde mesaj kuyruktan silinecek.
                    //channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                    channel.QueueDeclare("queue1", false, false, false, null);

                    Dictionary<string, object> headers = new Dictionary<string, object>();

                    headers.Add("format", "pdf");
                    headers.Add("shape", "a4");
                    headers.Add("x-match", "any");

                    channel.QueueBind("queue1", "header-exchange", string.Empty, headers);

                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume("queue1", false, consumer);
                    consumer.Received += (model, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var user = JsonConvert.DeserializeObject<User>(message);

                        Console.WriteLine($"Message has been received : {user.Id} - {user.Name} - {user.Email} - {user.Password}");

                        //business işlemimiz düzgün çalışmazsa bu komut çalışmayacağından kuyruktaki mesaj da silinmeyecek.
                        channel.BasicAck(ea.DeliveryTag, multiple: false);
                    };
                    Console.WriteLine("Click to exit..");
                    Console.ReadLine();
                }
            }
        }
    }
}
