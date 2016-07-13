using OpenCLTemplate;
using PerfomanceProduction.KernelProcess;
using System.Collections.Generic;

namespace PerfomanceProduction.Nodes
{
    /// <summary>
    /// Узел приемника данных.
    /// </summary>
    public class Receiver
    {
        /// <summary>
        /// Кернел сравнения исходных и декодированных данных.
        /// </summary>
        private TemplateKernel _compareKernel;

        /// <summary>
        /// Число итераций для запуска кернела compare.
        /// </summary>
        private int[] _compareWorkItemCount;

        /// <summary>
        /// Локальное число итераций для запуска.
        /// </summary>
        private int[] _localWorkItemCount;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public Receiver(ModelOption option)
        {
            Init(option);
        }

        /// <summary>
        /// Инициализация функционала приемника данных.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public void Init(ModelOption option)
        {
            _compareWorkItemCount = new int[1] { option.GlobalWorkItemCount * option.K };
            _localWorkItemCount = new int[1] { option.LocalWorkItemCount };

            _compareKernel = new TemplateKernel(
                ModelSystem.PATH + "compare.cl",
                new List<ReplaceItem>() { },
                "compare"
                );
        }

        /// <summary>
        /// Выполнение кернелов сравнения.
        /// </summary>
        /// <param name="source">Множестов исходных сообщений.</param>
        /// <param name="coded">Множество сообщений после процесса кодирования/декодирования.</param>
        /// <param name="result">Результат сравнения.</param>
        public void Execute(CLCalc.Program.Variable source, CLCalc.Program.Variable coded, CLCalc.Program.Variable result)
        {
            _compareKernel.Kernel.Execute(new CLCalc.Program.Variable[] { source, coded, result }, _compareWorkItemCount, _localWorkItemCount);
            CLCalc.Program.Sync();
        }
    }
}
