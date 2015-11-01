namespace fagox

open FSharp.Data
open System.Xml.Linq
open System.Collections.Generic

module ConfigurationProvider =

    [<Literal>]
    let GenericEnvironment =  """<configFragments>
                                     <configFragment>
                                          job {
                                      </configFragment>
                                      <configFragment importConfigFile="../scmDefinition.xml"/>
                                      <configFragment importConfigFile="../publishDefinition.xml"/>
                                      <configFragment>
                                          }  
                                      </configFragment>
                                  </configFragments>"""
    
    type Tuple(inFirst:int,inSecond:int) =
        member this.first = inFirst
        member this.second = inSecond

    type ConfigurationDefinition = XmlProvider<GenericEnvironment>

    let rec GetConfiguration (configurationFileLocation:string, inEnvironmentDictionary:Dictionary<string,string>,level:int):List<string> =
        let configFragmentList = new List<string>()
        let resolvedFragmentList = new List<string>()

        //we should start and process the file
        let parsedXml = ConfigurationDefinition.Parse(System.IO.File.ReadAllText(configurationFileLocation))

        for configFragment in parsedXml.ConfigFragments do
            if configFragment.ImportConfigFile.IsSome then
                configFragmentList.AddRange( GetConfiguration(configurationFileLocation+"/"+configFragment.ImportConfigFile.Value,inEnvironmentDictionary,level+1))
            else
                configFragmentList.Add(configFragment.Value.Value)

        //we should resolve the environment values 
        if level = 1 then
            for configFragment in configFragmentList do
                let forReturn = new List<string>()
                let encounters = new List<int>()
                let secondEncounters = new List<int>()

                for i=0 to configFragment.Length-1 do
                    if configFragment.[i] = '$' && configFragment.[i+1] = '{' then
                        encounters.Add(i)
                    else if configFragment.[i] = '}' then
                        secondEncounters.Add(i)

                //try to get values we can process
                let result = new List<Tuple>()

                let mutable j=0
                for i=0 to encounters.Count-1 do
                    if (encounters.[i]<secondEncounters.[j]) then
                        if (i=encounters.Count-1) || encounters.[i+1] >secondEncounters.[j] then
                            //add pair
                            result.Add(new Tuple(encounters.[i],secondEncounters.[j]))
                            j <- j+1
                    else 
                        j <- j+1

                for iterTuplu in result do
                    forReturn.Add("${"+ configFragment.Substring(iterTuplu.first+2, iterTuplu.second-iterTuplu.first-2) + "}")


                //start to replace the environment values we found
                let mutable newConfigValue = configFragment

                for valueFound in forReturn do
                    newConfigValue <- configFragment.Replace(valueFound,inEnvironmentDictionary.[valueFound.Substring(2,valueFound.Length-3)])

                resolvedFragmentList.Add(newConfigValue)

        if level = 1 then
            resolvedFragmentList
        else 
            configFragmentList

    let PublishConfiguration (configurationFileLocation:string, inEnvironmentDictionary:Dictionary<string,string>):string =
        let resultOfInterpretation = GetConfiguration(configurationFileLocation,inEnvironmentDictionary,1)
        let mutable allFile = ""

        for configFragment in resultOfInterpretation do
            allFile <- allFile + configFragment

        allFile

    let PublishConfigurationInConsole (configurationFileLocation:string, inEnvironmentDictionary:Dictionary<string,string>) =
         let resultOfInterpretation = PublishConfiguration(configurationFileLocation,inEnvironmentDictionary)

         printf "We have the result of interpretation:"
         printf "%s" resultOfInterpretation