﻿#if TEST

using Xamarin.Forms;

namespace Prism
{
    /// <summary>
    /// Class that mimic <see cref="Application" /> to make life easier when testing
    /// </summary>
    public abstract class FormsApplication
    {
        /// <summary>
        /// Create a new instance of <see cref="FormsApplication" />
        /// </summary>
        protected internal FormsApplication()
        {
            Current = this;
        }

        /// <summary>
        /// Active page
        /// </summary>
        public Page MainPage { get; set; }

        /// <summary>
        /// Singeleton instance property
        /// </summary>
        public static FormsApplication Current { get; set; }
    }
}

#endif