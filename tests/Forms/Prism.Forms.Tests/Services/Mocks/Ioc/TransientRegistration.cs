using System;

namespace Prism.Forms.Tests.Services.Mocks.Ioc
{
    public class TransientRegistration : IRegistration
    {
        public Type ServiceType { get; set; }
        public Type ImplementingType { get; set; }
        public string Name { get; set; }

        public object Create() => Activator.CreateInstance(ImplementingType ?? ServiceType);
    }
}
