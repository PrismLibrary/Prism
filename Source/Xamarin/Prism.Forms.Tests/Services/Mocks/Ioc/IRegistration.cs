using System;

namespace Prism.Forms.Tests.Services.Mocks.Ioc
{
    public interface IRegistration
    {
        Type ServiceType { get; }
        string Name { get; }
        object Create();
    }
}
