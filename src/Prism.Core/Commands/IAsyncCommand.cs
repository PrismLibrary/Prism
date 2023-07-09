using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

#nullable enable
namespace Prism.Commands;

/// <summary>
/// Provides an abstraction layer for custom controls which want to make use of Async Commands
/// </summary>
public interface IAsyncCommand : ICommand
{
    /// <summary>
    /// Executes the Command with a specified parameter and the Default <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="parameter">The Command Parameter</param>
    /// <returns>An Asynchronous Task</returns>
    Task ExecuteAsync(object? parameter);

    /// <summary>
    /// Executes the Command with a specified parameter and using a <see cref="CancellationToken"/>
    /// </summary>
    /// <param name="parameter">The Command Parameter</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>An Asynchronous Task</returns>
    Task ExecuteAsync(object? parameter, CancellationToken cancellationToken);
}
