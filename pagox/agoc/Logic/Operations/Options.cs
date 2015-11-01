using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Agoc.Logic.Operations
{
    internal class Options
    {
        [Option('e', "environment", Required = true,
            HelpText = "Xml that contains environment definition.")]
        public string EnvironmentXml { get; set; }

        // omitting long name, default --verbose
        [Option('s', "step", Required = true,
            HelpText = "Xml that contains the properties/steps definitian.")]
        public string StepsXml { get; set; }

        [Option('p', "environmentParameters", Required = false, HelpText = "Pass extra environment variables.")]
        public string EnvironmentParameters { get; set; }

        [Option('f', "printInFile", Required = false, DefaultValue = true,
            HelpText = "Xml that contains the properties/steps definitian.")]
        public bool PrintInFile { get; set; }

        [Option('c', "printInConsole", Required = false, DefaultValue = false,
            HelpText = "Xml that contains the properties/steps definitian.")]
        public bool PrintInConsole { get; set; }

        [Option('l', "printFileLocation", Required = false,
            HelpText = "Xml that contains the properties/steps definitian.")]
        public string PrintFileLocation { get; set; }

    }
}

