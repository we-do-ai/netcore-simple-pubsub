# netcore-simple-pubsub
Simple PubSub Project to push and receive events in a distributed cloud enviroment

## Installation
clone or via Nuget: `ai.we-do.pubsub`

## Usage
The project is based on two concepts:
### 1. Event hubs
... you can push messages to and that are also responsible for dispatching incomming messages

The `EventHub`class should be derived for your own usage so that you can easily create multiple hubs:
```csharp
public class GlobalEventHub : EventHub
{
  public GlobalEventHub(IEventConnection connection) : base(connection)
  {
  }

  protected override Func<Task> Dispatch(EventItem eventItem)
  {
    // Dispatch the event right here
    return () => Task.CompletedTask;
  }
```

if you need dedicated channels in your connection pass the channel name as string in the constructor and call `base(connection, channelName)`

### 2. Connections
... that provide pub sub functionalities for different infrastructure components. Already included is a Redis and a Azure Storage Bus Connection
The `IEventConnection` interface contains two methods: `Subscribe` and `PublishNewMessageAsync`


## Configuration
1. Add the following in your Startup.cs:
```csharp
services.AddSingleton<IEventConnection, RedisEventConnection>();
services.AddSingleton<GlobalEventHub>();
```

be aware that the GlobalEventHub gets called when it is injected by another component and the subscription is not active until it is first used. So maybe you want to create the Eventhub manually or inject it in `public void Configure` in your Startup.cs

2. add connection strings to your appSettings.json:

Redis example:
```json
"ConnectionStrings": {
  "EventHubRedisCache": "yourserver.domain.tld:6380,password=yourfancypassword,ssl=True,abortConnect=False"
},
```

Azure Storage Bus Example
```json
"ConnectionStrings": {
  "EventHubAzureStorageBus": "Endpoint=sb://yourservicebusname.servicebus.windows.net/;SharedAccessKeyName=YourSharedAccessKey;SharedAccessKey=YourSharedAccessKey"
},
```

