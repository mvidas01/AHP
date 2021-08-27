using System;
using System.Collections.Generic;
using System.Text;

namespace AHP.Helpers
{
  

    public class AHPStageException : Exception
    {
        public AHPStage AHPStage { get; set; }

        public AHPStageException(AHPStage stage)
        {
            AHPStage = stage;
        }

        public AHPStageException(AHPStage stage, string message) : base(message)
        {
            AHPStage = stage;
        }
    }
}
