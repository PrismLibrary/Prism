using System.Threading.Tasks;
using System.Windows.Input;

namespace Prism.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
