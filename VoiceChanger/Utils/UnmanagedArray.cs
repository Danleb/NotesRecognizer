using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VoiceChanger.Utils
{
    public unsafe class UnmanagedArray<T> : IEnumerable<T>, IDisposable where T : unmanaged
    {
        private readonly IntPtr _ptr;
        private readonly int _count;
        private bool _disposed = false;
        private int _addIndex = 0;

        public static implicit operator T*(UnmanagedArray<T> mem)
        {
            return (T*)mem._ptr;
        }

        public UnmanagedArray(int count)
        {
            _count = count;
            _ptr = Marshal.AllocHGlobal(_count * sizeof(T));
        }

        public UnmanagedArray(nuint count) : this((int)count)
        {

        }

        public UnmanagedArray(uint count) : this((int)count)
        {

        }

        ~UnmanagedArray()
        {
            Dispose(false);
        }

        public T this[int i]
        {
            get
            {
                if (i < 0 || i >= _count)
                {
                    throw new IndexOutOfRangeException();
                }
                return ((T*)_ptr)[i];
            }
            set
            {
                if (i < 0 || i >= _count)
                {
                    throw new IndexOutOfRangeException();
                }
                 ((T*)_ptr)[i] = value;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Add(T value)
        {
            if (_addIndex >= _count)
            {
                throw new IndexOutOfRangeException("Too many values added.");
            }

            this[_addIndex] = value;
            _addIndex++;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            Marshal.FreeHGlobal(_ptr);
            _disposed = true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Enumerator<U> : IEnumerator<U> where U : unmanaged
        {
            private readonly UnmanagedArray<U> _unmanagedArray;
            private int _index = 0;

            public U Current => _unmanagedArray[_index];

            object IEnumerator.Current => Current;

            public Enumerator(UnmanagedArray<U> unmanagedArray)
            {
                _unmanagedArray = unmanagedArray;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                _index++;
                return _index < _unmanagedArray._count;
            }

            public void Reset()
            {
                _index = 0;
            }
        }
    }
}
