using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.UserName = "dp";
            factory.Password = "as1234";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);
                    channel.BasicQos(0, prefetchCount:1, global:false);//配置公平分发机制 设置prefetchCount : 1来告知RabbitMQ，在未收到消费端的消息确认时，不再分发消息，也就确保了当消费端处于忙碌状态时
                    var consumer = new QueueingBasicConsumer(channel);
                    bool autoAck = false;//设置是否自动ack 默认是true
                    channel.BasicConsume("hello", autoAck, consumer);

                    Console.WriteLine(" waiting for message.");
                    while (true)
                    {
                        var ea = consumer.Queue.Dequeue();
                        
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(dots * 1000);
                        Console.WriteLine("Received {0}", message);
                        channel.BasicAck(ea.DeliveryTag, false);//ack
                    }
                }
            }
        }
    }
}
Exchange