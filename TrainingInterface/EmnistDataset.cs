using System;
using System.IO;
using System.Linq;

namespace TrainingInterface
{
    public class EmnistDataset : IDataset
    {
        private readonly InputData[] inputData;
        private readonly Random rnd = new Random();

        public EmnistDataset(string directory)
        {
            string imagesPath = Path.Combine(directory, "emnist-digits-train-images-idx3-ubyte");
            string labelsPath = Path.Combine(directory, "emnist-digits-train-labels-idx1-ubyte");

            using (BinaryReader brImages = new BinaryReader(new FileStream(imagesPath, FileMode.Open)),
                brLabels = new BinaryReader(new FileStream(labelsPath, FileMode.Open)))
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
                    double[] inputs = Transpose(brImages.ReadBytes(dimensions).Select(x => ((double)x / 255) * 0.99 + 0.01).ToArray(), numRows, numCols);
                    int target = brLabels.ReadByte();

                    inputData[i] = new InputData(inputs, target);
                }
            }
        }

        public InputData[] GetData()
        {
            return inputData;
        }

        public void Shuffle()
        {
            int n = inputData.Length;
            while (n > 1)
            {
                int k = rnd.Next(n--);

                InputData temp = inputData[n];
                inputData[n] = inputData[k];
                inputData[k] = temp;
            }
        }

        private int ReadInt32Endian(BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(int));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

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
    }
}
