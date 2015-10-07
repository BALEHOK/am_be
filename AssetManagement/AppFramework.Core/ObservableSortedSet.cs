﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AppFramework.Core
{
    public class ObservableSortedSet<T> : ICollection<T>,
                                          INotifyCollectionChanged,
                                          INotifyPropertyChanged
    {
        readonly SortedSet<T> _innerCollection = new SortedSet<T>();

        public IEnumerator<T> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _innerCollection.Add(item);
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            _innerCollection.Clear();
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            _innerCollection.Remove(item);
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return true;
        }

        public int Count
        {
            get { return _innerCollection.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<T>)_innerCollection).IsReadOnly; }
        }

        // TODO: possibly add some specific methods, if needed

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
