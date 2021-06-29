using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MultiWPFApp.Model
{
    public class MeasurementModel
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }

    }
}
