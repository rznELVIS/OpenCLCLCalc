using System.Text;

namespace PerfomanceProduction.KernelProcess
{
    /// <summary>
    /// Заменяемый элемент шаблона.
    /// </summary>
    public class ReplaceItem
    {
        /// <summary>
        /// Заменяемая строка.
        /// </summary>
        private string _replaceableSource;

        /// <summary>
        /// Cтрока заменитель.
        /// </summary>
        private string _substitute;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="replaceable">Заменяемая строка.</param>
        /// <param name="substitute">Cтрока заменитель.</param>
        public ReplaceItem(string replaceable, string substitute)
        {
            _replaceableSource = replaceable;
            _substitute = substitute;
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="replaceable">Заменяемая строка.</param>
        /// <param name="substituteName">Название массива заменителя.</param>
        /// <param name="substitute">Массив заменитель.</param>
        public ReplaceItem(string replaceable, string substituteName, int[] substitute)
        {
            _replaceableSource = replaceable;
            _substitute = ToStringArray(substitute, substituteName);
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="replaceable">Заменяемая строка.</param>
        /// <param name="substituteName">Название массива заменителя.</param>
        /// <param name="substitute">Массив заменитель.</param>
        public ReplaceItem(string replaceable, string substituteName, int[][] substitute)
        {
            _replaceableSource = replaceable;
            _substitute = ToStringArray(substitute, substituteName);
        }

        /// <summary>
        /// Преобразовать массив в строку.
        /// </summary>
        /// <param name="array">Исходный массив.</param>
        /// <param name="alias">Имя массива в строке.</param>
        private string ToStringArray(int[] array, string alias)
        {
            var str = new StringBuilder();

            str.AppendFormat("ushort {0}[{1}];\r\n", alias, array.Length.ToString());

            for (var i = 0; i < array.Length; i++)
            {
                str.AppendFormat("{0}[{1}] = {2};\r\n", alias, i.ToString(), array[i].ToString());
            }

            return str.ToString();
        }

        /// <summary>
        /// Преобразовать массив в строку.
        /// </summary>
        /// <param name="array">Исходный массив.</param>
        /// <param name="alias">Имя массива в строке.</param>
        private string ToStringArray(int[][] array, string alias)
        {
            var str = new StringBuilder();

            str.AppendFormat("ushort {0}[{1}];\r\n", alias, array.Length.ToString());

            int index = 0;
            for (int j = 0; j < array.Length; j++)
            {
                for (var i = 0; i < array[j].Length; i++)
                {
                    str.AppendFormat("{0}[{1}] = {2};\r\n", alias, index.ToString(), array[j][i].ToString());
                    index++;
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// Заменяемая строка.
        /// </summary>
        public string Replaceable
        {
            get
            {
                return string.Format("[%{0}%]", _replaceableSource);
            }
        }

        /// <summary>
        /// Cтрока заменитель.
        /// </summary>
        public string Substitute
        {
            get
            {
                return _substitute;
            }
        }
    
    }
}
