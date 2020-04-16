using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Prism.Common
{
    /// <summary>
    /// Mvvm helper methods for UI Elements
    /// </summary>
    public static class MvvmHelpers
    {
        /// <summary>
        /// Checks object type via casting using the as operator
        /// If the object is a view, the method uses the view as the Delegate's parameter
        /// If the object is a FrameworkElement, the method uses the FrameworkElement's DataContext 
        /// The method then passes the Invokes the delegate passing the cast object as a parameter
        /// </summary>
        /// <typeparam name="T">Generic Type of UI Element</typeparam>
        /// <param name="view">Object to be passed to the delegate</param>
        /// <param name="action">Delegate to call</param>
        public static void ViewAndViewModelAction<T>(object view, Action<T> action) where T : class
        {
            T viewAsT = view as T;
            if (viewAsT != null)
                action(viewAsT);
            var element = view as FrameworkElement;
            if (element != null)
            {
                var viewModelAsT = element.DataContext as T;
                if (viewModelAsT != null)
                {
                    action(viewModelAsT);
                }
            }
        }

        /// <summary>
        /// Returns the object's Implementation based on object
        /// If the object is a view, the method returns the view
        /// If the object is a FrameworkElement, the method returns FrameworkElement's DataContext 
        /// </summary>
        /// <typeparam name="T">Generic Type of UI Element</typeparam>
        /// <param name="view">Object to be passed to the delegate</param>
        public static T GetImplementerFromViewOrViewModel<T>(object view) where T : class
        {
            T viewAsT = view as T;
            if (viewAsT != null)
            {
                return viewAsT;
            }

            var element = view as FrameworkElement;
            if (element != null)
            {
                var vmAsT = element.DataContext as T;
                return vmAsT;
            }

            return null;
        }
    }
}
