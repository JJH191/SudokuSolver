﻿using HelperClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ViewModels;
using ViewModels.Converters;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for GridPage.xaml
    /// </summary>
    public partial class GridPage : Page
    {
        //private readonly CellViewModel[,] cells = new CellViewModel[9,9];
        private readonly SudokuGridViewModel sudokuGrid;

        public GridPage(int[,] sudoku)
        {
            sudokuGrid = new SudokuGridViewModel(sudoku);
            DataContext = sudokuGrid;
            
            InitializeComponent();

            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    GrdSudokuGrid.Children.Add(GetCell(i, j));
                }
            }
        }

        #region Generating Grid
        UIElement GetCell(int i, int j)
        {
            Grid grid = new Grid();
            grid.SetBinding(BackgroundProperty, new Binding("Colour") { Source = sudokuGrid[i, j], Converter = new ColourToSolidColourBrush() });

            SetGridPosition(grid, i, j); // Set grid position

            TextBox textBox = new TextBox();
            textBox.SetBinding(TextBox.TextProperty, new Binding("Number") { Source = sudokuGrid[i, j], Converter = new IntToCellString(), UpdateSourceTrigger=UpdateSourceTrigger.PropertyChanged });
            grid.Children.Add(textBox);

            // Make transparent
            textBox.Background = new SolidColorBrush(Colors.Transparent);
            VerticalAlignment = VerticalAlignment.Stretch;
            textBox.BorderThickness = new Thickness(0);

            // Align text to center
            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox.VerticalAlignment = VerticalAlignment.Center;
            textBox.TextAlignment = TextAlignment.Center;

            // Text size
            textBox.FontSize = 20;

            return grid;
        }

        private void SetGridPosition(UIElement elem, int i, int j)
        {
            Grid.SetColumn(elem, i);
            Grid.SetRow(elem, j);
        }
        #endregion

        private void BtnSolveSudoku_Click(object sender, RoutedEventArgs e)
        {

            if (!sudokuGrid.IsFull()) // Solve
            {
                if (!sudokuGrid.Solve())
                {
                    MessageBox.Show("Could not solve the sudoku.\nThis is most likely because one or more numbers are incorrect.", "Error solving");
                }
            }
            else // Check or save
            {
                if (!sudokuGrid.IsButtonShowingSave) sudokuGrid.DisplayErrors(); // Check
                else // Save
                {
                    // TODO: Save
                    MessageBox.Show("Sudoku saved!", "Saved");

                    // Go back to main menu
                    NavigationService.GoBack();
                    NavigationService.GoBack();
                }
            }
        }

        private void BtnClearSudoku_Click(object sender, RoutedEventArgs e)
        {
            sudokuGrid.Clear();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
