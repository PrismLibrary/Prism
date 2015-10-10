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
        private WeakReference<INotifyPropertyChanged> Target { get; set; }

        private Dictionary<string, List<PropertyChangedEventHandler>> PropertyChangeds { get; } = new Dictionary<string, List<PropertyChangedEventHandler>>();

        private Action DisposeAction { get; set; }

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="target">PropertyChanged event invoke target.</param>
        public PropertyChangedListener(INotifyPropertyChanged target)
        {
            this.Target = new WeakReference<INotifyPropertyChanged>(target);
            target.PropertyChanged += Target_PropertyChanged;
            DisposeAction = () => target.PropertyChanged -= Target_PropertyChanged;
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
            var target = default(INotifyPropertyChanged);
            if (!Target.TryGetTarget(out target)) { return; }

            lock (PropertyChangeds)
            {
                var l = default(List<PropertyChangedEventHandler>);
                if (PropertyChangeds.TryGetValue(e.PropertyName, out l))
                {
                    foreach (var h in l)
                    {
                        h?.Invoke(target, e);
                    }
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeAction();
                this.Target = null;
                lock(PropertyChangeds)
                {
                    PropertyChangeds.Clear();
                }
            }
        }

        // Support collection initializer.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return PropertyChangeds.ToArray().GetEnumerator();
        }
    }
}
