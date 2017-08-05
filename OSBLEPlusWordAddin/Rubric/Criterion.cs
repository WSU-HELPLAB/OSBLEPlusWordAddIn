using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSBLEPlusWordAddin.Rubric
{
    public class Criterion
    {
        public string Header { get; set; }
        public List<Level> Levels { get; set; }

        public Criterion(string header, List<Level> levels)
        {
            Header = header;
            Levels = levels;
        }
    }
}
