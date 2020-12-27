using System;
using System.Collections;
using System.Collections.Generic;

namespace WebConfig
{
    internal sealed class CollectionWrapper<T> : IWrapper, ICollection, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : IWrapper, new()
    {
        public CollectionWrapper(ArrayList sourceList)
        {
            if (sourceList != null)
            {
                Initialize(sourceList);
                return;
            }
            _list = new List<T>();
        }

        public CollectionWrapper()
        {
            _list = new List<T>();
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList)_list).IsReadOnly;
            }
        }

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public void Add(T item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public object GetData()
        {
            ArrayList arrayList = new ArrayList(_list.Count);
            foreach (T t in _list)
            {
                arrayList.Add(t.GetData());
            }
            return arrayList;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        private void Initialize(ArrayList sourceList)
        {
            _list = new List<T>(sourceList.Count);
            foreach (object data in sourceList)
            {
                T item = (default(T) == null) ? Activator.CreateInstance<T>() : default;
                item.SetData(data);
                _list.Add(item);
            }
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IWrapper.SetData(object o)
        {
            Initialize((ArrayList)o);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)_list).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)_list).SyncRoot;
            }
        }

        private List<T> _list;
    }

}
