using OpenCLTemplate;
using PerfomanceProduction.KernelProcess;
using System;
using System.Collections.Generic;

namespace PerfomanceProduction.Nodes
{
    /// <summary>
    /// Узел физического канала.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// Кернелы для заполнения информационной части кодового слова.
        /// </summary>
        private TemplateKernel[] _channelKernel;

        /// <summary>
        /// Число итераций для запуска кернела channel.
        /// </summary>
        private int[] _channelWorkItem;

        /// <summary>
        /// Локальное число итераций для запуска.
        /// </summary>
        private int[] _localWorkItemCount;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public Channel(ModelOption option)
        {
            Init(option);
        }

        /// <summary>
        /// Инициализация функционала физического канала.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public void Init(ModelOption option)
        {
            _channelWorkItem = new int[1] { option.GlobalWorkItemCount };
            _localWorkItemCount = new int[1] { option.LocalWorkItemCount };

            _channelKernel = new TemplateKernel[option.EbValues.Count];

            for (int i = 0; i < option.EbValues.Count; i++)
            {
                _channelKernel[i] = new TemplateKernel(
                    ModelSystem.PATH + "channel.cl",
                    new List<ReplaceItem>() 
                {
                    new ReplaceItem("n", option.N.ToString()),
                    new ReplaceItem("PI", Math.PI.ToString("0.00000", System.Globalization.CultureInfo.InvariantCulture)),
                    new ReplaceItem("sigma", option.GetSigma(option.EbValues[i]).ToString("0.00000", System.Globalization.CultureInfo.InvariantCulture)),
                    new ReplaceItem("M", option.MathWait.ToString("0.00000", System.Globalization.CultureInfo.InvariantCulture))
                },
                "channel");
            }
        }

        /// <summary>
        /// Выполнение кернелов физического канала.
        /// </summary>
        /// <param name="modulate">Множестов исходных передаваемх сообщений.</param>
        /// <param name="seed">множество случайных значений, инициалзирущех генератор xorshift.</param>
        public void Execute(CLCalc.Program.Variable modulate, CLCalc.Program.Variable seed, int index)
        {
            _channelKernel[index].Kernel.Execute(new CLCalc.Program.Variable[] { modulate, seed }, _channelWorkItem, _localWorkItemCount);
        }
    }
}
