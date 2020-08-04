using System;
using System.Threading.Tasks;
using PubSub.Connections;
using PubSub.Items;

namespace PubSub.Hubs
{
    public abstract class EventHub : IEventHub
    {
        private readonly IEventConnection _connection;
        
        public event EventHandler<EventItem> OnFcEvent;
        private readonly string _channel;

        protected EventHub(IEventConnection connection, string channel)
        {
            _channel = channel;
            _connection = connection;
            _connection.Subscribe(this, _channel);
            OnFcEvent += DispatchEvent;
        }
        
        public virtual async Task PushAsync(EventItem data)
        {
            await _connection.PublishNewMessageAsync(data, _channel);
        }

        protected abstract Func<Task> Dispatch(EventItem eventItem);

        private void DispatchEvent(object? sender, EventItem e)
        {
            Dispatch(e).Invoke();
        }
        
        public void SpreadEvent(EventItem item)
        {
            OnFcEvent?.Invoke(this, item);
        }
    }
}