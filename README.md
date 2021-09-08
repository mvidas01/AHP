# .NET implementation analytic hierarchy process 

# AHP
Если хотите написать собственное решение используйте AHP класс. Но для правильного результата необходимо соблюдать правильную последовательность операций, что AHP не обеспечивает.

1. Добавьте имена альтернатив и критерев 
2. Start
3. Попарное сравнение критериев (CriterionPairwiseCompare)
4. Нормализация матриц и нахождение весов критериев (CriterionAnalyze)
5. Попарное сравнение альтернатив (AlternativePairwiseCompare)
6. Нормализация матриц и нахождение весов альтернатив по отношению к критериям (AlternativeAnalyze)
7. 5 и 6 пункт повторяется для каждой матрицы альтернатив(количество матриц = количество критериев)
8. Получение результа (CalculateResult). Результативные матрицы храняться в ResultData
9. Clear, для того чтобы начать все заново

# AHPService
Но я рекоментую использовать AHPService, потому что он контролирует последовательность операций.

1. Инициализация имен альтернатив и критерев(SetInitialDataPost)
2. Попарное сравнение критериев (CriterionPairwiceComparePost)
3. Закончить попарное сравнение и перейти к следующему этапу(CriterionAnalyze)
4. Попарное сравнение альтернатив (AlternativePairwiceComparePost)
5. AlternativePairwiceCompareNext и AlternativePairwiceCompareLast для перемещения между матрицами альтернатив 
6. Получение результата (CalculateResult)
7. Сбросить последовательность и начать заново (Restart)

При вызове операции нарушающей последовательность (например нельзя добавить новые критерии, когда матрицы критериев уже построены) будет выброшено исключение AHPStageException.
В AHPStageException можно посмотреть на каком этапе последовательности вы сейчас находитесь. 

Например если вызвать CriterionPairwiceComparePost, после того как попарное сравнение критериев было закончено, будет выкинуто AHPStageException с AHPStage = AHPStage.AlternativePairwiseCompare. Это значит, что сейчас нужно провести попарное сранвение альтернатив, так веса критериев уже определены.

<blockquote>
            IAHPService service = new AHPService();

            //2 criteria
            //2 alternatives

            InitialData initialData = new InitialData();
            initialData.Criterias.Add("критерий 1");
            initialData.Criterias.Add("критерий 2");
            initialData.Alternatives.Add("альтернатива 1");
            initialData.Alternatives.Add("альтернатива 2");

            //set alternatives and criterias
            service.SetInitialDataPost(initialData);

            //criterion pairwise comparison 
            CriterionCompareData criterionCompareData = new CriterionCompareData()
            {
                Row = 0, //row of pairwise comparison matrix
                Column = 1, //column of pairwise comparison matrix
                Value = 2 
            };
            
            service.CriterionPairwiceComparePost(criterionCompareData);
            service.CriterionAnalyze();

            //alternavives criterion comparison
            for(int i = 0; i < 2; i++)
            {
                AlternativeCompareData compareData = new AlternativeCompareData()
                {
                    Row = 0, //row of pairwise comparison matrix
                    Column = 1, //column of pairwise comparison matrix
                    Value = 2, 
                    Index = i //number of matrix
                };

                service.AlternativePairwiceComparePost(compareData);
            }

            service.CalculateResult();

            var result = service.GetResult();

            Assert.Equal(0.667, Math.Round(result.ResultWeight[0], 3));
            Assert.Equal(0.333, Math.Round(result.ResultWeight[1], 3));
        

</blockquote>

