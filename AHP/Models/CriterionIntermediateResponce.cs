using System;
using System.Collections.Generic;
using System.Text;

namespace AHP.Models
{
    public class CriterionIntermediateResponce
    {
        public string[] CriterionNames { get; set; }

        public double[,] CriterionNormalizeMatrix { get; set; }

        public double[] Weights { get; set; }
    }
}
