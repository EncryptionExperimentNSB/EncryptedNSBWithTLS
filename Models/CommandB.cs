using Newtonsoft.Json;
using NServiceBus;
using System;

namespace Models
{
    public class CommandB : ICommand
    {
        public Guid Id { get; set; }

        public DateTime DoB { get; set; }

        public string Name { get; set; }
    }
}
