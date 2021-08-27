using System;
using System.Collections.Generic;
using System.Text;

namespace AHP.Helpers
{
    public enum AHPExceptionType
    {
        MatrixDiagonalException, //impossible to change the elements of the matrix diagonal 
        CriteriaGradeException, //scores between 1 and 9
        WrongColumnsException,// the number of columns of the matrix does not match, with an array of columns sums
    }

    public class AHPException : Exception
    {
        public AHPExceptionType ExceptionType { get; set; }

        public AHPException(AHPExceptionType ExceptionType)
        {
            this.ExceptionType = ExceptionType;
        }
        public AHPException(AHPExceptionType exceptionType, string message) : base(message)
        {
            this.ExceptionType = exceptionType;
        }

    }
}
