using Cloo;
using OpenCLTemplate;
using PerfomanceProduction.Nodes;
using PerfomanceProduction.Nodes.Utils;
using System;
using System.IO;
using System.Linq;

namespace PerfomanceProduction
{
    public class ModelSystem
    {
        public static string PATH = "";

        #region поля

        #region массивы

        /// <summary>
        /// Множестов исходных сообщений для моделирования.
        /// </summary>
        private int[] _source;

        /// <summary>
        /// Множество закодированнох сообщений.
        /// </summary>
        private int[] _coded;

        /// <summary>
        /// Множество синдромов.
        /// </summary>
        private int[] _syndrom;

        /// <summary>
        /// Множество разностных регистров.
        /// </summary>
        private int[] _diffrence;

        /// <summary>
        /// Мнржество згачений на ПЭ.
        /// </summary>
        private int[] _thershold;

        /// <summary>
        /// Результат моделирования.
        /// </summary>
        private int[] _result;

        /// <summary>
        /// Инициализация генератора случайных чисел.
        /// </summary>
        private int[] _seed;

        #endregion

        #region буферы

        /// <summary>
        /// Буффер для переменной <seealso cref="_source"/>
        /// </summary>
        private CLCalc.Program.Variable _sourceBuffer;

        /// <summary>
        /// Буффер для переменной <seealso cref="_coded"/>
        /// </summary>
        private CLCalc.Program.Variable _codedBuffer;

        /// <summary>
        /// Буффер для переменной <seealso cref="_syndrom"/>
        /// </summary>
        private CLCalc.Program.Variable _syndromBuffer;

        /// <summary>
        /// Буффер для переменной <seealso cref="_diffrence"/>
        /// </summary>
        private CLCalc.Program.Variable _diffrenceBuffer;

        /// <summary>
        /// Буффер для переменной <seealso cref="_thershold"/>
        /// </summary>
        private CLCalc.Program.Variable _thersholdBuffer;

        /// <summary>
        /// Буффер для переменной <seealso cref="_result"/>
        /// </summary>
        private CLCalc.Program.Variable _resultBuffer;

        /// <summary>
        /// Буффер для переменной <seealso cref="_seed"/>
        /// </summary>
        private CLCalc.Program.Variable _seedBuffer;

        #endregion

        #region узлы системы моделирования

        /// <summary>
        /// Источник данных.
        /// </summary>
        private Source _sourceNode;

        /// <summary>
        /// Кодер.
        /// </summary>
        private Coder _coderNode;

        /// <summary>
        /// Модулятор.
        /// </summary>
        private Modulator _modulateNode;

        /// <summary>
        /// Канал передачи данных.
        /// </summary>
        private Channel _channelNode;

        /// <summary>
        /// Демиодулятор.
        /// </summary>
        private Demodulator _demodulateNode;

        /// <summary>
        /// Синдром.
        /// </summary>
        private Syndrom _syndromNode;

        /// <summary>
        /// Декодер.
        /// </summary>
        private PerfomanceProduction.Nodes.Decoder _decodeNode;

        /// <summary>
        /// Приемник данных.
        /// </summary>
        private Receiver _recieverNode;

        /// <summary>
        /// Узел сброса значений.
        /// </summary>
        private Reset _resetNode;

        #endregion

        /// <summary>
        /// Генератор случайных чисел.
        /// </summary>
        private Random _random = new Random();

        #endregion

        /// <summary>
        /// Запустить общий процесс моделирования.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        public void Model(ModelOption option)
        {
            CLCalc.InitCL(ComputeDeviceTypes.Gpu);

            PATH = option.Path;

            InitArray(option);
            InitBuffers(option);
            InitNodes(option);

           // FillSource(source);

            var result = new ModelResult();
            for (var ebIndex = 0; ebIndex < option.EbValues.Count; ebIndex++)
            {
                result.Results.Add(ModelOneEb(option, ebIndex));                

                _result[0] = 0;
                _resultBuffer.WriteToDevice(_result, CLCalc.Program.CommQueues[0], true, null);
            }

            result.Save(option.ResultFilePath);

            CLCalc.DisableCL();
        }

        /// <summary>
        /// Инициализация массивов.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        private void InitArray(ModelOption option)
        {
            _source = new int[option.GlobalWorkItemCount * option.K];
            _coded = new int[option.GlobalWorkItemCount * option.N];
            _syndrom = new int[option.GlobalWorkItemCount * option.K];
            _diffrence = new int[option.GlobalWorkItemCount * option.M];
            _thershold = new int[option.GlobalWorkItemCount * option.M];
            _result = new int[1];
            _seed = new int[1];
        }

        /// <summary>
        /// Инициализация буфферов.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        private void InitBuffers(ModelOption option)
        {
            _sourceBuffer = new CLCalc.Program.Variable(_source);
            _sourceBuffer.WriteToDevice(_source, CLCalc.Program.CommQueues[0], true, null);

            _codedBuffer = new CLCalc.Program.Variable(_coded);
            _codedBuffer.WriteToDevice(_coded, CLCalc.Program.CommQueues[0], true, null);

            _syndromBuffer = new CLCalc.Program.Variable(_syndrom);
            _syndromBuffer.WriteToDevice(_syndrom, CLCalc.Program.CommQueues[0], true, null);

            _diffrenceBuffer = new CLCalc.Program.Variable(_diffrence);
            _diffrenceBuffer.WriteToDevice(_diffrence, CLCalc.Program.CommQueues[0], true, null);

            _thersholdBuffer = new CLCalc.Program.Variable(_thershold);
            _thersholdBuffer.WriteToDevice(_thershold, CLCalc.Program.CommQueues[0], true, null);

            _resultBuffer = new CLCalc.Program.Variable(_result);
            _resultBuffer.WriteToDevice(_result, CLCalc.Program.CommQueues[0], true, null);

            _seedBuffer = new CLCalc.Program.Variable(_seed);
        }

