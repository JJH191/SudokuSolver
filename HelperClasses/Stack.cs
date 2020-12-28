using System;
using System.Collections.Generic;

namespace HelperClasses
{
    public class Stack<T>
    {
        // Use an array with a starting size that doubles in size if it gets filled up
        private T[] data;
        private int endIndex = 0;

        public Stack(int initialSize)
        {
            data = new T[initialSize];
        }

        public void Push(T item)
        {
            if (endIndex == data.Length)
            {
                T[] resized = new T[data.Length * 2];
                data.CopyTo(resized, 0);
                data = resized;
            }
            data[endIndex++] = item;
        }

        public T Pop()
        {
            if (endIndex == 0) throw new IndexOutOfRangeException("Could not pop, the stack is empty");
            return data[--endIndex];
        }

        public T Peek()
        {
            if (endIndex == 0) throw new IndexOutOfRangeException("Could not peek, the stack is empty");
            return data[endIndex - 1];
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < endIndex; i++)
            {
                if (EqualityComparer<T>.Default.Equals(data[i], item)) return true;
            }

            return false;
        }

        public int Count { get => endIndex; }
    }
}
