# service-bus-jobs

Service Bus-based Job System for .NET Core

## Usage

in your `Startup.cs`, in `ConfigureServices`:

```cs
services.AddJobSystem((options) =>
{
	options.ServiceBusName = Environment.GetEnvironmentVariable("SERVICE_BUS_NAME");
	options.ServiceBusUrl = Environment.GetEnvironmentVariable("SERVICE_BUS_URL");
	options.ServiceBusSAKPolicyName = Environment.GetEnvironmentVariable("SERVICE_BUS_SAK_POLICYNAME");
	options.ServiceBusSAKSharedSecret = Environment.GetEnvironmentVariable("SERVICE_BUS_SAK_SHAREDSECRET");
	options.EnableListener = true; // or false to not listen for any messages in this role
});
```
