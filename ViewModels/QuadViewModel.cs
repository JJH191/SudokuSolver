using HelperClasses;
using Models;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Data;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the data for the quad that marks the outline of the sudoku
    /// </summary>
    public class QuadViewModel : INotifyPropertyChanged
    {
        private readonly QuadModel quad;

        /// <summary>
        /// Create a new quad model with the provided <paramref name="quad"/>
        /// </summary>
        /// <param name="quad">The corners of the quadrilateral</param>
        public QuadViewModel(Vector2D[] quad)
        {
            this.quad = new QuadModel(quad);
        }

        /// <summary>
        /// Access an individual corner of the quad
        /// </summary>
        /// <param name="index">The index of the corner to get/set</param>
        /// <returns>Corner of the quad at the provided <paramref name="index"/></returns>
        public Point this[int index]
        {
            get => quad[index];
            set
            {
                quad[index] = value; // Set the new value
                Notify(Binding.IndexerName); // Notify the UI of a change
            }
        }

        /// <summary>
        /// Represents the number of corners in the quad
        /// </summary>
        public int Length => 4; // The number of corners of the quad - will always be 4

        /// <summary>
        /// Get the quad model to access QuadModel methods
        /// </summary>
        /// <returns></returns>
        public QuadModel GetModel() => quad;

        // Notifier for when a property changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}