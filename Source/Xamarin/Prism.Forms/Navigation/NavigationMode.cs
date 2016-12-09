namespace Prism.Navigation
{
    public enum NavigationMode
    {        
        //
        // Summary:
        //     Navigation is to a new instance of a page (not going forward or backward in the
        //     stack).
        New = 0,
        //
        // Summary:
        //     Navigation is going backward in the stack.
        Back = 1,
        //
        // Summary:
        //     Navigation is going forward in the stack.
        Forward = 2,
        //
        // Summary:
        //     Navigation is to the current page (perhaps with different data).
        Refresh = 3
    }
}
