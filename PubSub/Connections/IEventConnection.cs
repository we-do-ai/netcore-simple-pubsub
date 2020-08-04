using System.Threading.Tasks;
using PubSub.Hubs;
using PubSub.Items;

namespace PubSub.Connections
{
    public interface IEventConnection
    {
        /// <summary>
        /// subscribe to updates from the infrastructure component
        /// </summary>
        /// <param name="hub">event hub that should be informed about new messages</param>
        /// <param name="channel">channel name for topic separation</param>
        void Subscribe(IEventHub hub, string channel);

        /// <summary>
        /// publish a message through the infrastructure to inform other server instances about anything
        /// </summary>
        /// <param name="serializedItem">item that should be pushed to all server instances</param>
        /// <param name="channel">channel that should receive the event</param>
        Task PublishNewMessageAsync(EventItem item, string channel);
    }
}