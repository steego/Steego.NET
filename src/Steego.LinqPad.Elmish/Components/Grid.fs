module Steego.LinqPad.Elmish.Components.Grid

open System
open Steego.LinqPad.Elmish
open Steego.LinqPad.Elmish.Props
open Steego.LinqPad.Elmish.Helpers   
open Reflection.Utils


let rec renderPrimitive(value:obj) : ReactElement = 
    match value with
    | null -> str("(null)")
    | :? string as s -> str(s)
    | :? int as x -> ofInt(x)
    | :? DateTime as v -> str(v.ToString())
    | :? Nullable<_> as n when n.HasValue = false -> str("(null)")
    | :? Nullable<_> as n -> renderPrimitive(n.Value)
    | x ->
    //    if (x :? IHtmlString) then HtmlRaw((x :?> IHtmlString))
        if x.GetType().IsValueType then str(x.ToString())
        else str("...")

let Grid(value:'a) = 
    let getters = GetMemberGetters(typeof<'a>)
    table [Class("table table-bordered table-striped")] [
        for g in getters do
            let o = g.Get.Invoke(value)
            yield tr [] [
                    yield th [] [ str(g.Name) ]
                    yield td [] [ renderPrimitive(o) ]
            ]
    ]