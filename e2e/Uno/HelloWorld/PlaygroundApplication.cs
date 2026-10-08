namespace HelloWorld;

// Keep the XAML root identical for the regular and NativeAOT validation builds.
// The PrismApplication alias selects DryIoc normally and the container-neutral
// PrismApplicationBase when the validation app supplies Microsoft DI.
public abstract class PlaygroundApplication : PrismApplication
{
}
