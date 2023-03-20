# Avigilon Control Center (ACC) API for .NET

[![Nuget](https://img.shields.io/nuget/vpre/Gralin.Avigilon.ControlCenterAPI)](https://www.nuget.org/packages/Gralin.Avigilon.ControlCenterAPI)

## About

This library enables communication with the Avigilon Control Center (ACC) API from .NET. Please join [Avigilon Technology Partner Program](https://www.avigilon.com/partners/technology-partner-program#become-a-partner) prior to using it in your project. When you become Avigilon partner, in addition to support, you will be receive your unique set of `user_nonce` and `user_key` to be used by your integration. Only with this data will you be able to communicate with the ACC server instance.

## Features

* Login and get session
* Get camera list

Currently limited functionality is available but it's easy to extend (contributions welcome!)

## Usage

```csharp
var factory = new WebEndpointClientFactory("user_nonce", "user_key");

var client = factory.Create(new Uri("https://acc_address:8443"));

await client.Login("username", "password");

foreach (var camera in await client.GetCameras())
    Console.WriteLine(camera.Name);
```
