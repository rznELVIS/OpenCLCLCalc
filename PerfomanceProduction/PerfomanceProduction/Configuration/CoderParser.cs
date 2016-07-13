using System;
using System.Globalization;
using System.IO;

namespace PerfomanceProduction.Configuration
{
    /// <summary>
    /// Пасрер настоек кодера МПД.
    /// </summary>
    public class CoderParser
    {
        /// <summary>
        /// Параметр числа проверочных ветвей.
        /// </summary>
        private const string NR_MASK = "nr=";

        /// <summary>
        /// Параметр числа информационных ветвей.
        /// </summary>
        private const string NK_MASK = "nk=";

        /// <summary>
        /// Параметр индекса проверочной ветви.
        /// </summary>
        private const string V_MASK = "v=";

        /// <summary>
        /// Параметр длины ветви.
        /// </summary>
        private const string BRANCH_MASK = "branch=";

        /// <summary>
        /// Параметр индекса информационной ветви.
        /// </summary>
        private const string U_MASK = "i=";

        /// <summary>
        /// Позиция скоторой начинаются параметы кодера в файле конфигураций.
        /// </summary>
        private const int СODER_START_POSITION = 3;

        /// <summary>
        /// Разбор настроек МПД.
        /// </summary>
        /// <param name="path">Путь к файлу с натсройками.</param>
        /// <param name="option">Опции моделирования, которые должны быть заполнены.</param>
        public void Parse(string path, ModelOption option)
        {
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(NR_MASK) && line.Contains(NK_MASK))
                        ParseCoder(line, reader, option);

                    if (line.Contains(BRANCH_MASK))
                        option.Branch = GetValueFromString(line);
                }
            }
        }

        /// <summary>
        /// Распарсить параметры кодера.
        /// </summary>
        /// <param name="line">Первая строка с конфигурацией кодера.</param>
        /// <param name="reader">Ридер файла.</param>
        /// <param name="option">Опции моделирования.</param>
        private void ParseCoder(string line, StreamReader reader, ModelOption option)
        {
            int nr, nk;
            GetNrAndNk(line, out nr, out nk);

            var coderAdders = new int[nr][][];
            for (int i = 0; i < nr; i++)
            {
                coderAdders[i] = new int[nk][];
            }

            for (int i = 0; i < nr * nk; i++)
            {
                string str = reader.ReadLine();
                if (str != null)
                {
                    ParseCoderLine(coderAdders, str);
                }
            }

            option.VCount = nr;
            option.UCount = nk;
            option.CoderAdders = coderAdders;
        }

        /// <summary>
        /// Получить значения информационной и проверочной ветви.
        /// </summary>
        /// <param name="line">Строка со значениями инф-ой и проверочной ветви.</param>
        /// <param name="nk">Число информационных ветвей.</param>
        /// <param name="nr">Число проверочных ветвей.</param>
        private void GetNrAndNk(string line, out int nk, out int nr)
        {
            var parameters = line.Split();

            nk = nr = -1;
            foreach(var parameter in parameters)
            {
                if (parameter.Contains(NR_MASK))
                    nr = GetValueFromString(parameter);

                if (parameter.Contains(NK_MASK))
                    nk = GetValueFromString(parameter);
            }
        }

        /// <summary>
        /// Парсить строку с конфигурациями кодера.
        /// </summary>
        /// <param name="coder">Конфигурация кодера.</param>
        /// <param name="line">Строка с параметрами кодера.</param>
        private void ParseCoderLine(int[][][] coder, string line)
        {
            int v, u;

            var parameters = line.Split();
            u = v = -1;
            foreach(var parameter in parameters)
            {
                if (parameter.Contains(U_MASK))
                    u = GetValueFromString(parameter);

                if (parameter.Contains(V_MASK))
                    v = GetValueFromString(parameter);
            }

            if (u != -1 && v != -1)
            {
                int length = GetValueFromString(parameters[2]);
                coder[v][u] = new int[length];
                for (int i = СODER_START_POSITION; i < parameters.Length; i++)
                {
                    coder[v][u][i - СODER_START_POSITION] = GetValueFromString(parameters[i]);
                }
            }
        }

        /// <summary>
        /// Получить занчение параметра.
        /// </summary>
        /// <param name="parameter">Парметр в строке вида [name]=[value]</param>
        /// <returns>Значение параметра.</returns>
        private int GetValueFromString(string parameter)
        {
            var strValue = parameter.Substring(parameter.IndexOf("=") + 1);
            return Int32.Parse(strValue);
        }

        /// <summary>
        /// Получить занчение параметра.
        /// </summary>
        /// <param name="parameter">Парметр в строке вида [name]=[value]</param>
        /// <returns>Значение параметра.</returns>
        private double GetDoubleValueFromString(string parameter)
        {
            var strValue = parameter.Substring(parameter.IndexOf("=") + 1);
            return Double.Parse(strValue, CultureInfo.InvariantCulture);
        }
    }
}
