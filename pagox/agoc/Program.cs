using Agoc.Logic.Operations;
using fagox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agoc
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //parse commandLine
            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine("Incorect usage of commandline parameters. Please review.");
            }

            //get environment properties passed from command line
            var initialEnvironment = OptionsProcessor.GetEnvironmentFromParameters(options.EnvironmentParameters);
            
            var manager = new OperationsManager()
            {
                InitialEnvironmentDictionary = initialEnvironment,
                EnvironmentConfigurationFile = options.EnvironmentXml,
                FragmentsConfigurationFile = options.StepsXml,
                PrintFileLocation =  options.PrintFileLocation,
            };

            //get output method
            {
                if (options.PrintInFile)
                {
                    manager.AddProcessingOption(ProcessingOptions.PrintFile);
                }

                if (options.PrintInConsole)
                {
                    manager.AddProcessingOption(ProcessingOptions.PrintConsole);
                }
            }

            try
            {
                manager.Process();
                Console.WriteLine("Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("We have encounter the following issues when provessing: {0} ",
                    ex.Message));
            }
        }
    }
}
