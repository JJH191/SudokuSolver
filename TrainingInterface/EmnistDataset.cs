using Common;
using System;
using System.IO;
using System.Linq;

namespace TrainingInterface
{
    /// <summary>
    /// Dataset including the hand-drawn digits from the EMNIST dataset
    /// </summary>
    public class EmnistDataset : IDataset
    {
        private readonly InputData[] inputData;

        public EmnistDataset(string directory)
        {
            // Code from https://stackoverflow.com/questions/46845406/read-emnist-database-in-c-sharp
            // Modified to store the data in my inputData array rather than two seperate byte arrays
            string images = Path.Combine(directory, "emnist-digits-train-images-idx3-ubyte");
            string labels = Path.Combine(directory, "emnist-digits-train-labels-idx1-ubyte");

            using (BinaryReader brImages = new BinaryReader(new FileStream(images, FileMode.Open)),
                                brLabels = new BinaryReader(new FileStream(labels, FileMode.Open)))
            {
                int magic1 = ReadInt32Endian(brImages);
                if (magic1 != 2051) throw new Exception($"Invalid magic number {magic1}!");

                int numImages = ReadInt32Endian(brImages);
                int numRows = ReadInt32Endian(brImages);
                int numCols = ReadInt32Endian(brImages);

                int magic2 = ReadInt32Endian(brLabels);
                int numLabels = ReadInt32Endian(brLabels);

                if (magic2 != 2049) throw new Exception($"Invalid magic number {magic2}!");
                if (numLabels != numImages) throw new Exception($"Number of labels ({numLabels}) does not equal number of images ({numImages})");

                inputData = new InputData[numImages];

                int dimensions = numRows * numCols;
                for (int i = 0; i < numImages; i++)
                {
                    // TODO (CHECK): Check MapRange normalises correctly
                    double[] inputs = Transpose(brImages.ReadBytes(dimensions).Select(x => Utils.MapRange(x, 0, 255, 0.01, 1)).ToArray(), numRows, numCols);
                    int target = brLabels.ReadByte();

                    inputData[i] = new InputData(inputs, target);
                }
            }
        }

        // Code from https://stackoverflow.com/questions/46845406/read-emnist-database-in-c-sharp
        private int ReadInt32Endian(BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(int));
            if (BitConverter.IsLittleEndian) 
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public void Shuffle()
        {
            inputData.Shuffle();
        }
         
        // TODO (ESSENTIAL): Not my code
        private double[] Transpose(double[] data, int width, int height)
        {
            double[] transposed = new double[data.Length];

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    int transposedIndex = j * width + i;
                    int originalIndex = i * height + j;

                    transposed[transposedIndex] = data[originalIndex];
                }
            }

            return transposed;
        }

        public InputData[] GetData() => inputData;
    }
}
