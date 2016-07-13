using OpenCLTemplate;
using PerfomanceProduction.KernelProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfomanceProduction.Nodes.Utils
{
    /// <summary>
    /// Узел сброса значений массива в 0.
    /// </summary>
    public class Reset
    {
        /// <summary>
        /// Кернел сброса значений массива.
        /// </summary>
        private TemplateKernel _resetKernel;

        /// <summary>
        /// Число итераций для запуска кернела reset.
        /// </summary>
        private int[] _resetWorkItemCount;

        /// <summary>
        /// Локальное число итераций для запуска кернела reset.
        /// </summary>
        private int[] _localWorkItemCount;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public Reset(ModelOption option)
        {
            Init(option);
        }

        /// <summary>
        /// Инициализация функционала.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public void Init(ModelOption option)
        {
            _resetWorkItemCount = new int[1] { 0 };
            _localWorkItemCount = new int[1] { option.LocalWorkItemCount };

            _resetKernel = new TemplateKernel(
                    ModelSystem.PATH + "reset.cl",
                    new List<ReplaceItem>() { },
                    "reset");
        }

        /// <summary>
        /// Выполнение кернелов reset.
        /// </summary>
        /// <param name="source">Множестов исходных сообщений.</param>
        public void Execute(CLCalc.Program.Variable data, int length)
        {
            _resetWorkItemCount[0] = length;

            if (_resetWorkItemCount[0] < _localWorkItemCount[0])
            {
                _localWorkItemCount[0] = _resetWorkItemCount[0];
            }

            _resetKernel.Kernel.Execute(new CLCalc.Program.Variable[] { data }, _resetWorkItemCount, _localWorkItemCount);
            CLCalc.Program.Sync();
        }
    }
}
