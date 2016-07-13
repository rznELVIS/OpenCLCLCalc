using OpenCLTemplate;
using PerfomanceProduction.KernelProcess;
using System.Collections.Generic;

namespace PerfomanceProduction.Nodes
{
    /// <summary>
    /// Узел декодера - вычисление синдрома.
    /// </summary>
    public class Syndrom
    {
        /// <summary>
        /// Кернел для заполнения Синдрома проверочными битами кодовго слова.
        /// </summary>
        private TemplateKernel _fillKernel;

        /// <summary>
        /// Кернелы для повторения процесса кодирвоания.
        /// </summary>
        private TemplateKernel[] _syndromKernels;

        /// <summary>
        /// Число итераций для запуска кернела fillSyndrom.
        /// </summary>
        private int[] _fillWorkItemCount;

        /// <summary>
        /// Число итераций для запуска кернела syndrom.
        /// </summary>
        private int[] _syndromWorkItem;

        /// <summary>
        /// Локальное число итераций для запуска.
        /// </summary>
        private int[] _localWorkItemCount;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public Syndrom(ModelOption option)
        {
            Init(option);
        }

        /// <summary>
        /// Инициализация функционала синдрома.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public void Init(ModelOption option)
        {
            _fillWorkItemCount = new int[1] { option.GlobalWorkItemCount * option.K };
            _syndromWorkItem = new int[1] { option.GlobalWorkItemCount * option.Branch };
            _localWorkItemCount = new int[1] { option.LocalWorkItemCount };

            _fillKernel = new TemplateKernel(
                ModelSystem.PATH + "fillSyndrom.cl",
                new List<ReplaceItem>() 
                {
                    new ReplaceItem("start", (option.K * option.GlobalWorkItemCount).ToString())
                },
                "fillSyndrom");

            _syndromKernels = new TemplateKernel[option.UCount * option.VCount];
            int index = 0;
            for (var u = 0; u < option.UCount; u++)
            {
                for (int v = 0; v < option.VCount; v++)
                {
                    _syndromKernels[index] = new TemplateKernel(
                        ModelSystem.PATH + "syndrom.cl",
                        new List<ReplaceItem>()
                                {
                                    new ReplaceItem("branch", option.Branch.ToString()),
                                    new ReplaceItem("start", (option.K * option.GlobalWorkItemCount).ToString()),
                                    new ReplaceItem("v", v.ToString()),
                                    new ReplaceItem("u", u.ToString()),
                                    new ReplaceItem("k", option.K.ToString()),
                                    new ReplaceItem("adderCount", option.CoderAdders[v][u].Length.ToString()),
                                    new ReplaceItem("adders", "adders", option.CoderAdders[v][u]),
                                },
                        "syndrom");
                    index++;
                }
            }
        }

        /// <summary>
        /// Выполнение кернелов вычисления синдрома.
        /// </summary>
        /// <param name="coded">Множестов исходных сообщений.</param>
        /// <param name="syndrom">Множество синдромов.</param>
        public void Execute(CLCalc.Program.Variable coded, CLCalc.Program.Variable syndrom)
        {
            _fillKernel.Kernel.Execute(new CLCalc.Program.Variable[] { coded, syndrom }, _fillWorkItemCount, _localWorkItemCount);
            CLCalc.Program.Sync();

            for (var i = 0; i < _syndromKernels.Length; i++)
            {
                _syndromKernels[i].Kernel.Execute(new CLCalc.Program.Variable[] { coded, syndrom }, _syndromWorkItem, _localWorkItemCount);
                CLCalc.Program.Sync();
            }
        }
    }
}
