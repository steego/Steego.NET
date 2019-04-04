namespace Steego.LinqPad.Elmish

[<AutoOpen>]
module Main =
    open System
    open Steego.LinqPad.Elmish

    let private render(o:obj) = Console.WriteLine(o)

    let private container(o:obj) = 
        let container = LINQPad.DumpContainer(o)
        Console.WriteLine(container)
        container

    let show(o:obj) = 
        match o with
        | :? ReactElement as r -> container(r)
        | value -> container(value)

    let includeBootstrap() =
        render(style [] [ str("body { color: black !important; background-color: white !important; }") ])
        render(Steego.LinqPad.Elmish.Components.CSS.resetCss)
        render(Steego.LinqPad.Elmish.Components.CSS.bootstrapCss)
        render(Steego.LinqPad.Elmish.Components.CSS.jQuery)
        render(Steego.LinqPad.Elmish.Components.CSS.popperJs)
        render(Steego.LinqPad.Elmish.Components.CSS.bootStrapJs)
        
