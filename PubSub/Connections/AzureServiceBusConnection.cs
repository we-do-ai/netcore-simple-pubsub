using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PubSub.Hubs;
using PubSub.Items;

namespace PubSub.Connections
{
    public class AzureServiceBusConnection : IEventConnection
    {
        private static readonly Dictionary<string, IEventHub> EventHubs = new Dictionary<string, IEventHub>();
        private readonly string _connectionString;

        public AzureServiceBusConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("EventHubAzureStorageBus");
        }

        public void Subscribe(IEventHub hub, string queueName)
        {
            CreateClient(queueName).RegisterMessageHandler(
                async (Message message, CancellationToken cancellationToken) =>
                    await NewMessageHandler(message, queueName),
                ExceptionReceivedHandler);
            if (!EventHubs.ContainsKey(queueName)) EventHubs.Add(queueName, hub);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            arg.Exception.Data.Add("ExceptionReceivedContext", arg.ExceptionReceivedContext);
            throw arg.Exception;
        }

        private async Task NewMessageHandler(Message message, string queueName)
        {
            var json = Encoding.UTF8.GetString(message.Body);

            var item = JsonConvert.DeserializeObject<EventItem>(json,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            if (item is null) throw new NullReferenceException("event item is null");

            if (EventHubs.TryGetValue(queueName, out var hub))
            {
                hub.SpreadEvent(item);
            }
        }

        public Task PublishNewMessageAsync(EventItem item, string queueName)
        {
            return CreateClient(queueName).SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item))));
        }

        private QueueClient CreateClient(string queueName) =>
            new QueueClient(_connectionString, queueName);
    }
}