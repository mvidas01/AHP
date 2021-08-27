using System;
using System.Collections.Generic;
using System.Text;

namespace AHP.Models
{
    public class AlternativeIntermediateResponce
    {
        public string[] AlternativeNames { get; set; }
        public double[,] AlternativeNormalizeMatrix { get; set; }

        public double[] ColumnsSum { get; set; }

        public double[] Weights { get; set; }
    }
}
