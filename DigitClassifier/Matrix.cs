using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DigitClassifier
{
    public class Matrix
    {
        public readonly double[][] data;

        private Matrix(int rows, int cols)
        {
            data = new double[rows][];

            for (var i = 0; i < rows; i++)
            {
                data[i] = new double[cols];
            }
        }

        private Matrix(double[][] array)
        {
            data = array;
        }

        private static double[][] CreateJagged(int rows, int cols)
        {
            var jagged = new double[rows][];

            for (var i = 0; i < rows; i++)
            {
                jagged[i] = new double[cols];
            }

            return jagged;
        }

        public static Matrix Create(int rows, int cols)
        {
            return new Matrix(rows, cols);
        }

        public static Matrix Create(double[][] array)
        {
            return new Matrix(array);
        }

        public static Matrix Create(double[] arr)
        {
            var input = new double[arr.Length][];

            for (var x = 0; x < input.Length; x++)
            {
                input[x] = new[] { arr[x] };
            }

            return Create(input);
        }

        public static Matrix CopyOf(Matrix m)
        {
            Matrix result = new Matrix(m.data.Length, m.data[0].Length);
            for (int i = 0; i < m.data.Length; i++)
            {
                for (int j = 0; j < m.data[i].Length; j++)
                {
                    result.data[i][j] = m.data[i][j];
                }
            }

            return result;
        }

        public void Initialize(Func<double> elementInitializer)
        {
            for (var x = 0; x < data.Length; x++)
            {
                for (var y = 0; y < data[x].Length; y++)
                {
                    data[x][y] = elementInitializer();
                }
            }
        }

        public static Matrix Map(Matrix matrix, Func<double, double> func)
        {
            Matrix m = CopyOf(matrix);
            for (var x = 0; x < m.data.Length; x++)
            {
                for (var y = 0; y < m.data[x].Length; y++)
                {
                    m.data[x][y] = func(m.data[x][y]);
                }
            }

            return m;
        }

        public void Map(Func<double, double> func)
        {
            for (var x = 0; x < data.Length; x++)
            {
                for (var y = 0; y < data[x].Length; y++)
                {
                    data[x][y] = func(data[x][y]);
                }
            }
        }

        public void MapWithIJ(Func<double, int, int, double> func)
        {
            for (var x = 0; x < data.Length; x++)
            {
                for (var y = 0; y < data[x].Length; y++)
                {
                    data[x][y] = func(data[x][y], x, y);
                }
            }
        }

        public double[][] Value => data;

        public static Matrix operator -(Matrix a, Matrix b)
        {
            var newMatrix = CreateJagged(a.Value.Length, b.Value[0].Length);

            for (var x = 0; x < a.Value.Length; x++)
            {
                for (var y = 0; y < a.Value[x].Length; y++)
                {
                    newMatrix[x][y] = a.Value[x][y] - b.Value[x][y];
                }
            }

            return Create(newMatrix);
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            var newMatrix = CreateJagged(a.Value.Length, b.Value[0].Length);

            for (var x = 0; x < a.Value.Length; x++)
            {
                for (var y = 0; y < a.Value[x].Length; y++)
                {
                    newMatrix[x][y] = a.Value[x][y] + b.Value[x][y];
                }
            }

            return Create(newMatrix);
        }

        public static Matrix operator +(Matrix a, double b)
        {
            for (var x = 0; x < a.Value.Length; x++)
            {
                for (var y = 0; y < a.Value[x].Length; y++)
                {
                    a.Value[x][y] = a.Value[x][y] + b;
                }
            }

            return a;
        }

        public static Matrix operator -(double a, Matrix m)
        {
            for (var x = 0; x < m.Value.Length; x++)
            {
                for (var y = 0; y < m.Value[x].Length; y++)
                {
                    m.Value[x][y] = a - m.Value[x][y];
                }
            }

            return m;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Value.Length == b.Value.Length && a.Value[0].Length == b.Value[0].Length)
            {
                var m = CreateJagged(a.Value.Length, a.Value[0].Length);

                Parallel.For(0, a.Value.Length, i =>
                {
                    //Parallel.For(0, a.Value[i].Length, j => m[i][j] = a.Value[i][j] * b.Value[i][j]);
                    for (var j = 0; j < a.Value[i].Length; j++)
                    {
                        m[i][j] = a.Value[i][j] * b.Value[i][j];
                    }
                });

                return Create(m);
            }

            var newMatrix = CreateJagged(a.Value.Length, b.Value[0].Length);

            if (a.Value[0].Length == b.Value.Length)
            {
                var length = a.Value[0].Length;

                Parallel.For(0, a.Value.Length, i =>
                {
                    for (var j = 0; j < b.Value[0].Length; j++)
                    {
                        var temp = 0.0;

                        for (var k = 0; k < length; k++)
                        {
                            temp += a.Value[i][k] * b.Value[k][j];
                        }

                        newMatrix[i][j] = temp;
                    }
                });
            }

            return Create(newMatrix);
        }

        public static Matrix operator *(double scalar, Matrix b)
        {
            var newMatrix = CreateJagged(b.Value.Length, b.Value[0].Length);

            for (var x = 0; x < b.Value.Length; x++)
            {
                for (var y = 0; y < b.Value[x].Length; y++)
                {
                    newMatrix[x][y] = b.Value[x][y] * scalar;
                }
            }

            return Create(newMatrix);
        }

        public Matrix Transpose()
        {
            var rows = data.Length;

            var newMatrix = CreateJagged(data[0].Length, rows); //rows --> cols, cols --> rows

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < data[row].Length; col++)
                {
                    newMatrix[col][row] = data[row][col];
                }
            }

            return Create(newMatrix);
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(data.Length);
            bw.Write(data[0].Length);

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[0].Length; j++)
                {
                    bw.Write(data[i][j]);
                }
            }
        }

        public static Matrix Deserialize(BinaryReader br)
        {
            int length0 = br.ReadInt32();
            int length1 = br.ReadInt32();

            double[][] data = new double[length0][];
            for (int i = 0; i < length0; i++)
            {
                for (int j = 0; j < length1; j++)
                {
                    if (j == 0) data[i] = new double[length1];
                    data[i][j] = br.ReadDouble();
                }
            }

            return new Matrix(data);
        }

        public double[] ToArray()
        {
            //if (_matrix.Length != 1) throw new Exception("Matrix must only have one column.");
            return data.SelectMany(x => x.Select(y => y)).ToArray();
        }
    }
}
