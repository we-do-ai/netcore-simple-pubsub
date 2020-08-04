using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PubSub.Extensions;

namespace PubSub.Items
{
    public class EventItem
    {
        // setters are important for deserialization
        /// <summary>
        /// Serialized Type of Item
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        ///  Serialized Item of Event
        /// </summary>
        public string Item { get; set; }
        /// <summary>
        ///  Serialized EventTopic
        /// </summary>
        public string EventTopic { get; set; }


        [JsonIgnore]
        public Type SystemType
        {
            get => System.Type.GetType(Type);
        }

        [JsonIgnore]
        public object Value
        {
            get => DeserializeItem();
        }

        /// <summary>
        /// Identifier to give some information about the audience that should receive the event.
        /// For example an SignalR Group name
        /// </summary>
        public string AudienceIdentifier { get; set; }

        public EventItem()
        {
        }

        public EventItem(Enum topic, object value, string audienceIdentifier)
        {
            EventTopic = topic.TryGetDescription(out var description) ? description : topic.ToString();

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            Type = value?.GetType().AssemblyQualifiedName ?? string.Empty;
            Item = JsonConvert.SerializeObject(value, jsonSerializerSettings);

            AudienceIdentifier = audienceIdentifier;
        }

        private object DeserializeItem()
        {
            var type = System.Type.GetType(Type);
            return JsonConvert.DeserializeObject(Item, type);
        }
    }
}