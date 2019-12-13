using Prism.Common;

namespace Prism.Tests.Common.Mocks
{
    internal class MockParameters : ParametersBase
    {
        public MockParameters() : base() { }
        public MockParameters(string query) : base(query) { }
    }
}
