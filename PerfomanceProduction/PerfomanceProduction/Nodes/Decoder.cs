using OpenCLTemplate;
using PerfomanceProduction.KernelProcess;
using System.Collections.Generic;
using System.Linq;

namespace PerfomanceProduction.Nodes
{
    /// <summary>
    /// Узел декодера.
    /// </summary>
    public class Decoder
    {
        /// <summary>
        /// Кернелы для процесса декодирования.
        /// </summary>
        private TemplateKernel[] _decodeKernels;

        /// <summary>
        /// Число итераций для запуска кернела decodeKernels.
        /// </summary>
        private int[] _decodeWorkItemCount;

        /// <summary>
        /// Локальное число итераций для запуска.
        /// </summary>
        private int[] _localWorkItemCount;

        /// <summary>
        /// Число итераций декодирования.
        /// </summary>
        private int _I;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public Decoder(ModelOption option)
        {
            Init(option);
        }

        /// <summary>
        /// Инициализация функционала синдрома.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public void Init(ModelOption option)
        {
            _I = option.Thersolds.Length;
            _decodeWorkItemCount = new int[] { option.GlobalWorkItemCount, option.Branch, option.UCount };
            _localWorkItemCount = new int[] { option.LocalWorkItemCount, 1, 1 };

            _decodeKernels = new TemplateKernel[_I];
            for (var i = 0; i < _I; i++)
            {
                _decodeKernels[i] = new TemplateKernel(
                    ModelSystem.PATH + "decode.cl",
                    new List<ReplaceItem>()
                    {
                        new ReplaceItem("branch", option.Branch.ToString()),
                        new ReplaceItem("adderLength", option.CoderAdders[0][0].Length.ToString()),
                        new ReplaceItem("adders", "adders", option.DecodedAdders),
                        new ReplaceItem("k", option.K.ToString()),
                        new ReplaceItem("m", option.M.ToString()),
                        new ReplaceItem("v", option.VCount.ToString()),
                        new ReplaceItem("u", option.UCount.ToString()),
                        new ReplaceItem("thershold", option.Thersolds[i].ToString()),
                        new ReplaceItem("adderBranch", option.CoderAdders.Sum(x => x.Length).ToString())
                    }, "decode");
            }
        }

        /// <summary>
        /// Выполнение кернелов декодирования.
        /// </summary>
        /// <param name="coded">Множестов исходных сообщений.</param>
        /// <param name="syndrom">Множество синдромов.</param>
        public void Execute(CLCalc.Program.Variable coded, CLCalc.Program.Variable syndrom, CLCalc.Program.Variable diffrence)
        {
            for (int i = 0; i < _I; i++)
            {
                _decodeKernels[i].Kernel.Execute(new CLCalc.Program.Variable[] { coded, syndrom, diffrence }, _decodeWorkItemCount, _localWorkItemCount);
                CLCalc.Program.CommQueues[0].AddBarrier();
                CLCalc.Program.Sync();
            }
        }
    }
}
