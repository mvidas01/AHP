using System;
using System.Collections.Generic;
using System.Text;

namespace AHP.Enitities
{
    public class AlternativeData
    {
        public int Index { get; set; }

        public string CriterionName { get; set; }

        public string[] AlternativeNames { get; set; }

        public double[,] AlternativeMatrix { get; set; }

        public double[,] AlternativeNormalizeMatrix { get; set; }

        public double[] ColumnsSum { get; set; }
        public double[] Weights { get; set; }
    }
}
