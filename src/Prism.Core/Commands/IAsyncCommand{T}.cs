using System.Threading.Tasks;
using System.Windows.Input;

namespace Prism.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncCommand<T> : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task ExecuteAsync(T parameter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool CanExecute(T parameter);
    }
}
