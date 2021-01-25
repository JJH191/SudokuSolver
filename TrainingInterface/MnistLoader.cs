using Common;
using System;
using System.IO;
using System.Linq;

namespace TrainingInterface
{
    public class MnistLoader
    {
        private readonly string directory;
        private readonly string images;
        private readonly string labels;

        public MnistLoader(string directory, string images, string labels)
        {
            this.directory = directory;
            this.images = images;
            this.labels = labels;
        }

        public InputData[] GetInputData()
        {
            // Code from https://stackoverflow.com/questions/46845406/read-emnist-database-in-c-sharp
            // Modified to store the data in my inputData array rather than two seperate byte arrays
            string images = Path.Combine(directory, this.images);
            string labels = Path.Combine(directory, this.labels);

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

                InputData[] inputData = new InputData[numImages];

                int dimensions = numRows * numCols;
                for (int i = 0; i < numImages; i++)
                {
                    double[] inputs = brImages.ReadBytes(dimensions).Select(x => Utils.MapRange(x, 0, 255, 0.01, 1)).ToArray();
                    int target = brLabels.ReadByte();

                    inputData[i] = new InputData(inputs, target);
                }

                return inputData;
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
    }
}
