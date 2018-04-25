<p align="center">
  <h1 align="center">
    <code class="rich-diff-level-one">&lt;T&gt;</code>
    <br/>
    Generic Protocol
  </h1>

  <blockquote align="center">⚡️ A fast TCP event based buffered server/client protocol for transferring data over the (inter)net in .NET 🌐</blockquote>

  <p align="center">
	<a href="https://docs.microsoft.com/en-us/dotnet/standard/net-standard">
		<img src="https://img.shields.io/badge/.NET-Standard-lightgrey.svg">
	</a>
	<a href="https://www.codacy.com/app/mrousavy/GenericProtocol?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=mrousavy/GenericProtocol&amp;utm_campaign=Badge_Grade">
		<img src="https://api.codacy.com/project/badge/Grade/6850ab93bbb445eaa7902450addfab6b">
	</a>
    <a href="https://ci.appveyor.com/project/mrousavy/genericprotocol">
      <img src="https://ci.appveyor.com/api/projects/status/vlgt97f4bpgci6pj?svg=true">
    </a>
	<a href="https://www.nuget.org/packages/GenericProtocol"/>
		<img src="https://img.shields.io/nuget/v/GenericProtocol.svg">
	</a>
	<a href="https://www.nuget.org/packages/GenericProtocol/">
		<img src="https://img.shields.io/nuget/dt/GenericProtocol.svg">
	</a>
  </p>
  <p align="center">	
    <a href='https://ko-fi.com/F1F8CLXG' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://az743702.vo.msecnd.net/cdn/kofi2.png?v=0' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>
  </p>
<p/>

## Why?
> Send whole objects over the net easier and faster

1. Send nearly **any** .NET `object`
(See: [supported](https://github.com/neuecc/ZeroFormatter#built-in-support-types), [custom](https://github.com/neuecc/ZeroFormatter#quick-start)) :package:
2. Send/Receive **faster** by **buffered send/receive** and **ZeroFormatter**'s **fast (de-)serializing** :dash:
3. Automatically **correct errors** with **TCP** and **Auto-reconnect** :white_check_mark:
4. **Async** and **Event** based :zap:
5. **Efficient Network Discovery** for other **GenericProtocol Hosts** :mag:
6. **Fast binary links** for file/images/.. transfer :floppy_disk:
7. Made with love :heart:

Sending objects:
```csharp
await client.Send(someObject);
```

...on the other end:
```csharp
private void MyMessageReceivedCallback(SomeObject someObject) {
  Console.WriteLine("I've received an object!");
}
```

## How?
Add **GenericProtocol** to your existing **.NET**/**.NET Core 2.0+**/**.NET Standard 2.0+** Project via **NuGet**:
```
PM> Install-Package GenericProtocol
```

Use the default namespace:
```csharp
using GenericProtocol;
```

Are you [connecting to a server](#client), or **are you** [the server](#server)?


## Client
### Connect to a [server](#server):
```csharp
IClient client = await Factory.StartNewClient<MessageObject>("82.205.121.132", 1024, true);
```
The Factory will **construct and connect** a new `IClient<T>` object, where `<T>` is the object
you want to **send over the net** (Here: `MessageObject`). This can be ([supported](https://github.com/neuecc/ZeroFormatter#built-in-support-types))
**built in types** (`string`, `IEnumerable`, ..) or **custom types** marked with `[ZeroFormattable]` (see [here](https://github.com/neuecc/ZeroFormatter#quick-start))

### Send/Receive custom objects:
```csharp
// MessageObject.cs
[ZeroFormattable]
public struct MessageObject {
  [Index(0)]
  public string Sender { get; set; }
  [Index(1)]
  public string Recipient { get; set; }
  [Index(2)]
  public string Message { get; set; }
  [Index(3)]
  public DateTime Timestamp { get; set; }
}

// Main.cs
IClient client = await Factory.StartNewClient<MessageObject>("82.205.121.132", 1024);
// MyMessageReceivedCallback will be called whenever this client receives a message
client.ReceivedMessage += MyMessageReceivedCallback; // void MyCallback(IPEndPoint, MessageObject)

var msgObject = new MessageObject() {
  Sender = "mrousavy",
  Recipient = "cbarosch",
  Message = "Hi server!",
  Timestamp = DateTime.Now
}
await client.Send(msgObject);
// (Optionally configure your Server so that it should redirect to the Recipient)
client.Dispose();
```

### Send large binary content
```csharp
IClient client = await Factory.StartNewBinaryDownlink("82.205.121.132", 1024, true);
client.Send(bytes); // bytes can be a large file for example
client.Dispose();
```
Use `BinaryDownlinks`/`BinaryUplinks` when you just want to **send binary content** (Files, Images, ..). The binary links will skip the serialization and **send buffered right away**.

### Other
```csharp
// Automatically try to reconnect on disconnects
client.AutoReconnect = true;
// Set the reading buffer size for incoming data
client.ReceiveBufferSize = 2048;
// Set the writing buffer size for outgoing data
client.SendBufferSize = 2048;
// Get the current Connection status
var status = client.ConnectionStatus;
// Connection to server lost handler
client.ConnectionLost += ...;
```

## Server
### Start a new server:
```csharp
IServer server = await Factory.StartNewServer<MessageObject>(IPAddress.Any, 1024, true);
```

### Send/Receive your objects (`MessageObject` in this example):
```csharp
// Attach to the Message Received event
server.ReceivedMessage += MyMessageReceivedCallback; // void MyCallback(IPEndPoint, MessageObject)

var msgObject = new MessageObject() {
  Sender = "server",
  Recipient = "mrousavy",
  Message = "Hello client!",
  Timestamp = DateTime.Now
}
var clientEndPoint = server.Clients.First(); // Get first client in connected-clients enumerable
await server.Send(msgObject, clientEndPoint); // Send object to given client
```

### Other
```csharp
// Event once a client connects
server.ClientConnected += ...; // void ClientConnectedCallback(IPEndPoint)
// Event once a client disconnects
server.ClientDisconnected += ...; // void ClientDisconnectedCallback(IPEndPoint)
// Set the reading buffer size for incoming data
server.ReceiveBufferSize = 2048;
// Set the writing buffer size for outgoing data
server.SendBufferSize = 2048;
// Set the count of maximum clients to queue on simultanious connection attempts
server.MaxConnectionsBacklog = 8;
```

> License: [MIT](https://github.com/mrousavy/GenericProtocol/blob/master/LICENSE) | [Contributing](https://github.com/mrousavy/GenericProtocol/blob/master/CONTRIBUTING.md) | Thanks!
