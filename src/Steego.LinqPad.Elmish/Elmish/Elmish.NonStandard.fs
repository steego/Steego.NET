[<AutoOpen>]
module Steego.LinqPad.Elmish.NonStandard

open Steego.LinqPad.Elmish

let dump(o:obj) = HTMLNode.DumpContainer(LINQPad.DumpContainer(o))