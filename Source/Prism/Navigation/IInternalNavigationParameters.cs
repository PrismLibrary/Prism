using System.Collections.Generic;

namespace Prism.Navigation
{
    public interface IInternalNavigationParameters
    {
        IReadOnlyDictionary<string, object> InternalParameters { get; }

        void AddInternalParameter(string key, object value);
    }
}
