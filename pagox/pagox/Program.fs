// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

module fagox =

[<EntryPoint>]
let main argv = 
    printfn "%A" argv

    fagox.EnvironmentProvider.PublishDictionary (fagox.EnvironmentProvider.GetProperties ( """C:\Users\savan\Documents\Visual Studio 2015\Projects\pagox\pagox\TopEnvironment.xml""", 1))

    fagox.ConfigurationProvider.PublishConfigurationInConsole("""C:\Users\savan\Documents\Visual Studio 2015\Projects\pagox\pagox\configFragmentDocument.xml""",fagox.EnvironmentProvider.GetProperties ( """C:\Users\savan\Documents\Visual Studio 2015\Projects\pagox\pagox\TopEnvironment.xml""", 1))

    0 // return an integer exit code
