﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agoc.Logic.Operations
{
    public  class OperationsManager
    {
        public string EnvironmentConfigurationFile { get; set; }
        public string FragmentsConfigurationFile { get; set; }
        public Dictionary<string, string> InitialEnvironmentDictionary { get; set; }
        private List<ProcessingOptions> InProcessingOptionses { get; set; } 
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
            var tempEnvioronmentDefinition = ExtractData.GetProperties(EnvironmentConfigurationFile, InitialEnvironmentDictionary, true);
            var resultOfInterpretation = ExtractData.PublishConfiguration(FragmentsConfigurationFile, tempEnvioronmentDefinition);

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
