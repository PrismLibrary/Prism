using System.Threading.Tasks;
using System.Windows.Input;

namespace Prism.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task ExecuteAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool CanExecute();
    }
}
