using OpenCLTemplate;
using PerfomanceProduction.KernelProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfomanceProduction.Nodes
{
    /// <summary>
    /// Узел демодулятора - BPSK.
    /// </summary>
    public class Demodulator
    {
        /// <summary>
        /// Кернел для выполнения демдодуляции.
        /// </summary>
        private TemplateKernel _demodulateKernel;

        /// <summary>
        /// Число итераций для запуска кернела demodulate.
        /// </summary>
        private int[] _demodulatelWorkItem;

        /// <summary>
        /// Локальное число итераций для запуска.
        /// </summary>
        private int[] _localWorkItemCount;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public Demodulator(ModelOption option)
        {
            Init(option);
        }

        /// <summary>
        /// Инициализация функционала демодулятора.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public void Init(ModelOption option)
        {
            _demodulatelWorkItem = new int[1] { option.GlobalWorkItemCount * option.N };
            _localWorkItemCount = new int[1] { option.LocalWorkItemCount };

            _demodulateKernel = new TemplateKernel(
                ModelSystem.PATH + "demodulate.cl",
                new List<ReplaceItem>() { },
                "demodulate");
        }

        /// <summary>
        /// Выполнение кернелов демодулятора.
        /// </summary>
        /// <param name="channeled">Множестов сообщений переданных через канал.</param>
        public void Execute(CLCalc.Program.Variable channeled)
        {
            _demodulateKernel.Kernel.Execute(new CLCalc.Program.Variable[] { channeled }, _demodulatelWorkItem, _localWorkItemCount);
        }
    }
}
