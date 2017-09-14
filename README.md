<p align="center">
  <h1 align="center">
    <code class="rich-diff-level-one">&lt;T&gt;</code>
    <br/>
    Generic Protocol
  </h1>

  <blockquote align="center">⚡️ A fast TCP event based buffered server/client protocol for transferring data over the (inter)net in .NET Core/Classic 🌐</blockquote>

  <p align="center">
    <a href="https://ci.appveyor.com/project/mrousavy/genericprotocol">
      <img src="https://ci.appveyor.com/api/projects/status/vlgt97f4bpgci6pj?svg=true">
    </a>
	<a href="https://www.nuget.org/packages/GenericProtocol"/>
		<img src="https://img.shields.io/nuget/v/GenericProtocol.svg">
	</a>
  </p>
<p/>

## Why?
> Sending whole objects over the net was never easier

1. Send **any** .NET `object` 
(See: [supported](https://github.com/neuecc/ZeroFormatter#built-in-support-types), [custom](https://github.com/neuecc/ZeroFormatter#quick-start)) :package:
2. Send/Receive **faster** by **buffered send/receive** and **ZeroFormatter**'s **fast (de-)serializing** :dash:
3. Automatically **correct errors** with **TCP** and **Auto-reconnect** :white_check_mark:
4. Event based :zap:
5. **Async**
6. Made with love :heart:

Sending objects:
```csharp
client.Send(someObject);
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
Connect to a [server](#server):
```csharp
IClient client = await Factory.StartNewClient<string>("82.205.121.132", 1024, true);
```
The Factory will construct and connect a new `IClient<T>` object, where `<T>` is the object
you want to send over the net. This can be ([supported](https://github.com/neuecc/ZeroFormatter#built-in-support-types))
built in types, or custom types marked with `[ZeroFormattable]` (see [here](https://github.com/neuecc/ZeroFormatter#quick-start))

Send/Receive your objects (`string` in this example):
```csharp
// Attach to the Message Received event
client.ReceivedMessage += MyMessageReceivedCallback; // void MyCallback(string)

// Send a message to the Server
await client.Send("Hello server!");
```

Send/Receive custom objects:
```csharp
//////////////////////
// MessageObject.cs //
//////////////////////
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

//////////////////////
//     Main.cs      //
//////////////////////
IClient client = await Factory.StartNewClient<MessageObject>("82.205.121.132", 1024);
client.ReceivedMessage += MyMessageReceivedCallback; // void MyCallback(MessageObject)

var msgObject = new MessageObject() {
  Sender = "mrousavy",
  Recipient = "cbarosch",
  Message = "Hi!",
  Timestamp = DateTime.Now
}
await client.Send(msgObject);
// (Optionally configure your Server so that it should redirect to the Recipient)
```

## Server
[TODO]
