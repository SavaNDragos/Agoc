using Agoc.Logic.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Agoc.Logic.Operations
{
    public static class ExtractData
    {
        /// <summary>
        /// We will extract all the environments variable from the environment definitian files.
        /// Note: This is a recursive method.
        /// </summary>
        /// <param name="inEnvironmentFileLocation"></param>
        /// <param name="isLastNode"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetProperties (string inEnvironmentFileLocation, Dictionary<string,string> inPreviousValues , bool isLastNode)
        {
            var forReturn = new Dictionary<string, string>();
            var ourValues = new Dictionary<string, string>();

            //sometimes we will end up with cases where we can't detect the correct 
            var resolvedEnvironmentFileLocation = inEnvironmentFileLocation;
            if (resolvedEnvironmentFileLocation.Contains("${"))
            {
                resolvedEnvironmentFileLocation = ResolveFragment(resolvedEnvironmentFileLocation, inPreviousValues);
            }

            FileStream textInFile;
            if (File.Exists(resolvedEnvironmentFileLocation))
            {
                textInFile = new FileStream(resolvedEnvironmentFileLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                throw new FileNotFoundException(string.Format("File {0} wasn't found.Please verify the commandLineProperties.", resolvedEnvironmentFileLocation));
            }

            //FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
            TextReader reader = new StreamReader(textInFile);

            var SerializerObj = new XmlSerializer(typeof(EnvironmentDef)); // we will do an xml serialization
            var tempEnvironmentDef = (EnvironmentDef)SerializerObj.Deserialize(reader);

            //include new values in dictionary
            foreach (var environmentDefItem in tempEnvironmentDef.Items)
            {
                if (environmentDefItem is EnvironmentDefProperty)
                {
                    var environmentProperty = environmentDefItem as EnvironmentDefProperty;
                    if (ourValues.ContainsKey(environmentProperty.Name))
                    {
                        ourValues[environmentProperty.Name] = environmentProperty.Value;
                    }
                    else
                    {
                        ourValues.Add(environmentProperty.Name, environmentProperty.Value);
                    }
                }
            }

            //add or override with outside values
            if (isLastNode)
            {
                foreach (var inPreviousValue in inPreviousValues)
                {
                    if (ourValues.ContainsKey(inPreviousValue.Key))
                    {
                        ourValues[inPreviousValue.Key] = inPreviousValue.Value;
                    }
                    else
                    {
                        ourValues.Add(inPreviousValue.Key, inPreviousValue.Value);
                    }
                }
            }

            foreach (var environmentDefItem in tempEnvironmentDef.Items.Reverse())
            {
                if (environmentDefItem is EnvironmentDefImport)
                {
                    var environmentProperty = environmentDefItem as EnvironmentDefImport;
                    
                    //add new values found
                    foreach (var iterValueFound in ourValues)
                    {
                        if (!ourValues.ContainsKey(iterValueFound.Key))
                        {
                            inPreviousValues.Add(iterValueFound.Key, iterValueFound.Value);
                        }
                    }

                    foreach (var newValue in GetProperties(inEnvironmentFileLocation + @"/" + environmentProperty.FileLocation, inPreviousValues, false))
                    {
                        //if it is coming from a top node it is clear that it should have been overriden
                        if (!ourValues.ContainsKey(newValue.Key))
                        {
                            ourValues.Add(newValue.Key, newValue.Value);
                        }
                    }
                }
            }

            if (isLastNode)
            {
                //resolve values
                var shouldContinue = true;
                while (shouldContinue)
                {
                    shouldContinue = false;
                    foreach (var iterKey in GetListOfKeys(ourValues))
                    {
                        if (ourValues[iterKey].Contains("${"))
                        {
                            var tempPartialSubstitues = GetValuesToBeProcessed(ourValues[iterKey]);


                            var newValue = ourValues[iterKey];
                            //search for value
                            foreach (var iterValue in tempPartialSubstitues)
                            {
                                if (ourValues.ContainsKey(iterValue.Substring(2, iterValue.Length - 3)))
                                {
                                    newValue = newValue.Replace(iterValue, ourValues[iterValue.Substring(2, iterValue.Length - 3)]);
                                    shouldContinue = true;
                                }
                            }

                            //update dictionary
                            ourValues[iterKey] = newValue;
                        }
                    }
                }
            }

            return ourValues;
        }

        /// <summary>
        /// We get a list of values we need to substitute;
        /// </summary>
        /// <param name="inValue">Value of property value in which we need to search for substitute values.</param>
        /// <returns>A list of values that needs to be substituted.</returns>
        public static List<string> GetValuesToBeProcessed(string inValue)
        {
            var forReturn = new List<string>();
            var encounters = new List<int>();
            var secondEncounters = new List<int>();

            for (int i = 0; i <= inValue.Length - 1; i++)
            {
                if (inValue[i] == '$' && inValue[i + 1] == '{')
                {
                    encounters.Add(i);
                }
                else if (inValue[i] == '}')
                {
                    secondEncounters.Add(i);
                }
            }

            //try to get values we can process
            var result = new Dictionary<int, int>();

            var j = 0;
            for (int i = 0; i <= encounters.Count - 1; i++)
            {
                if (encounters[i] < secondEncounters[j])
                {
                    if ((i == encounters.Count - 1) || encounters[i + 1] > secondEncounters[j])
                    {
                        //add pair
                        result.Add(encounters[i], secondEncounters[j]);
                        j = -j + 1;
                    }
                    else
                    {
                        j = -j + 1;
                    }
                }
            }

            foreach (var iterKey in result)
            {
                forReturn.Add("${" + inValue.Substring(iterKey.Key + 2, iterKey.Value - iterKey.Key - 2) + "}");
            }

            return forReturn;
        }

        /// <summary>
        /// Get a list with all the keys a dictionary contains.
        /// </summary>
        /// <param name="inDictionary">The dictionary from which we want to obtain a list of its keys.</param>
        /// <returns>A list of keys.</returns>
        public static List<string> GetListOfKeys (Dictionary<string,string> inDictionary)
        {
            return inDictionary.Select(obj => obj.Key).ToList();
        }

        /// <summary>
        /// Get configuration from file.
        /// </summary>
        /// <param name="inConfigurationFileLocation"></param>
        /// <param name="inEnvironmentDictionary"></param>
        /// <param name="isLastNode"></param>
        /// <returns></returns>
        public static List<string> GetConfiguration(string inConfigurationFileLocation,
            Dictionary<string, string> inEnvironmentDictionary, bool isLastNode)
        {
            var configFragmentList = new List<string>();
            var resolvedFragmentList = new List<string>();

            //sometimes we will end up with cases where we can't detect the correct 
            var resolvedConfigurationFileLocation = inConfigurationFileLocation;
            if (inConfigurationFileLocation.Contains("${"))
            {
                resolvedConfigurationFileLocation = ResolveFragment(inConfigurationFileLocation,inEnvironmentDictionary);
            }

            var isFileFound = true;
            FileStream textInFile=null;
            if (File.Exists(resolvedConfigurationFileLocation))
            {
                textInFile = new FileStream(resolvedConfigurationFileLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                isFileFound = false;
            }

            //in case the file isn't found we should throw an exception
            if (!isFileFound)
            {
                throw new FileNotFoundException(
                    string.Format("File {0} wasn't found.Please verify the commandLineProperties.",
                        inConfigurationFileLocation));
            }


            //FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
            TextReader reader = new StreamReader(textInFile);

            XmlSerializer SerializerObj = new XmlSerializer(typeof (ConfigurationFrag));
            // we will do an xml serialization
            ConfigurationFrag tempConfigurationFrag = (ConfigurationFrag) SerializerObj.Deserialize(reader);

            foreach (ConfigurationFragFragment iterConfigurationFrag in tempConfigurationFrag.Fragment)
            {
                if (string.IsNullOrWhiteSpace(iterConfigurationFrag.ImportConfigurationFrag))
                {
                    configFragmentList.Add(iterConfigurationFrag.Value);
                }
                else
                {
                    //we need to be sure that the File We are importing is right
                    var tempExtraConfigFragmentFileName = ResolveFragment(
                        iterConfigurationFrag.ImportConfigurationFrag, inEnvironmentDictionary);

                    configFragmentList.AddRange(
                        GetConfiguration(
                            inConfigurationFileLocation + "//" + tempExtraConfigFragmentFileName,
                            inEnvironmentDictionary, false));
                }
            }

            //we should resolve the environment values 
            if (isLastNode)
            {
                resolvedFragmentList.AddRange(configFragmentList.Select(iterConfigFragment => ResolveFragment(iterConfigFragment, inEnvironmentDictionary)));
                return resolvedFragmentList;
            }

            return configFragmentList;
        }

        /// <summary>
        /// Get string for configuration file passed.
        /// </summary>
        /// <param name="configurationFileLocation"></param>
        /// <param name="inEnvironmentDictionary"></param>
        /// <returns>The string specific for yje configuration file that was passed</returns>
        public static string PublishConfiguration(string configurationFileLocation,
            Dictionary<string, string> inEnvironmentDictionary)
        {
            var resultOfInterpretation = GetConfiguration(configurationFileLocation, inEnvironmentDictionary, true);
            return resultOfInterpretation.Aggregate(string.Empty, (current, configFragment) => current + configFragment);
        }

        /// <summary>
        /// Published resulted configuration in a file
        /// </summary>
        /// <param name="inOutFileLocation"></param>
        /// <param name="inOutText"></param>
        public static void PublishConfigurationFile(string inOutText,string inOutFileLocation)
        {
            if (!File.Exists(inOutFileLocation))
            {
              var result =  File.Create(inOutFileLocation);
                result.Close();
            }

            using (TextWriter tw = new StreamWriter(inOutFileLocation))
            {
                tw.WriteLine(inOutText);
            }
        }

        /// <summary>
        /// Print result of configuration interpretation in console. 
        /// </summary>
        /// <param name="inOutText">Output text.</param>
        public static void PublishConfigurationInConsole(string inOutText)
        {
            Console.WriteLine("The resolved configuration file:");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(inOutText);
            Console.WriteLine(Environment.NewLine);
        }

        public static string ResolveFragment(string inFragment, Dictionary<string, string> inEnvironmentDictionary)
        {
            var changesFound = true;

            while (changesFound)
            {
                var forReturn = new List<string>();
                var encounters = new List<int>();
                var secondEncounters = new List<int>();

                for (int i = 0; i <= inFragment.Length - 1; i++)
                {
                    if (inFragment[i] == '$' && inFragment[i + 1] == '{')
                        encounters.Add(i);
                    else if (inFragment[i] == '}')
                        secondEncounters.Add(i);
                }

                var result = new Dictionary<int, int>();

                //try to get values we can process
                var j = 0;
                for (int i = 0; i <= encounters.Count - 1; i++)
                {
                    if (encounters[i] < secondEncounters[j])
                    {
                        if ((i == encounters.Count - 1) || encounters[i + 1] > secondEncounters[j])
                        {
                            //add pair
                            result.Add(encounters[i], secondEncounters[j]);
                            j = j + 1;
                        }
                        else
                        {
                            j = j + 1;
                        }
                    }
                }

                foreach (var iterKey in result)
                {
                    forReturn.Add("${" +
                                  inFragment.Substring(iterKey.Key + 2, iterKey.Value - iterKey.Key - 2) +
                                  "}");
                }

                //get keys
                var tempListOfKeys = inEnvironmentDictionary.Keys.ToList();
                forReturn.RemoveAll(obj => !tempListOfKeys.Contains(obj.Substring(2, obj.Length - 3)));

                var newConfigValue = inFragment;
                //start to replace the environment values we found
                foreach (var iterValueForReplace in forReturn)
                {
                    if (inEnvironmentDictionary.ContainsKey(iterValueForReplace.Substring(2, iterValueForReplace.Length - 3)))
                    {
                        newConfigValue = inFragment.Replace(iterValueForReplace,
                            inEnvironmentDictionary[iterValueForReplace.Substring(2, iterValueForReplace.Length - 3)]);
                    }
                }
                if (newConfigValue == inFragment)
                {
                    changesFound = false;
                }

                inFragment = newConfigValue;
            }

            return inFragment;
        }
    }
}
