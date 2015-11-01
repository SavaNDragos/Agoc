namespace fagox

open FSharp.Data
open System.Xml.Linq
open System.Collections.Generic

module EnvironmentProvider =

    type Property(inName, inValue) =
        member this.Name = inName
        member this.Value = inValue

    type Tuple(inFirst:int,inSecond:int) =
        member this.first = inFirst
        member this.second = inSecond


    [<Literal>]
    let GenericEnvironment = """<environment>   
        <property name="Location" value="London" />
        <property name="Cluster"  value="HighEnd" />
        <import fileLocation="../Upper/SecondFile" orderOfInterpreting="1"/>
        <import fileLocation="../Upper/SecondFile" orderOfInterpreting="2"/>
        </environment>"""

    type Environment = XmlProvider<GenericEnvironment>

    let rec GetProperties (environmentFileLocation:string, level:int) : Dictionary<string, string> =
        let dictForReturn = new System.Collections.Generic.Dictionary<string,string>()
        let tempPropertiesForReturn = new List<Property>()
        let importedFiles = new List<string>()
        let tempFinalProperties = new List<Property>()
        let tempToBeProcessedProperties = new List<Property>()

        let parsedXml = Environment.Parse(System.IO.File.ReadAllText(environmentFileLocation))

        //#region METHODS

        //by adding them to dictionary we solve the issue of  overriding values
        let AddToDictionary (inProperty:Property) =
            if dictForReturn.ContainsKey(inProperty.Name) then
                dictForReturn.[inProperty.Name] <- inProperty.Value
            else
                dictForReturn.Add(inProperty.Name,inProperty.Value)

        let AddToDictionaryWhitoutOverride (inKey:string, inValue:string) =
            if not (dictForReturn.ContainsKey(inKey)) then
                dictForReturn.Add(inKey,inValue)

        let rec GetValuesToBeProcessed (inValue:string) : List<string> =
            let forReturn = new List<string>()
            let encounters = new List<int>()
            let secondEncounters = new List<int>()

            for i=0 to inValue.Length-1 do
                if inValue.[i] = '$' && inValue.[i+1] = '{' then
                    encounters.Add(i)
                else if inValue.[i] = '}' then
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
                forReturn.Add("${"+ inValue.Substring(iterTuplu.first+2, iterTuplu.second-iterTuplu.first-2) + "}")

            forReturn

        //replace strings
        let ReplaceString (inStringList:List<string> , inKey:string, inValue:string):int =
            let mutable foundValue = 0
            let mutable newValue = inValue
            //search for value
            for iterValue in inStringList do
                if (dictForReturn.ContainsKey(iterValue.Substring(2,iterValue.Length-3))) then
                    newValue <- newValue.Replace(iterValue,dictForReturn.[iterValue.Substring(2,iterValue.Length-3)])
                    foundValue <- foundValue + 1

            //update dictionary
            if not (System.String.Equals(inValue, newValue)) then
                dictForReturn.[inKey] <- newValue

            foundValue

        //#endregion

        //OPERATIONS

        //get properties from file
        for iterProperty in parsedXml.Properties do
            //separate properties that are final from those that still need to be processed 
            if iterProperty.Value.Contains("${") then
                tempToBeProcessedProperties.Add(new Property(iterProperty.Name,iterProperty.Value))
            //add current value to dictionary
            AddToDictionary(new Property(iterProperty.Name,iterProperty.Value))
       
        //get files from which we are going to extract more properties
        for iterImported in parsedXml.Imports do
            importedFiles.Add(iterImported.FileLocation)

        //walked throw each imported file and get properties          
        importedFiles.Reverse()

        for iterImported in importedFiles do
            let newDict = GetProperties((environmentFileLocation + @"/" + iterImported), level+1 )

            //add new items from dictionary
            for iterdic in newDict do
                AddToDictionaryWhitoutOverride(iterdic.Key,iterdic.Value)
        
        if level=1 then
            let mutable shouldContinue = true
            
            let iterator = new List<string>()

            //create list with 
            for iterDic in dictForReturn do
                iterator.Add(iterDic.Key)

            //start solving coded values
            while shouldContinue do
                shouldContinue <- false

                for iterDic in iterator do
                    if dictForReturn.[iterDic].Contains("${") then
                        let stringToProcess = GetValuesToBeProcessed(dictForReturn.[iterDic])

                        if (stringToProcess.Count > 0) then
                            if (ReplaceString(stringToProcess, iterDic,dictForReturn.[iterDic]) > 0) then
                                shouldContinue <- true

        //return dictionary
        dictForReturn

    //let GetProperties (environmentFileLocation:string) : Dictionary<string, string> =

    let PublishDictionary (inDictionary:Dictionary<string,string>) =
         for iterdic in inDictionary do
            printf "property name: %s  value:  %s" iterdic.Key  iterdic.Value