using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Prism.Mvvm
{
    /// <summary>
    /// Subscribe PropertyChangedEvent utility. 
    /// </summary>
    public sealed class PropertyChangedListener : IDisposable, IEnumerable
    {
        private INotifyPropertyChanged Target { get; set; }

        private Dictionary<string, List<PropertyChangedEventHandler>> PropertyChangeds { get; } = new Dictionary<string, List<PropertyChangedEventHandler>>();

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="target">PropertyChanged event invoke target.</param>
        public PropertyChangedListener(INotifyPropertyChanged target)
        {
            this.Target = target;
            target.PropertyChanged += Target_PropertyChanged;
        }

        ~PropertyChangedListener()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Add PropertyChangedEventHandler.
        /// </summary>
        /// <param name="propertyName">Target property name.</param>
        /// <param name="h">EventHandler</param>
        public void Add(string propertyName, PropertyChangedEventHandler h)
        {
            lock (PropertyChangeds)
            {
                var l = default(List<PropertyChangedEventHandler>);
                if (!PropertyChangeds.TryGetValue(propertyName, out l))
                {
                    l = new List<PropertyChangedEventHandler>();
                    PropertyChangeds.Add(propertyName, l);
                }

                l.Add(h);
            }
        }

        /// <summary>
        /// Unsubscribe all EventHandler.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void Target_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock (PropertyChangeds)
            {
                var l = default(List<PropertyChangedEventHandler>);
                if (PropertyChangeds.TryGetValue(e.PropertyName, out l))
                {
                    foreach (var h in l)
                    {
                        h?.Invoke(sender, e);
                    }
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Target.PropertyChanged -= Target_PropertyChanged;
                this.Target = null;
            }
        }

        // Support collection initializer.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return PropertyChangeds.ToArray().GetEnumerator();
        }
    }
}
