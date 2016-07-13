using System;
using System.Collections.Generic;

namespace PerfomanceProduction
{
    /// <summary>
    /// Параметры моделирования СПЦД с МПД.
    /// </summary>
    public class ModelOption
    {
        #region поля

        #region конфигурации МПД

        /// <summary>
        /// Адреса ячееек регистров информационных ветвей, участвующих в формирование проверочной ветви.
        /// Первое измерение - индекс проверочной ветви (v).
        /// Второе измерение - инлекс информационной ветви (u).
        /// Третье измерение - индекс ячейки ригистра информационной ветви (u) для формирования проверочной ветви (v).
        /// </summary>
        private int[][][] _coderAdders;

        /// <summary>
        /// Адреса ячееек регистров проверочных ветвей, участвующих в вычисление значения на пороговом элементе.
        /// Первое измерение - инлекс информационной ветви (u).
        /// Второе измерение -индекс ячейки ригистра проверочной ветви (v) для порогового значения для декодирования информационного бита (v).
        /// </summary>
        private int[][] _decodedAdders;

        #endregion

        #endregion

        public ModelOption()
        {
            EbValues = new List<double>();
        }

        public string Path { get; set; }

        /// <summary>
        /// Установка значения decodedAdders.
        /// </summary>
        private void SetDecoderAdders(int[][][] coderAdders)
        {
            _decodedAdders = new int[UCount][];
            for (var u = 0; u < UCount; u++)
            {
                var decoders = new List<int>();
                for (var v = 0; v < VCount; v++)
                {
                    for(var i = 0; i < _coderAdders[v][u].Length; i++)
                    {
                        decoders.Add(_coderAdders[v][u][i] + v * Branch);
                    }

                    _decodedAdders[u] = decoders.ToArray();
                }
            }
        }

        /// <summary>
        /// Получение отношение сигнал/шум насообщение в канале.
        /// </summary>
        private double GetEs(double eb)
        {
            return eb + 10 * Math.Log10(R);
        }

        /// <summary>
        /// Получение средеквадратичное отклонение.
        /// </summary>
        public double GetSigma(double eb)
        {
            return 1 / (2 * Math.Pow(10, GetEs(eb) / 10.0));
        }

        #region свойства

        #region свойства канала передеачи данных

        /// <summary>
        /// Различные значения отношений сигнал/шум для моделирования.
        /// </summary>
        public List<double> EbValues { get; set; }

        /// <summary>
        /// Математическое ожидание нормального распеделения белого гаусовского шума.
        /// </summary>
        public double MathWait { get; set; }

        #endregion

        /// <summary>
        /// Кол-во используемых на GPU PE "за один раз".
        /// </summary>
        public int LocalWorkItemCount { get; set; }

        /// <summary>
        /// Кратность объема используемых на GPU PE.
        /// </summary>
        public int GlobalWorkItemCoeff { get; set; }

        /// <summary>
        /// Кол-во повторений экспермента на GPU.
        /// </summary>
        public int Iteration { get; set; }

        /// <summary>
        /// Адреса ячееек регистров информационных ветвей, участвующих в формирование проверочной ветви.
        /// Первое измерение - индекс проверочной ветви (v).
        /// Второе измерение - инлекс информационной ветви (u).
        /// Третье измерение - индекс ячейки ригистра информационной ветви (u) для формирования проверочной ветви (v).
        /// </summary>
        public int[][][] CoderAdders
        {
            get
            {
                return _coderAdders;
            }
            set
            {
                _coderAdders = value;
                SetDecoderAdders(value);
            }
        }

        /// <summary>
        /// Адреса ячееек регистров проверочных ветвей, участвующих в вычисление значения на пороговом элементе.
        /// Первое измерение - инлекс информационной ветви (u).
        /// Второе измерение - индекс проверочной ветви (v).
        /// Третье измерение - индекс ячейки ригистра проверочной ветви (v) для порогового значения для декодирования информационного бита (v).
        /// </summary>
        public int[][] DecodedAdders
        {
            get
            {
                return _decodedAdders;
            }
        }

        /// <summary>
        /// Общее кол-во используемых на GPU PE.
        /// </summary>
        public int GlobalWorkItemCount
        {
            get
            {
                return LocalWorkItemCount * GlobalWorkItemCoeff;
            }
        }

        public long TotalCount
        {

            get
            {
                return (long)LocalWorkItemCount * (long)GlobalWorkItemCoeff * (long)K * (long)Iteration;
            }
        
        }

        /// <summary>
        /// Кол-во значений загружаемых за раз.
        /// </summary>
        public int Coalesce { get; set; }

        /// <summary>
        /// Остаток ветви.
        /// </summary>
        public int Lack
        {
            get
            {
                return Branch % Coalesce;
            }
        }

        /// <summary>
        /// Число загрузок для использования brach.
        /// </summary>
        public int CoalesceBranch
        {
            get
            {
                return Branch / Coalesce;
            }
        }

        #region конфигурации МПД

        /// <summary>
        /// Длина проверочной/информационной ветви.
        /// </summary>
        public int Branch { get; set; }

        /// <summary>
        /// Кол-во информацилнных ветвей.
        /// </summary>
        public int UCount  { get; set; }

        /// <summary>
        /// Кол-во проверочных ветвей.
        /// </summary>
        public int VCount  { get; set; }

        /// <summary>
        /// Значений порогов на ПЭ. Число элементов в массиве, это число какадов декодирования.
        /// </summary>
        public int[] Thersolds { get; set; }

        /// <summary>
        /// Длина информационной части кодовго слова.
        /// </summary>
        public int K
        {
            get
            {
                return Branch * UCount;
            }
        }

        /// <summary>
        /// Длина проверочной части кодового слова.
        /// </summary>
        public int M
        {
            get
            {
                return Branch * VCount;
            }
        }

        /// <summary>
        /// Длина кодовго слова.
        /// </summary>
        public int N
        {
            get
            {
                return Branch * (UCount + VCount);
            }
        }

        /// <summary>
        /// Кодовая скорость.
        /// </summary>
        public double R
        {
            get
            {
                return (double)UCount / (double)(UCount + VCount);
            }
        }

        #endregion

        /// <summary>
        /// Путь к файлу с результатми файла.
        /// </summary>
        public string ResultFilePath { get; set; }

        #endregion
    }
}
