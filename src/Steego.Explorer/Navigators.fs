module Steego.Reflection.Navigators

open System
open System.Collections
open Fasterflect

type NavRule = (string -> obj -> obj option)
type NavRulePicker = Type -> NavRule option

module NavRules = 
    let TryGetProperty : NavRule = fun (name:string) (o:obj) -> 
        Some(Fasterflect.PropertyExtensions.GetPropertyValue(o, name))
    let TryGetDictionary : NavRule = fun (name:string) (o:obj) -> 
        let d = o :?> IDictionary
        if d.Contains(name) then Some(d.Item[name])
        else None
    

module NavRulePickers = 
    let TryDictionaryNav : NavRulePicker = fun t -> 
        if t = typeof<IDictionary> || t.Implements<IDictionary>() then
            Some(NavRules.TryGetDictionary)
        else
            None
  
    let TryIndexerNav : NavRulePicker = fun t -> 
        try
            let indexerMethod = t.DelegateForGetIndexer(typeof<string>)
            let tryGetIndex(name:string) (o:obj) = Some(indexerMethod.Invoke(obj, name))
            Some(tryGetIndex)
        with ex -> None

    let DefaultPickers = [ TryIndexerNav; TryDictionaryNav ]

    let DefaultPicker : NavRulePicker = fun t ->
        DefaultPickers |> List.tryPick(fun p -> p t)

    

let getNavRule (rules: NavRulePicker list) = 
    let getRule(t:Type) = 
        match rules |> List.tryPick(fun r -> r t) with
        | Some(r) -> r
        | None -> NavRules.TryGetProperty
    Steego.TypeInfo.cacheTypeInfo getRule

//let GetRule = getNavRule NavRulePickers.DefaultPickers


[<Struct>]
type NavContext(value:obj, path:string list) = 
    member this.Value = value
    member this.Path = path
    member this.AddPath(segment, newValue) = NavContext(newValue, segment::path)
    member this.SetValue(value:obj) = NavContext(value, path)
    new(value) = NavContext(value, [])

//let NavigatePath(obj:NavContext, path:string list) =
//    if obj.Value = null || path = null then obj.SetValue(null)
//    else obj