module Steego.Reflection.Navigators

open System
open System.Collections
open System.Linq
open Fasterflect

type NavRule = (string -> obj -> obj option)
type NavRulePicker = Type -> NavRule option
type NavRuleMap = Type -> NavRule



let (|Numeric|_|) (s:string) = 
  let (isInt,value) = Int32.TryParse(s)
  if isInt then Some(value) else None

module NavRules = begin
    let TryGetProperty : NavRule = fun (name:string) (o:obj) -> 
        Some(Fasterflect.PropertyExtensions.GetPropertyValue(o, name))
    let TryGetDictionary : NavRule = fun (name:string) (o:obj) -> 
        let d = o :?> IDictionary
        if d.Contains(name) then Some(d.Item[name])
        else None
    let TryGetEnumerable : NavRule = fun(name:string) (o:obj) ->
        match name, o with
        | Numeric(i), (o) -> Some((o :?> IEnumerable).OfType<obj>().Take(i).FirstOrDefault())
        | _ -> None
end



module NavRulePickers = begin
    

    let TrySeqNav : NavRulePicker = fun t -> 
        let isSeq = t = typeof<IEnumerable> || t.Implements<IEnumerable>()
        if isSeq then Some(NavRules.TryGetEnumerable)
        else None

    let TryDictionaryNav : NavRulePicker = fun t -> 
        let isDictionary = t = typeof<IDictionary> || t.Implements<IDictionary>()
        if isDictionary then Some(NavRules.TryGetDictionary)
        else None
  
    let TryIndexerNav : NavRulePicker = fun t -> 
        try
            let indexerMethod = t.DelegateForGetIndexer(typeof<string>)
            let tryGetIndex(name:string) (o:obj) = Some(indexerMethod.Invoke(o, name))
            Some(tryGetIndex)
        with ex -> None

    let DefaultPickers : NavRulePicker list = [ TryIndexerNav; TryDictionaryNav; TrySeqNav ]

    let DefaultPicker : NavRulePicker = fun t ->
        DefaultPickers |> List.tryPick(fun p -> p t)
end
    

let CombineTypeRules (rules: NavRulePicker list) : Type -> NavRule = 
    let getRule(t:Type) = 
        match rules |> List.tryPick(fun r -> r t) with
        | Some(r) -> r
        | None -> NavRules.TryGetProperty
    Steego.TypeInfo.cacheTypeInfo getRule

let defaultNavRuleMap : NavRuleMap = CombineTypeRules NavRulePickers.DefaultPickers

[<Struct>]
type NavContext(value:obj, path:string list) = 
    member this.Value = value
    member this.Path = path
    member this.AddPath(segment, newValue) = NavContext(newValue, segment::path)
    member this.SetValue(value:obj) = NavContext(value, path)
    new(value) = NavContext(value, [])

let ToContext<'a>(value:'a) = NavContext(value :> obj, [])

let NavigatePath(navRule:NavRule) = 
    let navNext(obj:NavContext)(path:string) : NavContext = 
        match navRule path obj.Value with
        | Some(value) -> obj.AddPath(path, value)
        | None -> raise(exn("Member not found!"))
    fun (path:string list) (obj:NavContext) ->
        if obj.Value = null then obj
        elif path = [] then obj
        else
            path |> List.fold navNext obj

open Steego.TypeInfo

let navRule(rule:NavRuleMap) : NavRule = 
  let lookup = rule |> cacheTypeInfo
  fun name obj ->
    let t = if obj = null then null else obj.GetType()
    let rule = lookup t
    rule name obj