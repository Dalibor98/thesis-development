using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace CBS.MessageBus
{
    public class MessageBus : IMessageBus
    {
        public async Task PublishMessage2(object message, string topic_queue_Name, string connectionString)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(topic_queue_Name);

            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding
                .UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}
