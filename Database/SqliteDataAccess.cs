
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace Database
{
    #region Helper Model Classes
    class SudokuRow
    {
        public string ImagePath { get; set; }
        public string Date { get; set; }
        public int GridID { get; set; }
    }

    class CellRow
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Value { get; set; }
    }
    #endregion

    /// <summary>
    /// Provides methods to interact with the Sqlite database
    /// </summary>
    public class SqliteDataAccess
    {
        /// <summary>
        /// Get all the saved entries for the review section
        /// </summary>
        /// <returns>A list of all the saved ReviewEntryModels</returns>
        public static List<ReviewEntryModel> GetReviewEntries()
        {
            using (IDbConnection connection = new SQLiteConnection(LoadConnectionString()))
            {
                // Get all the sudokus
                var sudokus = connection.Query<SudokuRow>("SELECT * FROM Sudoku ORDER BY Date DESC", new DynamicParameters());

                List<ReviewEntryModel> reviewEntries = new List<ReviewEntryModel>();
                foreach (SudokuRow sudoku in sudokus)
                {
                    ReviewEntryModel reviewEntry = new ReviewEntryModel(sudoku.ImagePath, DateTime.Parse(sudoku.Date));

                    // Get all the cell values from the Cell table and fill a 2D array with them
                    var cells = connection.Query<CellRow>("SELECT * FROM Cell WHERE GridID = @GridID", new DynamicParameters(new { sudoku.GridID }));

                    int[,] grid = new int[9, 9];
                    foreach (CellRow cell in cells)
                        grid[cell.Row, cell.Col] = cell.Value;

                    reviewEntry.SudokuGrid = new SudokuGridModel(grid);
                    reviewEntries.Add(reviewEntry);
                }

                return reviewEntries;
            }
        }

        /// <summary>
        /// Saves the given <paramref name="sudokuGrid"/> to the database along with the path of the photo - <paramref name="imagePath"/>
        /// </summary>
        /// <param name="sudokuGrid">The grid to save</param>
        /// <param name="imagePath">The path to the image of the sudoku</param>
        public static void Save(SudokuGridModel sudokuGrid, string imagePath)
        {
            using (IDbConnection connection = new SQLiteConnection(LoadConnectionString()))
            {
                int gridID = GetNextGridID(connection); // Get a unique ID

                connection.Open();
                var transaction = connection.BeginTransaction(); // Begin a transaction so that we can commit all commands at the same time to speed up the processing

                // Add each cell in the grid to the Cell table with its corresponding row and column
                for (int j = 0; j < sudokuGrid.Data.GetLength(1); j++)
                {
                    for (int i = 0; i < sudokuGrid.Data.GetLength(0); i++)
                    {
                        int value = sudokuGrid.Data[i, j].Number;
                        connection.ExecuteAsync("INSERT INTO Cell (GridID, Row, Col, Value) VALUES (@gridID, @i, @j, @value)", new { gridID, i, j, value });
                    }
                }

                // Send all the commands
                transaction.Commit();
                connection.Close();

                // Get the current date and time as a string and insert into Sudoku along with the grid and image path
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                connection.Execute("INSERT INTO Sudoku (ImagePath, Date, GridId) VALUES (@imagePath, @date, @gridID)", new { imagePath, date, gridID });
            }
        }

        /// <summary>
        /// Works out what the next unique ID is for a grid
        /// </summary>
        /// <param name="connection">The database connection</param>
        /// <returns>A unique ID for the next grid</returns>
        private static int GetNextGridID(IDbConnection connection)
        {
            try
            {
                // Get the maximum ID currently in the database
                int currentMaxGridID = connection.QuerySingle<int>("SELECT MAX(GridID) FROM Cell");
                return currentMaxGridID + 1; // Increment it to get a unique ID
            }
            catch
            {
                // Table was empty, so start at index 1
                return 1;
            }
        }

        /// <summary>
        /// Loads the string needed to connect to the Sqlite database
        /// </summary>
        /// <param name="id">ID of the connections string in App.config</param>
        /// <returns>The string from App.config to load the requested database</returns>
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
