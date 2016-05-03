using Models;
using NServiceBus;
using System;

namespace HostA
{
    public class Startup : IWantToRunWhenBusStartsAndStops
    {
        private readonly IBus _bus;

        public Startup(IBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
            var commandB = new CommandB
            {
                Id = Guid.NewGuid(),
                DoB = DateTime.UtcNow,
                Name = "Trevor"
            };

            _bus.Send("HostB", commandB);
        }

        public void Stop()
        {
            return;
        }
    }
}
