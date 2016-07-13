using System;
using System.Globalization;
using System.IO;

namespace PerfomanceProduction.Configuration
{
    /// <summary>
    /// Парсер файла с настройками OpenCL.
    /// </summary>
    public class OpenClConfigParser
    {
        private const string LOCAL_WORK_ITEM_COUNT = "LocalWorkItemCount";
        private const string GLOBAL_WORK_ITEM_COEFF = "GlobalWorkItemCoeff";
        private const string ITERATION = "Iteration";
        private const string PATH = "Path";
        private const string EB_N0_START = "EbN0Start";
        private const string EB_N0_END = "EbN0End";
        private const string EB_N0_STEP = "EbN0Step";

        /// <summary>
        /// Парс настроек OpenCL.
        /// </summary>
        /// <param name="path">Путь к файлу с настройками.</param>
        /// <param name="option">Опции моделирвоания, которые необзодимо запонлнить.</param>
        public void Parse(string path, ModelOption option)
        {
            using(var reader = new StreamReader(path))
            {
                double ebn0Start, ebn0End, ebN0Step;

                ebn0Start = ebn0End = ebN0Step = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(LOCAL_WORK_ITEM_COUNT))
                        option.LocalWorkItemCount = Int32.Parse(line.Substring(line.IndexOf("=") + 1));

                    if (line.Contains(GLOBAL_WORK_ITEM_COEFF))
                        option.GlobalWorkItemCoeff = Int32.Parse(line.Substring(line.IndexOf("=") + 1));

                    if (line.Contains(ITERATION))
                        option.Iteration = Int32.Parse(line.Substring(line.IndexOf("=") + 1));

                    if (line.Contains(PATH))
                        option.Path = line.Substring(line.IndexOf("=") + 1);

                    if (line.Contains(EB_N0_START))
                        ebn0Start = Double.Parse(line.Substring(line.IndexOf("=") + 1), CultureInfo.InvariantCulture);

                    if (line.Contains(EB_N0_END))
                        ebn0End = Double.Parse(line.Substring(line.IndexOf("=") + 1), CultureInfo.InvariantCulture);

                    if (line.Contains(EB_N0_STEP))
                        ebN0Step = Double.Parse(line.Substring(line.IndexOf("=") + 1), CultureInfo.InvariantCulture);
                }

                for (; ebn0Start <= ebn0End; ebn0Start += ebN0Step)
                {
                    option.EbValues.Add(ebn0Start);
                }
            }
        }
    }
}
