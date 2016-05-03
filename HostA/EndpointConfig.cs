using System.Configuration;
using Autofac;
using log4net.Config;
using NServiceBus.Log4Net;
using NServiceBus.Logging;
using NServiceBus;

namespace HostA
{
    public class HostAEndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            XmlConfigurator.Configure();
            LogManager.Use<Log4NetFactory>();
            configuration.EndpointName("HostA");
            configuration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.UseSerialization<JsonSerializer>();
            configuration.UseTransport<RabbitMQTransport>();
        }
    }
}
