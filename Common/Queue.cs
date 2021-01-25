using System;
using System.Collections.Generic;

namespace Common
{
    // TODO (ESSENTIAL): inspiration from https://developerslogblog.wordpress.com/2019/07/23/how-to-implement-a-stack-in-c/
    /// <summary>
    /// A FIFO data structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Queue<T>
    {
        private T[] data; // Holds the data in the stack

        public int Count { get => end - start; } // Keeps track of the number of elements
        private int end = 0;
        private int start = 0;

        /// <summary>
        /// Create a stack starting with a size of <paramref name="initialSize"/>
        /// </summary>
        /// <param name="initialSize"></param>
        public Queue(int initialSize)
        {
            data = new T[initialSize]; // Initialise the data with the original size
        }

        /// <summary>
        /// Add a new item to the end of the stack
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            // If the stack is full, create a new array with double the length and copy across the data
            if (end == data.Length)
            {
                T[] resized = new T[data.Length * 2];
                data.CopyTo(resized, 0);
                data = resized;
            }

            // Add the item to the array and increment the Count for the next item
            data[end++] = item;
        }

        /// <summary>
        /// Returns the last element in the stack and then removes it
        /// </summary>
        /// <returns>The last element in the stack</returns>
        public T Pop()
        {
            if (Count == 0) throw new IndexOutOfRangeException("Could not pop, the stack is empty"); // Throw an error if user tries to pop an empty stack
            return data[start++]; // Return the last element and decrement Count so the item is essential erased (it will be overritten on the next push)
        }

        /// <summary>
        /// Returns the last element in the stack without removing it
        /// </summary>
        /// <returns>The last element in the stack</returns>
        public T Peek()
        {
            if (Count == 0) throw new IndexOutOfRangeException("Could not peek, the stack is empty"); // Throw an error if user tries to peek an empty stack
            return data[start]; // Return the last element
        }

        /// <summary>
        /// Checks if the stack contains <paramref name="item"/>
        /// </summary>
        /// <param name="item">Item to find in the stack</param>
        /// <returns>True if the stack contains the item, false otherwise</returns>
        public bool Contains(T item)
        {
            // Loop through all the items in the stack
            for (int i = 0; i < Count; i++)
                if (EqualityComparer<T>.Default.Equals(data[i], item)) return true; // If item is equal, return true (using EqualityComparer to ensure use of IEquatable functions) 

            // Return false as no item in the stack are equal to the given item
            return false;
        }
    }
}