        /// <summary>
        /// Инициализация узлов системы моделирования.
        /// </summary>
        /// <param name="option">Опции моделирования.</param>
        private void InitNodes(ModelOption option)
        {
            _sourceNode = new Source(option, _random);
            _coderNode = new Coder(option);
            _modulateNode = new Modulator(option);
            _channelNode = new Channel(option);
            _demodulateNode = new Demodulator(option);
            _syndromNode = new Syndrom(option);
            _decodeNode = new PerfomanceProduction.Nodes.Decoder(option);
            _recieverNode = new Receiver(option);
            _resetNode = new Reset(option);
        }

        /// <summary>
        /// Выполнение одной итерации моделирования.
        /// </summary>
        private ModelResultItem ModelOneEb(ModelOption option, int ebIndex)
        {
            double ber = 0;
            DateTime start = DateTime.Now;

            for (var i = 0; i < option.Iteration; i++)
            {
                _seed[0] = _random.Next(Int16.MaxValue);
                _seedBuffer.WriteToDevice(_seed, CLCalc.Program.CommQueues[0], true, null);

                // генерирование сообщения
                _sourceNode.Execute(_sourceBuffer, _seedBuffer);

                // кодирование
                _coderNode.Execute(_sourceBuffer, _codedBuffer);

                // модуляция
                _modulateNode.Execute(_codedBuffer);

                // физический канал
                _channelNode.Execute(_codedBuffer, _seedBuffer, ebIndex);

                // демодуляция
                _demodulateNode.Execute(_codedBuffer);

                // вычисление синдрома
                _syndromNode.Execute(_codedBuffer, _syndromBuffer);

                // декодирвоание
                _decodeNode.Execute(_codedBuffer, _syndromBuffer, _diffrenceBuffer);

                // приемник данных.
                _recieverNode.Execute(_sourceBuffer, _codedBuffer, _resultBuffer);

                /*sourceVar.ReadFromDeviceTo(source, CLCalc.Program.CommQueues[0], true, null);
                codedVar.ReadFromDeviceTo(coded, CLCalc.Program.CommQueues[0], true, null);
                syndromVar.ReadFromDeviceTo(syndrom, CLCalc.Program.CommQueues[0], true, null);
                diffrenceVar.ReadFromDeviceTo(diffrence, CLCalc.Program.CommQueues[0], true, null);*/
                _resultBuffer.ReadFromDeviceTo(_result, CLCalc.Program.CommQueues[0], true, null);

                // Сбросить вспомогательные массивы.
                _resetNode.Execute(_syndromBuffer, _syndrom.Length);
                _resetNode.Execute(_codedBuffer, _coded.Length);
                _resetNode.Execute(_diffrenceBuffer, _diffrence.Length);

                ber += (double)_result[0] / (double)(option.GlobalWorkItemCount * option.K);
                _result[0] = 0;
                _resultBuffer.WriteToDevice(_result, CLCalc.Program.CommQueues[0], true, null);

                if (i % 5 == 0) Console.WriteLine(String.Format("{1}: секунды: {0} сек", (DateTime.Now - start).TotalSeconds, i));
            }

            ber /= option.Iteration;

            Console.WriteLine("Eb/N0={0}", option.EbValues[ebIndex].ToString("0.00"));
            Console.WriteLine("Бит: {0}, скорость: {1} кбит/с", option.TotalCount, option.TotalCount / 1000 / (DateTime.Now - start).TotalSeconds);
            Console.WriteLine("BER: {0}%", ber);

            return new ModelResultItem()
            {
                BER = ber,
                EbN0 = option.EbValues[ebIndex],
                SendBites = option.TotalCount
            };
        }

        protected static void FillSource(int[] source)
        {
            for (var i = 0; i < source.Length / 22; i++)
            {
                source[0 + i * 22] = 0;
                source[1 + i * 22] = 1;
                source[2 + i * 22] = 1;
                source[3 + i * 22] = 0;
                source[4 + i * 22] = 1;
                source[5 + i * 22] = 1;
                source[6 + i * 22] = 1;
                source[7 + i * 22] = 0;
                source[8 + i * 22] = 0;
                source[9 + i * 22] = 0;
                source[10 + i * 22] = 1;

                source[11 + i * 22] = 1;
                source[12 + i * 22] = 0;
                source[13 + i * 22] = 1;
                source[14 + i * 22] = 1;
                source[15 + i * 22] = 1;
                source[16 + i * 22] = 1;
                source[17 + i * 22] = 1;
                source[18 + i * 22] = 1;
                source[19 + i * 22] = 0;
                source[20 + i * 22] = 0;
                source[21 + i * 22] = 0;
            }
        }
    }
}
