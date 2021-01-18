
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
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
        public int Value{ get; set; }
    }

    public class SqliteDataAccess
    {
        public static List<ReviewEntryModel> GetReviewEntries()
        {
            using (IDbConnection connection = new SQLiteConnection(LoadConnectionString()))
            {
                var sudokus = connection.Query<SudokuRow>("select * from Sudoku", new DynamicParameters());

                List<ReviewEntryModel> reviewEntries = new List<ReviewEntryModel>();
                foreach (SudokuRow sudoku in sudokus)
                {
                    ReviewEntryModel reviewEntry = new ReviewEntryModel(sudoku.ImagePath, DateTime.Parse(sudoku.Date));

                    var cells = connection.Query<CellRow>("select * from Cell where GridID = @GridID", new DynamicParameters(new { sudoku.GridID }));

                    int[,] grid = new int[9, 9];
                    foreach (CellRow cell in cells)
                        grid[cell.Col, cell.Row] = cell.Value;

                    reviewEntry.SudokuGrid = new SudokuGridModel(grid);
                    reviewEntries.Add(reviewEntry);
                }

                return reviewEntries;
            }
        }

        public static void Save(SudokuGridModel sudokuGrid, string imagePath)
        {
            using (IDbConnection connection = new SQLiteConnection(LoadConnectionString()))
            {
                int gridID = GetNextGridID(connection);

                connection.Open();
                var transaction = connection.BeginTransaction();

                for (int j = 0; j < sudokuGrid.Data.GetLength(1); j++)
                {
                    for (int i = 0; i < sudokuGrid.Data.GetLength(0); i++)
                    {
                        int value = sudokuGrid.Data[i, j].Number;
                        connection.ExecuteAsync("insert into Cell (GridID, Row, Col, Value) values (@gridID, @i, @j, @value)", new { gridID, i, j, value });
                    }
                }
                transaction.Commit();
                connection.Close();

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                connection.Execute("insert into Sudoku (ImagePath, Date, GridId) values (@imagePath, @date, @gridID)", new { imagePath, date, gridID });
            }
        }

        private static int GetNextGridID(IDbConnection connection)
        {
            try
            {
                int currentMaxGridID = connection.QuerySingle<int>("select max(GridID) from Cell");
                return currentMaxGridID + 1;
            }
            catch
            {
                // Table was empty, so start at index 1
                return 1;
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
