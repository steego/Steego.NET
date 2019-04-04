namespace Steego.LinqPad.Elmish

[<AutoOpen>]
module Main =
    open System
    open Steego.LinqPad.Elmish

    let private container(o:obj) = 
        let container = LINQPad.DumpContainer(o)
        Console.WriteLine(container)
        container

    let show(o:obj) = 
        match o with
        | :? ReactElement as r -> container(r)
        | value -> container(value)
