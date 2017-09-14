<div align="center">
  <h1 align="center">
    <code class="rich-diff-level-one">&lt;T&gt;</code>
    <br/>
    Generic Protocol
  </h1>

  <blockquote align="center">A fast TCP event based buffered server/client protocol for transferring data over the network in .NET Core/Classic</blockquote>

  <p align="center">
    <a href="https://ci.appveyor.com/project/mrousavy/genericprotocol">
      <img src="https://ci.appveyor.com/api/projects/status/vlgt97f4bpgci6pj?svg=true">
    </a>
  </p>
<div/>

## Usage
Add **GenericProtocol** to your existing **.NET**/**.NET Core 2.0+**/**.NET Standard 2.0+** Project:
```
PM> Install-Package GenericProtocol
```

Use the default namespace:
```csharp
using GenericProtocol;
```

Are you [connecting to a server](#client), or are you [a server](#server)?


## Client
Connect to a [server](#server):
```csharp
// 1. Factory
IClient client = await Factory.StartNewClient<string>("82.205.121.132", 1024, true);
// OR
// 2. Manually
IClient client = new ProtoClient<string>(IPAddress.Parse("82.205.121.132"), 1024)
await client.Connect(true);
```

Send/Receive simple strings:
```csharp
// Attach to the Message Received event
client.ReceivedMessage += MyMessageReceivedCallback; // void MyCallback(string)

// Send a message to the Server
await client.Send("Hello server!");
```

Send/Receive objects:
```csharp
// MessageObject.cs //
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

// Main.cs //
IClient client = await Factory.StartNewClient<MessageObject>("82.205.121.132", 1024);
// Attach to the Message Received event
client.ReceivedMessage += MyMessageReceivedCallback; // void MyCallback(MessageObject)

// Send a message to the Server
var msgObject = new MessageObject() {
  Sender = "mrousavy",
  Recipient = "cbarosch",
  Message = "Hi!",
  Timestamp = DateTime.Now
}
await client.Send(msgObject);
// (Configure your Server so that it should redirect to the Recipient)
```

## Server
[TODO]
