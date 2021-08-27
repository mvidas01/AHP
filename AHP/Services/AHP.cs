using System;
using System.Collections.Generic;
using System.Text;
using AHP.Enitities;
using AHP.Helpers;

namespace AHP.Services
{
    public interface IAHP
    {
        string Target { get; set; }
        List<string> AlternativeNames { get; set; } // list of alternative names
        List<string> CriterionNames { get; set; } // list of criterion names 
        ResultData ResultData { get; set; } //result matrices
        CriterionData CriterionData { get; set; } //criterion comparison matrices
        AlternativeData[] AlternativesData { get; set; } //matrices for comparing alternatives

        void Start();
        void Clear();
        void CriterionPairwiseCompare(int x, int y, int value);
        void CriterionAnalyze();
        void AlternativePairwiseCompare(int x, int y, int value, int index);
        void AlternativeAnalyze(int index);
        void CalculateResult();

    }

    public class AHP : IAHP
    {
        public string Target { get; set; } // 

        public List<string> AlternativeNames { get; set; } 

        public List<string> CriterionNames { get; set; }

        public ResultData ResultData { get; set; } 

        public CriterionData CriterionData { get; set; } 

        public AlternativeData[] AlternativesData { get; set; }

        public AHP()
        {
            AlternativeNames = new List<string>();
            CriterionNames = new List<string>();
            ResultData = new ResultData();
            CriterionData = new CriterionData();
        }
        
        //LOGIC

        public void Start()
        {
            var criteriaNamesArray = CriterionNames.ToArray();
            var criteriaMatrixSize = criteriaNamesArray.Length;

            //initialization of empty arrays
            CriterionData.CriterionNames = criteriaNamesArray;
            CriterionData.CriterionMatrix = CreateEmptyMatrix(criteriaMatrixSize);
            CriterionData.CriterionNormalizeMatrix = CreateEmptyMatrix(criteriaMatrixSize);
            CriterionData.ColumnsSum = new double[criteriaMatrixSize];
            CriterionData.Weights = new double[criteriaMatrixSize];

            var alternativeNamesArray = AlternativeNames.ToArray();
            var alternativeMatrixSize = alternativeNamesArray.Length;
            AlternativesData = new AlternativeData[criteriaMatrixSize];

            for (int i = 0; i < criteriaMatrixSize; i++)
            {
                AlternativeData alternative = new AlternativeData();
                alternative.Index = i;
                alternative.CriterionName = CriterionNames[i];
                alternative.AlternativeNames = alternativeNamesArray;
                alternative.AlternativeMatrix = CreateEmptyMatrix(alternativeMatrixSize);
                alternative.AlternativeNormalizeMatrix = CreateEmptyMatrix(alternativeMatrixSize);
                alternative.ColumnsSum = new double[alternativeMatrixSize];
                alternative.Weights = new double[alternativeMatrixSize];

                AlternativesData[i] = alternative;
            }

            ResultData.AlternativesToCriterias = new double[alternativeMatrixSize, criteriaMatrixSize];
            ResultData.CriterionNames = criteriaNamesArray;
            ResultData.AlternativeNames = alternativeNamesArray;
        }

        public void Clear()
        {
            AlternativeNames.Clear();
            CriterionNames.Clear();

            CriterionData = null;
            CriterionData = new CriterionData();

            AlternativesData = null;

            ResultData = null;
            ResultData = new ResultData();
        }

        public void CriterionPairwiseCompare(int x, int y, int value)
        {
            //pairwise comparison of criteria 
            PairwiseCompare(x, y, value, CriterionData.CriterionMatrix);
        }

        public void CriterionAnalyze()
        {
            // matrix normalization and finding criterion weights
            CriterionData.ColumnsSum = ColumnSum(CriterionData.CriterionMatrix);//receiving  sum
            NormalizeMatrix(CriterionData.CriterionMatrix, CriterionData.CriterionNormalizeMatrix, CriterionData.ColumnsSum);//normalization
            CriterionData.Weights = GetWeights(CriterionData.CriterionNormalizeMatrix);//receiving weights
        }

        public void AlternativePairwiseCompare(int x, int y, int value, int index)
        {
            //pair comparison of alternatives 
            try
            {
                PairwiseCompare(x, y, value, AlternativesData[index].AlternativeMatrix);
            }
            catch(ArgumentOutOfRangeException)
            {
                throw new ArgumentException();
            }
        }

        public void AlternativeAnalyze(int index)
        {
            try
            {
                var alternative = AlternativesData[index];

                // matrix normalization and finding alternative weights
                alternative.ColumnsSum = ColumnSum(alternative.AlternativeMatrix);//receiving sum
                NormalizeMatrix(alternative.AlternativeMatrix, alternative.AlternativeNormalizeMatrix, alternative.ColumnsSum);//normalization
                alternative.Weights = GetWeights(alternative.AlternativeNormalizeMatrix);//receiving weights
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException();
            }
        }

