using System;
using System.Threading.Tasks;
using PubSub.Items;

namespace PubSub.Hubs
{
    public interface IEventHub
    {
        /// <summary>
        /// Push event to all server instances 
        /// </summary>
        /// <param name="data"></param>
        Task PushAsync(EventItem data);

        /// <summary>
        /// event handler any component can subscribe to, to receive pushed messages
        /// </summary>
        event EventHandler<EventItem> OnFcEvent;

        /// <summary>
        /// spread an event through the event handler  
        /// </summary>
        /// <param name="item"></param>
        void SpreadEvent(EventItem item);
    }
}