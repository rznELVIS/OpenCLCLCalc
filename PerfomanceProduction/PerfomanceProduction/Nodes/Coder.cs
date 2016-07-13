using OpenCLTemplate;
using PerfomanceProduction.KernelProcess;
using System.Collections.Generic;

namespace PerfomanceProduction.Nodes
{
    /// <summary>
    /// Узел кодера.
    /// </summary>
    public class Coder
    {
        /// <summary>
        /// Кернел для заполнения информационной части кодового слова.
        /// </summary>
        private TemplateKernel _fillKernel;

        /// <summary>
        /// Кернелы для заполнений проверочной части кодового слова.
        /// </summary>
        private TemplateKernel[] _codeKernels;

        /// <summary>
        /// Число итераций для запуска кернела fillCoder.
        /// </summary>
        private int[] _fillWorkItemCount;

        /// <summary>
        /// Число итераций для запуска кернела code.
        /// </summary>
        private int[] _codeWorkItem;

        /// <summary>
        /// Число итераций для запуска кернела code.
        /// </summary>
        private int[] _codeWorkItemLack;

        /// <summary>
        /// Локальное число итераций для запуска.
        /// </summary>
        private int[] _localWorkItemCount;


        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public Coder(ModelOption option)
        {
            Init(option);
        }

        /// <summary>
        /// Инициализация функционала кодера.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public void Init(ModelOption option)
        {
            _fillWorkItemCount = new int[1] { option.GlobalWorkItemCount * option.M };
            _codeWorkItem = new int[1] { option.GlobalWorkItemCount * option.Branch };
            _localWorkItemCount = new int[1] { option.LocalWorkItemCount };

            _fillKernel = new TemplateKernel(
               ModelSystem.PATH + "fillCoder.cl",
                new List<ReplaceItem>() { },
                "fillCoder");

            _codeKernels = new TemplateKernel[option.UCount * option.VCount];
            int index = 0;
            for (var u = 0; u < option.UCount; u++)
            {
                for (int v = 0; v < option.VCount; v++)
                {
                    _codeKernels[index] = new TemplateKernel(
                        ModelSystem.PATH + "code.cl",
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
                        "code");
                    index++;
                }
            }
        }

        /// <summary>
        /// Выполнение кернелов кодирования.
        /// </summary>
        /// <param name="source">Множестов исходных сообщений.</param>
        /// <param name="coded">Множество кодовых слов.</param>
        public void Execute(CLCalc.Program.Variable source, CLCalc.Program.Variable coded)
        {
            _fillKernel.Kernel.Execute(new CLCalc.Program.Variable[] { source, coded }, _fillWorkItemCount, _localWorkItemCount);
            CLCalc.Program.Sync();

            for (var i = 0; i < _codeKernels.Length; i++)
            {
                _codeKernels[i].Kernel.Execute(new CLCalc.Program.Variable[] { source, coded }, _codeWorkItem, _localWorkItemCount);
                CLCalc.Program.Sync();
            }
            
        }
    }
}
