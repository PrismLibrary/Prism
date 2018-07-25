namespace Prism
{
    public interface IStartArgs
    {
        object Arguments { get; }
        StartCauses StartCause { get; }
    }
}
