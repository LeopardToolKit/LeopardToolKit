using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Collection
{
    public class DynamicEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> comparer;

        public DynamicEqualityComparer(Func<T, T, bool> comparer)
        {
            this.comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return this.comparer(x, y);
        }

        public int GetHashCode(T obj) => 0;
    }
}
