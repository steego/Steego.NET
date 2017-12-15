#r "bin\Debug\Fasterflect.dll"
#r "bin\Debug\Steego.Fs.Reflection.dll"

// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

//#load "Library1.fs"
open Steego.Fs.Reflection

type MyApp() = 
    member this.Files = ["one"]

let getters = 
    typeof<MyApp> |> TypeInfo.getMemberGetters

// Define your library scripting code here

