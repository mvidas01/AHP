using AutoMapper;
using AHP.Enitities;
using AHP.Helpers;
using AHP.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AHP.Services
{
    public interface IAHPService
    {
        void Restart();
        void SetInitialDataGet();
        void SetInitialDataPost(InitialData initialData);
        CriterionData CriterionPairwiceCompareGet();
        CriterionCompareResponce CriterionPairwiceComparePost(CriterionCompareData model);
        CriterionIntermediateResponce CriterionIntermediateResults();
        void CriterionAnalyze();

        AlternativeData AlternativePairwiceCompareNext();
        bool IsNextAvailable();
        AlternativeData AlternativePairwiceCompareLast();
        bool IsLastAvailable();
        AlternativeCompareResponce AlternativePairwiceComparePost(AlternativeCompareData model);
        AlternativeIntermediateResponce AlternativeIntermediateResults();

        void CalculateResult();
        ResultData GetResult();


    }

    public class AHPService : IAHPService
    {
        protected IAHP ahp = new AHP(); 

        protected AHPStage AhpStage; //current stage

        protected int alternativesStage = -1; //the stage of comparing alternatives with the criteria 

        protected IMapper mapper;

        public AHPService()
        {
            AhpStage = AHPStage.AddInitialData;
            InitializeMapper();
        }

        private void InitializeMapper()
        {
            var config = new AutoMapperConfiguration().Configure();
            mapper = config.CreateMapper();
        }
        public void Restart()
        {
            ahp.Clear();
            NextStage(AHPStage.AddInitialData);
            alternativesStage = -1;
        }
        public void SetInitialDataGet()
        {
            TryAhpStage(AHPStage.AddInitialData);
        }
        public void SetInitialDataPost(InitialData initialData)
        {
            TryAhpStage(AHPStage.AddInitialData);

            if (initialData == null || initialData.Alternatives == null || initialData.Criterias == null)
                throw new ArgumentNullException();

            for(int i = 0; i < initialData.Criterias.Count; i++)
            {
                if (String.IsNullOrEmpty(initialData.Criterias[i]))
                    initialData.Criterias[i] = "Criterion " + (i+1).ToString();

                ahp.CriterionNames.Add(initialData.Criterias[i]);

            }

            for (int i = 0; i < initialData.Alternatives.Count; i++)
            {
                if (String.IsNullOrEmpty(initialData.Alternatives[i]))
                    initialData.Alternatives[i] = "Alternative " + (i+1).ToString();

                ahp.AlternativeNames.Add(initialData.Alternatives[i]);

            }

            //initialization of primary data
            ahp.Start();

            NextStage(AHPStage.CriterionPairwiseCompare);
        }

        public CriterionData CriterionPairwiceCompareGet()
        {
            TryAhpStage(AHPStage.CriterionPairwiseCompare);

            return ahp.CriterionData;
        }
        public CriterionCompareResponce CriterionPairwiceComparePost(CriterionCompareData model)
        {
            //pairwise comparison of the criteria matrix

            TryAhpStage(AHPStage.CriterionPairwiseCompare);

            if (model == null)
                throw new ArgumentNullException();

            ahp.CriterionPairwiseCompare(model.Row, model.Column, model.Value);

            var responce = new CriterionCompareResponce()
            {
                Column = model.Row,
                Row = model.Column,
                Value = ahp.CriterionData.CriterionMatrix[model.Column, model.Row]
            };

            return responce;
        }
        public CriterionIntermediateResponce CriterionIntermediateResults()
        {
            //returns the intermediate results of the pairwise comparison

            TryAhpStage(AHPStage.CriterionPairwiseCompare);

            ahp.CriterionAnalyze();

            return mapper.Map<CriterionIntermediateResponce>(ahp.CriterionData);
        }
        public void CriterionAnalyze()
        {

            //completes the pairwise comparison of criteria, switches to a new stage

            TryAhpStage(AHPStage.CriterionPairwiseCompare);

            ahp.CriterionAnalyze();

            NextStage(AHPStage.AlternativePairwiseCompare);
        }

        public bool IsNextAvailable()
        {
            TryAhpStage(AHPStage.AlternativePairwiseCompare);

            if (ahp.AlternativesData.Length-1 > alternativesStage)
                return true;
            return false;
        }
        public bool IsLastAvailable()
        {
            TryAhpStage(AHPStage.AlternativePairwiseCompare);

            if (alternativesStage > 0)
                return true;
            return false;
        }

        private AlternativeData AlternativeNext()
        {
            //returns the next alternative comparison matrix

            if (IsNextAvailable())
            {

                alternativesStage++;
                var result = ahp.AlternativesData[alternativesStage];

                return result;
            }
            else
            {
                throw new Exception("Next comparison doesn't exists");
            }

        }
        private AlternativeData AlternativeLast()
        {
            //returns the previous alternatives comparison matrix

            if (IsLastAvailable())
            {
                alternativesStage--;

                var result = ahp.AlternativesData[alternativesStage];

                return result;
            }
            else
            {
                throw new Exception("Last comparison doesn't exists");
            }

        }
        private AlternativeData AlternativeFinal()
        {
            return ahp.AlternativesData[ahp.AlternativesData.Length - 1];
        }
        private AlternativeData AlternativeFirst()
        {
            return ahp.AlternativesData[0];
        }
        public AlternativeData AlternativePairwiceCompareNext()
        {
            TryAhpStage(AHPStage.AlternativePairwiseCompare);

            if (IsNextAvailable())
                return AlternativeNext();
            else
                return AlternativeFinal();
        }
        public AlternativeData AlternativePairwiceCompareLast()
        {
            TryAhpStage(AHPStage.AlternativePairwiseCompare);

            if (IsLastAvailable())
                return AlternativeLast();
            else
                return AlternativeFirst();
        }
        public AlternativeCompareResponce AlternativePairwiceComparePost(AlternativeCompareData model)
        {
            //pairwise comparison of alternatives

            TryAhpStage(AHPStage.AlternativePairwiseCompare);

            if (model == null)
                throw new ArgumentNullException();

            ahp.AlternativePairwiseCompare(model.Row, model.Column, model.Value, model.Index);

            var responce = new AlternativeCompareResponce()
            {
                Column = model.Row,
                Row = model.Column,
                Value = ahp.AlternativesData[model.Index].AlternativeMatrix[model.Column, model.Row]
            };

            return responce;
        }
        public AlternativeIntermediateResponce AlternativeIntermediateResults()
        {
            //returns the intermediate results of the current pairwise comparison

            TryAhpStage(AHPStage.AlternativePairwiseCompare);

            if (alternativesStage >= 0 && alternativesStage < ahp.AlternativesData.Length)
                ahp.AlternativeAnalyze(alternativesStage);
            else
                throw new InvalidOperationException();

            return mapper.Map<AlternativeIntermediateResponce>(ahp.AlternativesData[alternativesStage]);
        }
        public void CalculateResult()
        {
            //calculate result 

            TryAhpStage(AHPStage.AlternativePairwiseCompare);

            for(int i = 0; i < ahp.AlternativesData.Length; i++)
            {
                ahp.AlternativeAnalyze(i);
            }

            ahp.CalculateResult();

            NextStage(AHPStage.Result);
        }
        public ResultData GetResult()
        {
            TryAhpStage(AHPStage.Result);

            return ahp.ResultData;
        }


        //HELPERS

        protected void NextStage(AHPStage stage)
        {
            AhpStage = stage;
        }
        protected void TryAhpStage(AHPStage stage)
        {
            if(AhpStage != stage)
            {
                throw new AHPStageException(AhpStage);
            }
        }

    }
}
