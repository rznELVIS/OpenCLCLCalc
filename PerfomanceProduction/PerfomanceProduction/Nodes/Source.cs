using OpenCLTemplate;
using PerfomanceProduction.KernelProcess;
using System;
using System.Collections.Generic;

namespace PerfomanceProduction.Nodes
{
    /// <summary>
    /// Узел источника данных.
    /// </summary>
    public class Source
    {
        /// <summary>
        /// Кернел генерирования данных.
        /// </summary>
        private TemplateKernel _sourceKernel;

        /// <summary>
        /// Число итераций для запуска кернела source.
        /// </summary>
        private int[] _sourceWorkItemCount;

        /// <summary>
        /// Локальное число итераций для запуска.
        /// </summary>
        private int[] _localWorkItemCount;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="option"></param>
        public Source(ModelOption option, Random random)
        {
            Init(option, random);
        }

        /// <summary>
        /// Инициализация функционала источника данных.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public void Init(ModelOption option,Random random)
        {
            _sourceWorkItemCount = new int[1] { option.GlobalWorkItemCount };
            _localWorkItemCount = new int[1] { option.LocalWorkItemCount };

            _sourceKernel = new TemplateKernel(
                ModelSystem.PATH + "source.cl",
                new List<ReplaceItem>() 
                {
                    new ReplaceItem("k", option.K.ToString()),
                    new ReplaceItem("random", random.Next(Int16.MaxValue).ToString()),
                },
                "source"
                );
        }

        /// <summary>
        /// Выполнение кернелов сравнения.
        /// </summary>
        /// <param name="source">Множестов исходных сообщений.</param>
        public void Execute(CLCalc.Program.Variable source, CLCalc.Program.Variable seed)
        {
            _sourceKernel.Kernel.Execute(new CLCalc.Program.Variable[] { source, seed }, _sourceWorkItemCount, _localWorkItemCount);
            CLCalc.Program.Sync();
        }
    }
}
