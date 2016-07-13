using OpenCLTemplate;
using System.Collections.Generic;
using System.IO;

namespace PerfomanceProduction.KernelProcess
{
    /// <summary>
    /// Кернел, чей код сформирован с заменой шаблонов..
    /// </summary>
    public class TemplateKernel
    {
        /// <summary>
        /// Код kernel.
        /// </summary>
        private string _text;

        /// <summary>
        /// Исполняемый кернел.
        /// </summary>
        private CLCalc.Program.Kernel _kernel;

        /// <summary>
        /// Контсруктор.
        /// </summary>
        public TemplateKernel(string path, List<ReplaceItem> items, string name)
        {
            Update(path, items, name);
        }

        /// <summary>
        /// Обновить тект кернела.
        /// </summary>
        /// <param name="path">Путь к файлу к кодом кернела.</param>
        /// <param name="items">Элементы для замены.</param>
        public void Update(string path, List<ReplaceItem> items, string name)
        {
            _text = GetKernel(path);
            Replace(items);
            _kernel = Compile(name);
        }

        /// <summary>
        /// Скомпилировать код кернела.
        /// </summary>
        /// <param name="name">Название кернеал.</param>
        public CLCalc.Program.Kernel Compile(string name)
        {
            CLCalc.Program.Compile(new string[] { _text });

            return new CLCalc.Program.Kernel(name);
        }

        /// <summary>
        /// Загрукза файла с бинарниками из файла.
        /// </summary>
        /// <param name="path">Путь к файлу.</param>
        /// <returns>Код kernel.</returns>
        private string GetKernel(string path)
        {
            var res = string.Empty;

            using (var reader = new StreamReader(path))
            {
                res = reader.ReadToEnd();
            }

            return res;
        }

        /// <summary>
        /// Замена шаблонных параметров.
        /// </summary>
        /// <param name="items">Шаблонные параметры.</param>
        private void Replace(List<ReplaceItem> items)
        {
            foreach(var item in items)
            {
                Replace(item);
            }
        }

        /// <summary>
        /// Заменна шаблонного параметра.
        /// </summary>
        /// <param name="item">Шаблонный параметр.</param>
        private void Replace(ReplaceItem item)
        {
            _text = _text.Replace(item.Replaceable, item.Substitute);
        }

        public CLCalc.Program.Kernel Kernel
        {
            get
            {
                return _kernel;
            }
        }
    }
}
