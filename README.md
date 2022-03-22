# Avigilon Control Center (ACC) API for .NET

## About

This library alows you to communicate with Avigilon Control Center (ACC) API from .NET. Before you can start using it, you need to send and email to integrations@avigilon.com and ask for your unique pair of user nonce and user key values. Only having those will you be able to communicate with ACC server instance.

## Features

* Login and get session
* Get camera list

Currently limited functionality is available but it's easy to extend (contributions welcome!)

## Usage

```csharp
var factory = new WebEndpointClientFactory("user_nonce", "user_key");
var client = factory.Create(new Uri("https://acc_address:8443"));
await client.Login("username", "password");
var cameras = await client.GetCameras();
```