using Prism.Common;

namespace Prism.Tests.Common.Mocks
{
    internal class MockParameters : ParametersBase
    {
        public MockParameters() { }
        public MockParameters(string query) : base(query) { }
    }
}
