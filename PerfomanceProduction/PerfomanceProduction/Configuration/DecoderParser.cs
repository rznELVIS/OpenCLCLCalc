using System;
using System.IO;

namespace PerfomanceProduction.Configuration
{
    /// <summary>
    /// Парсер настроект декодера МПД.
    /// </summary>
    public class DecoderParser
    {
        /// <summary>
        /// Параметр значений порогов.
        /// </summary>
        private const string T_MASK = "t";

        /// <summary>
        /// Парсинг настроек.
        /// </summary>
        /// <param name="path">Путь к файлу с настройками.</param>
        /// <param name="option">Опции моделирования.</param>
        public void Parse(string path, ModelOption option)
        {
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == T_MASK)
                        ParseThersholds(option, reader);
                }
            }
        }

        /// <summary>
        /// Парсинг значений порогов.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        /// <param name="reader">Ридер файла.</param>
        private void ParseThersholds(ModelOption option, StreamReader reader)
        {
            var values = reader.ReadLine().Split();

            option.Thersolds = new int[values.Length];
            for(var i = 0; i < values.Length; i++)
            {
                option.Thersolds[i] = Int32.Parse(values[i]);
            }
        }
    }
}
