#r "FSharp.Core.dll"

#I @"bin\Release"


#r "System.dll"
#r "System.Core.dll"
#r "System.Web.dll"
#r "System.ValueTuple.dll"
#r "Fasterflect.dll"
#r "Suave.dll"
#r "Steego.Web.dll"

type Person(name:string) = 
    member this.Name = name

Person("Bob") |> Steego.Web.Explorer.dump 1