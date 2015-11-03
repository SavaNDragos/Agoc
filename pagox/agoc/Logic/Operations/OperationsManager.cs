using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agoc.Logic.Types;

namespace Agoc.Logic.Operations
{
    public  class OperationsManager
    {
        public string EnvironmentConfigurationFile { get; set; }
        public string FragmentsConfigurationFile { get; set; }
        public string RulesBookFile { get; set; }
        public Dictionary<string, string> InitialEnvironmentDictionary { get; set; }
        private List<ProcessingOptions> InProcessingOptionses { get; set; } 
        public Dictionary<string,RulesBookRule> RulesBook { get; set; }
        public string PrintFileLocation { get; set; }

        public OperationsManager()
        {
            InProcessingOptionses = new List<ProcessingOptions>();
        }

        public void AddProcessingOption(ProcessingOptions inProcessingOption)
        {
            InProcessingOptionses.Add(inProcessingOption);
        }

        /// <summary> Extra comments
        /// Resolve operations needed to get resulted configuration file.
        /// </summary>
        public void Process()
        {
            //get environment definition
            var tempEnvioronmentDefinition = ExtractData.GetProperties(EnvironmentConfigurationFile, InitialEnvironmentDictionary, true);

            //get rules book if it was passed
            if (!string.IsNullOrWhiteSpace(RulesBookFile))
            {
                RulesBook = ExtractData.GetRulesBook(RulesBookFile);
            }

            var resultOfInterpretation = ExtractData.PublishConfiguration(FragmentsConfigurationFile,
                tempEnvioronmentDefinition, RulesBook);

            if (InProcessingOptionses.Contains(ProcessingOptions.PrintFile))
            {
                if (string.IsNullOrWhiteSpace(PrintFileLocation))
                {
                    PrintFileLocation = Directory.GetCurrentDirectory() + "/GeneratedFile" +
                                             DateTime.Now.ToString("YY-MM-DD") + ".xml";
                }

                ExtractData.PublishConfigurationFile(resultOfInterpretation, PrintFileLocation);
            }

            if (InProcessingOptionses.Contains(ProcessingOptions.PrintConsole))
            {
                ExtractData.PublishConfigurationInConsole(resultOfInterpretation);
            }
        }
    }
}
