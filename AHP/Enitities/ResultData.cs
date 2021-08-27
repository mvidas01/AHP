using System;
using System.Collections.Generic;
using System.Text;

namespace AHP.Enitities
{
    public class ResultData
    {
        public string[] AlternativeNames { get; set; }
        public string[] CriterionNames { get; set; }
        public double[,] AlternativesToCriterias { get; set; } 
        public double[] ResultWeight { get; set; } 
    }
}
