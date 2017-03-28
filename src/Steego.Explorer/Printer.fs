module Steego.Printer

open System
open System.Text
open System.Collections
open System.Linq
open TypeInfo

let div = Html.makeTag "div"
let table = Html.makeTag "table"
let thead = Html.makeTag "thead"
let tbody = Html.makeTag "tbody"
let tr = Html.makeTag "tr"
let th = Html.makeTag "th"
let td = Html.makeTag "td"

let getTypeName(o:obj) = Html.Text( sprintf "Not matched: %s" (o.GetType().FullName))

let keyValueRow(name:string, value:Html.Tag) = tr [] [ th [] [ Html.Text(name) ]; td [] [ value ] ]

let info = new TypeInfo(null)

let maxTake = 100

let rec print (level:int) = 
    let rec print (level:int) (o:obj) : Html.Tag =         
        if level < 1 then
            match o with
            | null -> Html.Text("<null>")
            | :? int as v -> Html.Text(v.ToString())
            | :? String as s -> Html.Text(s)
            | :? DateTime as v -> Html.Text(v.ToShortDateString())
            | IsPrimitive(n) -> Html.Text(n.ToString())
            | IsNullable(n) -> Html.Text(n.ToString())
            | GenericList(getters, list) -> Html.Text("List...")
            | :? System.Collections.IEnumerable as s -> Html.Text("List...")
            | Object(members, prims, subObjs, enums, o1) -> Html.Text("Object...")
            | _ -> Html.Text("...")
        else
            match o with
            | null -> Html.Text("<null>")
            | :? int as v -> Html.Text(v.ToString())
            | :? String as s -> Html.Text(s)
            | :? DateTime as v -> Html.Text(v.ToShortDateString())
            | IsPrimitive(n) -> Html.Text(n.ToString())
            | IsNullable(n) -> Html.Text(n.ToString())
            | GenericList(getters, list) -> printGenericList (level - 1)  getters list
            | :? System.Collections.IEnumerable as s -> printList (level - 1) s
            | Object(members, prims, subObjs, enums, o1) -> printObject (level - 1) members o
            | _ -> Html.Text("...")        

    and printObject (level:int) (getters:MemberGetter list) =
        fun (o:obj) ->
            let sb = new StringBuilder()
            let typeInfo = new TypeInfo(o)
            let primativeMembers = typeInfo.PrimitiveMembers
            table 
                [("class", "table table-bordered table-striped")]
                [   
                    //  Show primitive members
                    for g in primativeMembers do
                        let value = g.Get(o) |> print level
                        yield keyValueRow(g.Name, value)
                    //  Show object members
                    for g in typeInfo.ObjectMembers do
                        let value = g.Get(o) |> print level
                        yield keyValueRow(g.Name, value)
                    //  Show enumerable members
                    for g in typeInfo.EnumerableMembers do
                        let value = g.Get(o) |> print level
                        yield tr [] [ 
                            th [("colspan", "1")] [ Html.Text(g.Name) ] 
                            td [("colspan", "1")] [ value ]
                        ]
                ]

    and printList (level:int) (list:IEnumerable) = 
        let list = seq { for o in list -> o }
        table [("class", "table table-bordered table-striped")]
              [  for item in list.Take(maxTake) do
                    let value = item |> print level
                    yield tr [] [ td [] [ value ] ] 
              ]

    and printGenericList (level:int) (getters:MemberGetter list) (list:IEnumerable) =
        let list = seq { for o in list -> o }
        table 
            [("class", "table table-bordered table-striped")]
            [ thead [] [ for g in getters -> th [] [ Html.Text(g.Name) ] ]
              tbody [] [ for item in list.Take(maxTake) do
                            yield tr [] [ for g in getters do
                                            let value = g.Get(item) |> print level 
                                            yield td [] [ value ]
                                        ] 
                       ]
            ]
    fun (o:obj) -> (print level o).ToString()

