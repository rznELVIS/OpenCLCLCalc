using PerfomanceProduction;
using PerfomanceProduction.Configuration;
using System;

namespace ConsoleClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            var option = new ModelOption()
            {
                MathWait = 0
            };

            var openClParser = new OpenClConfigParser();
            openClParser.Parse(args[2], option);

            var coderParser = new CoderParser();
            coderParser.Parse(args[0], option);

            var decoderParser = new DecoderParser();
            decoderParser.Parse(args[1], option);

            option.ResultFilePath = args[3];

            var system = new ModelSystem();
            system.Model(option);

            Console.WriteLine("Моделирование закончено!");
            Console.ReadLine();
        }
    }
}
