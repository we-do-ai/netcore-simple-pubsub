using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PubSub.Hubs;
using PubSub.Items;
using StackExchange.Redis;

namespace PubSub.Connections
{
    public class RedisEventConnection : IEventConnection
    {
        private readonly ISubscriber _subscriber;

        private static readonly Dictionary<string, IEventHub> EventHubs = new Dictionary<string, IEventHub>();

        public RedisEventConnection(IConfiguration configuration)
        {
            var multiplexer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("EventHubRedisCache"));
            _subscriber = multiplexer.GetSubscriber();
        }
        
        public void Subscribe(IEventHub hub, string channel)
        {
            EventHubs[channel] = hub;
            
            _subscriber.Subscribe(channel, NewMessageHandler);
        }

        public async Task PublishNewMessageAsync(EventItem item, string channel)
        {
            await _subscriber.PublishAsync(channel, Serialize(item));
        }
        
        private void NewMessageHandler(RedisChannel channel, RedisValue value)
        {
            string json = value;
            var item = JsonConvert.DeserializeObject<EventItem>(json, new JsonSerializerSettings{ContractResolver = new CamelCasePropertyNamesContractResolver()});
            if(item is null) throw new NullReferenceException("event item is null");
            
            if(EventHubs.TryGetValue(channel.ToString(), out var hub))
            {
                hub.SpreadEvent(item);
            }
        }
        
        private static string Serialize(EventItem data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}