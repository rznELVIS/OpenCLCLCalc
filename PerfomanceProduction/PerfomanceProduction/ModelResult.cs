using System.Collections.Generic;
using System.IO;

namespace PerfomanceProduction
{
    /// <summary>
    /// Результат моделирования одного эксперимента.
    /// </summary>
    public class ModelResultItem
    {
        /// <summary>
        /// Кол-во переданных байт.
        /// </summary>
        public long SendBites { get; set; }

        /// <summary>
        /// Bit error relation.
        /// </summary>
        public double BER { get; set; }

        /// <summary>
        /// Отношения сигнал/шум.
        /// </summary>
        public double EbN0 { get; set; }
    }

    /// <summary>
    /// Результат моделирования.
    /// </summary>
    public class ModelResult
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        public ModelResult()
        {
            Results = new List<ModelResultItem>();
        }

        /// <summary>
        /// Результаты моделирования при разлтчных значениях отношения сигнал/шум.
        /// </summary>
        public List<ModelResultItem> Results { get; set; }

        /// <summary>
        /// Сохранить результаты тестирования в файл.
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            using(var writer = new StreamWriter(path))
            {
                foreach(var item in Results)
                {
                    writer.WriteLine(string.Format("EbN0={1}, BER={0:0.##E+00}%, Передано бит={2}", item.BER, item.EbN0, item.SendBites));
                }
            }
        }
    }
}
