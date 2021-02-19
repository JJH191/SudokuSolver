using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DigitClassifier
{
    public class Matrix
    {
        private readonly double[][] data;

        #region Constructors
        /// <summary>
        /// Create a matrix with the specified number of <paramref name="rows"/> and <paramref name="cols"/>
        /// </summary>
        /// <param name="rows">The number of rows of the matrix</param>
        /// <param name="cols">The number of columns of the matrix</param>
        public Matrix(int rows, int cols)
        {
            data = new double[rows][]; // Create a 2D array with the specified number of rows

            // Fill each index of the array with an array of length 'cols'
            for (int i = 0; i < rows; i++)
                data[i] = new double[cols];
        }

        /// <summary>
        /// Create a matrix from a 2D array
        /// </summary>
        /// <param name="data">The data to fill the matrix with</param>
        public Matrix(double[][] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Create a matrix from a 1D array (a column <paramref name="vector"/>)
        /// </summary>
        /// <param name="vector">The column vector to create a matrix from</param>
        public Matrix(double[] vector)
        {
            // Create a 2D array with 1 column and the same number of rows as the vector
            data = new double[vector.Length][];

            for (int i = 0; i < vector.Length; i++)
                data[i] = new double[] { vector[i] }; // Create an array with one item in it (from the vector)
        }

        /// <summary>
        /// Create a deep copy of a matrix
        /// </summary>
        /// <param name="matrix">The matrix to copy</param>
        public Matrix(Matrix matrix)
        {
            // Create a 2D array with the same number of rows
            data = new double[matrix.data.Length][];

            // Fill the data with the values from 'matrix'
            for (int i = 0; i < matrix.data.Length; i++)
            {
                for (int j = 0; j < matrix.data[i].Length; j++)
                {
                    data[i][j] = matrix.data[i][j];
                }
            }
        }
        #endregion

        #region Mathematical operations
        /// <summary>
        /// Adds <paramref name="matrix1"/> and <paramref name="matrix2"/> together
        /// Each item in <paramref name="matrix1"/> is added to the corresponding value in <paramref name="matrix2"/>
        /// </summary>
        /// <param name="matrix1">First matrix</param>
        /// <param name="matrix2">Second matrix</param>
        /// <returns>A new matrix which is the sum of <paramref name="matrix1"/> and <paramref name="matrix2"/></returns>
        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.data.Length != matrix2.data.Length || matrix1.data[0].Length != matrix2.data[0].Length) throw new ArgumentException("Shape error - matrices do not have the same shape");
            return Map(matrix1, (value, i, j) => value + matrix2.data[i][j]); // Loop through all the values in matrix1 and add the value of matrix2 at the same location
        }

        /// <summary>
        /// Subtract <paramref name="matrix2"/> from <paramref name="matrix1"/>
        /// Each item in <paramref name="matrix2"/> is subtracted from the corresponding value in <paramref name="matrix1"/>
        /// </summary>
        /// <param name="matrix1">The matrix to subtract from</param>
        /// <param name="matrix2">The matrix that is subtracted</param>
        /// <returns>A new matrix where each item in <paramref name="matrix2"/> is subtracted from <paramref name="matrix1"/></returns>
        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.data.Length != matrix2.data.Length || matrix1.data[0].Length != matrix2.data[0].Length) throw new ArgumentException("Shape error - matrices do not have the same shape");
            return Map(matrix1, (value, i, j) => value - matrix2.data[i][j]); // Loop through all the values in matrix1 and subtract the value of matrix2 at the same location
        }

        /// <summary>
        /// Scalar multiplication between <paramref name="matrix"/> and <paramref name="val"/>
        /// Each item in the <paramref name="matrix"/> is multiplied by <paramref name="val"/>
        /// </summary>
        /// <param name="matrix">The matrix to multiply</param>
        /// <param name="val">The value to multiply each item by</param>
        /// <returns>A new matrix with all the items multiplied by <paramref name="val"/></returns>
        public static Matrix operator *(Matrix matrix, double val)
        {
            return Map(matrix, (value) => value * val); // Loop through all the values in matrix and multiply them by val
        }

        /// <summary>
        /// Matrix multiplication of <paramref name="matrix1"/> and <paramref name="matrix2"/>
        /// The items in the rows of <paramref name="matrix1"/> are multiplied by the items in the columns of <paramref name="matrix2"/> and then it is summed up for each value in the resulting matrix
        /// </summary>
        /// <param name="matrix1">First matrix</param>
        /// <param name="matrix2">Second matrix</param>
        /// <returns>A new matrix that is the matrix multiplication of <paramref name="matrix1"/> and <paramref name="matrix2"/></returns>
        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.data[0].Length != matrix2.data.Length) throw new ArgumentException("Shape error - left matrix columns should equal right matrix rows");

            double[][] matrixProduct = new double[matrix1.data.Length][];

            Parallel.For(0, matrix1.data.Length, i =>
            {
                matrixProduct[i] = new double[matrix2.data[0].Length];
                for (int j = 0; j < matrix2.data[0].Length; j++)
                {
                    double sum = 0;

                    // Multiply each row item in matrix1 by the corresponding item in the column of matrix2 and add it all up
                    for (int k = 0; k < matrix1.data[0].Length; k++)
                        sum += matrix1.data[i][k] * matrix2.data[k][j];

                    matrixProduct[i][j] = sum;
                }
            });

            return new Matrix(matrixProduct);
        }

        /// <summary>
        /// Multiplies all the items in <paramref name="matrix1"/> by the corresponding value in <paramref name="matrix2"/>
        /// </summary>
        /// <param name="matrix1">First matrix</param>
        /// <param name="matrix2">Second matrix</param>
        /// <returns>A new matrix where each item is the product of the items from <paramref name="matrix1"/> and <paramref name="matrix2"/></returns>
        public static Matrix ScalarProduct(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.data.Length != matrix2.data.Length || matrix1.data[0].Length != matrix2.data[0].Length) throw new ArgumentException("Shape error - matrices do not have the same shape");

            return Map(matrix1, (value, i, j) => value * matrix2.data[i][j]); // Loop through all the values in matrix1 and multiply by the value of matrix2 at the same location
        }

        /// <summary>
        /// Get the transposition of this matrix
        /// Transposing will swap the rows and columns of the matrix
        /// </summary>
        /// <returns>This matrix after being transposed</returns>
        public Matrix GetTransposed()
        {
            Matrix transposed = new Matrix(data[0].Length, data.Length); // Create a new matrix with the rows and columns flipped

            // Loop through all the values in col i and row j of this matrix
            // and set col j and row i on the transposed matrix to that value
            // (swapping the rows and columns)
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    transposed.data[j][i] = data[i][j];
                }
            }

            return transposed;
        }
        #endregion

        #region Map
        /// <summary>
        /// Applies the function <paramref name="func"/> to each item in the matrix
        /// </summary>
        /// <param name="func">The function to apply to each item - taking in the current value and returning the new one</param>
        public void Map(Func<double, double> func)
        {
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = func(data[i][j]); // Apply the function to each item
                }
            }
        }

        /// <summary>
        /// Applies the function <paramref name="func"/> to each item in the matrix
        /// </summary>
        /// <param name="func">The function to apply to each item - taking in the current value, and the position of that value, then returning the new value</param>
        public void Map(Func<double, int, int, double> func)
        {
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = func(data[i][j], i, j); // Apply the function to each item
                }
            }
        }

        // Static mapping

        /// <summary>
        /// Applies the function <paramref name="func"/> to each item in the matrix
        /// </summary>
        /// <param name="matrix">The matrix to apply <paramref name="func"/> to</param>
        /// <param name="func">The function to apply to each item - taking in the current value and returning the new one</param>
        /// <returns>A new matrix filled with the results of applying <paramref name="func"/> to the items in <paramref name="matrix"/></returns>
        public static Matrix Map(Matrix matrix, Func<double, double> func)
        {
            Matrix result = new Matrix(matrix.data.Length, matrix.data[0].Length);
            for (int i = 0; i < result.data.Length; i++)
            {
                for (int j = 0; j < result.data[i].Length; j++)
                {
                    result.data[i][j] = func(matrix.data[i][j]); // Apply the function to each item
                }
            }

            return result;
        }

        /// <summary>
        /// Applies the function <paramref name="func"/> to each item in the matrix
        /// </summary>
        /// <param name="matrix">The matrix to apply <paramref name="func"/> to</param>
        /// <param name="func">The function to apply to each item - taking in the current value, and the position of that value, then returning the new value</param>
        /// <returns>A new matrix filled with the results of applying <paramref name="func"/> to the items in <paramref name="matrix"/></returns>
        public static Matrix Map(Matrix matrix, Func<double, int, int, double> func)
        {
            Matrix result = new Matrix(matrix.data.Length, matrix.data[0].Length);
            for (int i = 0; i < result.data.Length; i++)
            {
                for (int j = 0; j < result.data[i].Length; j++)
                {
                    result.data[i][j] = func(matrix.data[i][j], i, j); // Apply the function to each item
                }
            }

            return result;
        }
        #endregion

        #region Serialising
        /// <summary>
        /// Write the data of this matrix to <paramref name="bw"/>
        /// </summary>
        /// <param name="bw">The destination for the data to be written to</param>
        public void Serialise(BinaryWriter bw)
        {
            bw.Write(data.Length); // Write the number of rows
            bw.Write(data[0].Length); // Then the number of columns

            // Then write each item of data
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[0].Length; j++)
                {
                    bw.Write(data[i][j]);
                }
            }
        }

        /// <summary>
        /// Reads the data from <paramref name="br"/> and creates a new matrix
        /// </summary>
        /// <param name="br">The source of data to read from</param>
        /// <returns>A new matrix with the data from the binary reader</returns>
        public static Matrix Deserialise(BinaryReader br)
        {
            // Read in the number of rows and columns6
            int rows = br.ReadInt32();
            int cols = br.ReadInt32();

            // Create a new 2D data array, then loop through the rows and columns, reading in the values to fill the array with
            double[][] data = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                data[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    data[i][j] = br.ReadDouble();
                }
            }

            return new Matrix(data);
        }
        #endregion

        /// <summary>
        /// Converts this matrix to an array
        /// NOTE: The matrix must have only 1 column
        /// </summary>
        /// <returns>The array version of this matrix</returns>
        public double[] ToArray()
        {
            // TODO (CHECK): Check my code works (myArray) as it should (test)
            // TODO (EXTRA FEATURE): Make sure the matrix has dimensions Nx1
            var test = data.SelectMany(x => x.Select(y => y)).ToArray();
            double[] myArray = new double[data.Length];
            for (int i = 0; i < myArray.Length; i++)
                myArray[i] = data[i][0];

            if (!AreEqual(myArray, test))
                throw new Exception("ToArray function failed!");

            return myArray;
        }

        // TODO (CLEANING): Remove this once done with checking
        private bool AreEqual(double[] arr1, double[] arr2)
        {
            if (arr1.Length != arr2.Length) return false;

            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i]) return false;
            }

            return true;
        }
    }
}
