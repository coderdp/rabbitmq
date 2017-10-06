using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Publish
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
                    bool durable = true;//设置队列持久化
                    channel.QueueDeclare("hello", durable, false, false, null);
                    string message = GetMessage(args);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.DeliveryMode = 2;//设置消息持久化或者可以使用 properties.SetPersistent(true);
                    //todo需要注意的是，将消息设置为持久化并不能完全保证消息不丢失。虽然他告诉RabbitMQ将消息保存到磁盘上，但是在RabbitMQ接收到消息和将其保存到磁盘上这之间仍然有一个小的时间窗口。 RabbitMQ 可能只是将消息保存到了缓存中，并没有将其写入到磁盘上。持久化是不能够一定保证的，但是对于一个简单任务队列来说已经足够。如果需要消息队列持久化的强保证，可以使用publisher confirms
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("", "hello", properties, body);

                    Console.WriteLine(" send {0}", message);
                }
            }
            Console.ReadKey();
        }
        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}
