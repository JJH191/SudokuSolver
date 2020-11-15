using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingInterface
{
    public interface IDataset
    {
        InputData[] GetData();
        void Shuffle();
    }
}
