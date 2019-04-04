[<AutoOpen>]
module Steego.LinqPad.Elmish.UI

open Steego.LinqPad.Elmish.Components.UI

let grid(x) = Grid(x) :> ReactElement
let table(x) = Table(x) :> ReactElement
let page(x) = Page(x) :> ReactElement
let panels(x) = Panels(x) :> ReactElement
let tabs(x) = Tabs("", x) :> ReactElement