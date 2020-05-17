using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Publisher.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Publisher
{
    class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("CloudAMQPUrl");// Cloud AMQP Url yazılır.
            //factory.HostName = "localhost";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //channel.QueueDeclare("task_queue", durable: true, false, false, null);//durable true olması sunucu restart olursa mesaj kuyrukta kaydedilecek
                    //channel.ExchangeDeclare(exchange: "fanout-exchange", durable: true, type: ExchangeType.Fanout);
                    //channel.ExchangeDeclare(exchange: "direct-exchange", durable: true, type: ExchangeType.Direct);
                    //channel.ExchangeDeclare(exchange: "topic-exchange", durable: true, type: ExchangeType.Topic);
                    channel.ExchangeDeclare(exchange: "header-exchange", durable: true, type: ExchangeType.Headers);
                    
                    var properties = channel.CreateBasicProperties();

                    Dictionary<string, object> headers = new Dictionary<string, object>();

                    headers.Add("format5", "pdf");
                    headers.Add("shape", "a4");

                    properties.Headers = headers;

                    User user = new User()
                    {
                        Id = 1,
                        Name = "Kubra",
                        Email = "a@a.com",
                        Password = "1111"
                    };

                    var message = JsonConvert.SerializeObject(user);

                    channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(message));
                }
                Console.WriteLine("Click to exit..");
                Console.ReadLine();
            }     
        }
    }
}