        public void CalculateResult()
        {
            //calculating results

            GetAlternativesToCriterias();
            var result = MultiplyMatrix(ResultData.AlternativesToCriterias, ArrayToColumn(CriterionData.Weights));
            ResultData.ResultWeight = ColumnToArray(result, 0);
        }

        private void GetAlternativesToCriterias()
        {
            //builds a matrix of alternatives to the criteria 

            for (int i = 0; i < AlternativesData.Length; i++)
            {
                var weights = AlternativesData[i].Weights;

                for(int j = 0; j < weights.Length; j++)
                {
                    ResultData.AlternativesToCriterias[j, i] = weights[j];
                }
            }
        }

        //HELPERS
        private double[,] MultiplyMatrix(double[,] A, double[,] B)
        {
            if (A == null || B == null)
                throw new ArgumentNullException();

            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);

            double temp = 0;
            double[,] result = new double[rA, cB];


            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cB; j++)
                {
                    temp = 0;
                    for (int k = 0; k < cA; k++)
                    {
                        temp += A[i, k] * B[k, j];
                    }
                    result[i, j] = temp;
                }
            }
            return result;

        }
        private void PairwiseCompare(int x, int y, int value, double[,] matrix)
        {
            // pairwise comparison of two matrix elements 

            if (matrix == null)
                throw new ArgumentNullException();

            if (x < 0 || y < 0 || x >= matrix.GetLength(0) || y >= matrix.GetLength(1))
                throw new ArgumentException("The size of matrices are different");

            if (x == y)
                throw new AHPException(AHPExceptionType.MatrixDiagonalException);

            if (value < 1 || value > 9)
                throw new AHPException(AHPExceptionType.CriteriaGradeException);

            matrix[x, y] = Convert.ToDouble(value);
            matrix[y, x] = 1.0 / Convert.ToDouble(value); ;
        }
        private double[] ColumnSum(double [,] matrix)
        {
            //the sum of matrix column values

            if (matrix == null)
                throw new ArgumentNullException();

            double[] columns = new double[matrix.GetLength(1)];

            for(int i = 0; i < matrix.GetLength(1); i++)
            {
                double sum = 0;

                for(int j = 0; j < matrix.GetLength(0); j++)
                {
                    sum += matrix[j, i];
                }

                columns[i] = sum;
            }

            return columns;
        }
        private double[] GetWeights(double[,] matrix)
        {
            //average of the rows

            if (matrix == null)
                throw new ArgumentNullException();
            
            double[] avarageRows = new double[matrix.GetLength(0)];

            for(int i = 0; i < matrix.GetLength(0); i++)
            {
                double sum = 0;

                for(int j = 0; j < matrix.GetLength(1); j++)
                {
                    var a = matrix[i, j];
                    sum += a;
                    
                }
                var result = sum / matrix.GetLength(1);
                avarageRows[i] = sum / matrix.GetLength(1);
            }

            return avarageRows;
        }
        private void NormalizeMatrix(double[,] matrix, double[,] normalizeMatrix, double [] numbers)
        {
            //matrix normalization 

            if (matrix == null || numbers == null || normalizeMatrix == null)
                throw new ArgumentNullException();

            if (matrix.GetLength(0) != normalizeMatrix.GetLength(0) || matrix.GetLength(1) != normalizeMatrix.GetLength(1))
                throw new ArgumentException("The size of matrices are different"); //если размеры матриц не совпадают

            if (matrix.GetLength(1) != numbers.Length)
                throw new AHPException(AHPExceptionType.WrongColumnsException);

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    normalizeMatrix[i, j] = matrix[i, j] / numbers[j];
                }
            }
        }
        private double[,] CreateEmptyMatrix(int length)
        {
            //create an empty matrix filled with ones

            var tempMatrix = new double[length, length];
            for(int i = 0; i < length; i++)
            {
                for(int j = 0; j < length; j++)
                {
                    tempMatrix[i, j] = 1;
                }
            }

            return tempMatrix;
        }
        private double[,] ArrayToColumn(double [] array)
        {
            if (array == null)
                throw new ArgumentNullException();

            double[,] column = new double[array.Length, 1];

            for(int i = 0; i < array.Length; i++)
                column[i, 0] = array[i];

            return column;
        }
        private double[] ColumnToArray(double [,] matrix, int index)
        {
            if (matrix == null)
                throw new ArgumentNullException();


            double [] array = new double[matrix.GetLength(0)];
            for (int i = 0; i < array.Length; i++)
                array[i] = matrix[i, index];

            return array;
        }

    }
    

}
