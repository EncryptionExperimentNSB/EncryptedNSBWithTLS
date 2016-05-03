using NServiceBus;
using log4net;
using Models;

namespace HostB
{
    public class CommandBHandler : IHandleMessages<CommandB>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(CommandBHandler));
        public void Handle(CommandB message)
        {
            _logger.Info("Id: " + message.Id.ToString("D") + "DoB: " + message.DoB.ToString("yyyy-MM-dd HH:mm:ss") + "Name: " + message.Name);
        }
    }
}
