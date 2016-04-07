#if TEST

using Xamarin.Forms;

namespace Prism
{
    /// <summary>
    /// Class that mimic <see cref="Application" /> to make life easier when testing
    /// </summary>
    public abstract class FormsApplication
    {
        protected FormsApplication()
        {
            Current = this;
        }

        public Page MainPage { get; set; }

        public static FormsApplication Current { get; set; }
    }
}

#endif