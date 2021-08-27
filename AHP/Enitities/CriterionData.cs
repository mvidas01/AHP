using System;
using System.Collections.Generic;
using System.Text;

namespace AHP.Enitities
{
    public class CriterionData
    {
        public string[] CriterionNames { get; set; }

        public double[,] CriterionMatrix { get; set; }

        public double[,] CriterionNormalizeMatrix { get; set; }

        public double[] ColumnsSum { get; set; }

        public double[] Weights { get; set; }
    }
}
