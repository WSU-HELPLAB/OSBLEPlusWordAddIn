using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSBLEPlusWordAddin.Rubric
{
    public class Level
    {
        public string Header { get; set; }
        public string Details { get; set; }

        public Level(string header, string details)
        {
            Header = header;
            Details = details;
        }
    }
}
