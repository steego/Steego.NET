[<AutoOpen>]
module Steego.LinqPad.Elmish.UI

open Steego.LinqPad.Elmish.Components.UI
open System

module Ob = 
    open FSharp.Control.Reactive
    open FSharp.Control.Reactive.Builders

    let takeMax max input = 
        if List.length input > max then List.take max input else input

    let shiftBuffer max input = 
        observe {
            let mutable result = []
            let! item = input
            result <- item::result |> takeMax max
            yield result
        }

let grid(x) = Grid(x) :> ReactElement
let table(x) = Table(x) :> ReactElement
let page(x) = Page(x) :> ReactElement
let panels(x) = Panels(x) :> ReactElement
let tabs(x) = Tabs("", x) :> ReactElement
let watch(observable:IObservable<_>) = 
    let container = LINQPad.DumpContainer()
    observable.Subscribe(fun x -> container.UpdateContent(x)) |> ignore
    HTMLNode.DumpContainer(container)
let watchLast max = Ob.shiftBuffer max >> watch
        