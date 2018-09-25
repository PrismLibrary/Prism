using System;

namespace Prism.Navigation
{
    public interface INavigationPath
    {
        int Index { get; }
        string Key { get; }
        Type View { get; }
        INavigationParameters Parameters { get; }
        string QueryString { get; }
        Type ViewModel { get; }

        string ToString();
    }
}