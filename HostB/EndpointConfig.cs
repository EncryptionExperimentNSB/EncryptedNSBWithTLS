using NServiceBus;
using NServiceBus.Log4Net;
using NServiceBus.Logging;
using log4net.Config;
using Models;

namespace HostB
{
    public class HostBEndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            XmlConfigurator.Configure();
            LogManager.Use<Log4NetFactory>();
            configuration.UseSerialization<JsonSerializer>();
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.EndpointName("HostB");

            configuration.UseTransport<RabbitMQTransport>();
            configuration.RegisterComponents(c => c.ConfigureComponent<MessageEncryptor>(DependencyLifecycle.InstancePerCall));

        }
    }
}
