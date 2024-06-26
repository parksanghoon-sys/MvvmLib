using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CoreMvvmLib.WPF.Components
{
    public class CustomObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification = false;

        public CustomObservableCollection()
            : base()
        {
        }
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_suppressNotification == false)
                base.OnCollectionChanged(e);
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            foreach (T item in list)
            {
                Add(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
