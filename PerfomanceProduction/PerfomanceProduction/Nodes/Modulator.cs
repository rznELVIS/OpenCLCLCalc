using OpenCLTemplate;
using PerfomanceProduction.KernelProcess;
using System.Collections.Generic;

namespace PerfomanceProduction.Nodes
{
    /// <summary>
    /// Узел модулятора - BPSK.
    /// </summary>
    public class Modulator
    {
        /// <summary>
        /// Кернел для выполнения модуляции.
        /// </summary>
        private TemplateKernel _modulateKernel;

        /// <summary>
        /// Число итераций для запуска кернела modulate.
        /// </summary>
        private int[] _modulatelWorkItem;

        /// <summary>
        /// Локальное число итераций для запуска.
        /// </summary>
        private int[] _localWorkItemCount;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public Modulator(ModelOption option)
        {
            Init(option);
        }

        /// <summary>
        /// Инициализация функционала модулятора.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public void Init(ModelOption option)
        {
            _modulatelWorkItem = new int[1] { option.GlobalWorkItemCount * option.N };
            _localWorkItemCount = new int[1] { option.LocalWorkItemCount };

            _modulateKernel = new TemplateKernel(
               ModelSystem.PATH + "modulate.cl",
               new List<ReplaceItem>() { },
               "modulate");
        }

        /// <summary>
        /// Выполнение кернелов модулятора.
        /// </summary>
        /// <param name="coded">Множестов исходных закодированных сообщений.</param>
        public void Execute(CLCalc.Program.Variable coded)
        {
            _modulateKernel.Kernel.Execute(new CLCalc.Program.Variable[] { coded }, _modulatelWorkItem, _localWorkItemCount);
        }
    }
}
