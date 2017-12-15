module Steego.Fs.Reflection.Navigators

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
        | Numeric(i), (o) -> Some((o :?> IEnumerable).OfType<obj>().Skip(i).FirstOrDefault())
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

    //let DefaultPickers : NavRulePicker list = [ TryIndexerNav; TryDictionaryNav; TrySeqNav ]
    let DefaultPickers : NavRulePicker list = [ TrySeqNav ]

    let DefaultPicker : NavRulePicker = fun t ->
        DefaultPickers |> List.tryPick(fun p -> p t)
end
    

let CombineTypeRules (rules: NavRulePicker list) : Type -> NavRule = 
    let getRule(t:Type) = 
        match rules |> List.tryPick(fun r -> r t) with
        | Some(r) -> r
        | None -> NavRules.TryGetProperty
    TypeInfo.cacheTypeInfo getRule

let defaultNavRuleMap : NavRuleMap = CombineTypeRules NavRulePickers.DefaultPickers

type NavContext = { Value:obj; Path: string list }
    
let toContext(value:'a) = { Value = value :> obj; Path = [] }
let addPath(segment,newValue) (ctx:NavContext) = 
    { Value = newValue :> obj; Path = segment::ctx.Path }

let NavigatePath(navRule:NavRule) = 
    let navNext(obj:NavContext)(path:string) : NavContext = 
        match navRule path obj.Value with
        | Some(value) -> obj |> addPath(path, value)
        | None -> raise(exn("Member not found!"))
    fun (path:string list) (obj:NavContext) ->
        if isNull obj.Value then obj
        elif List.isEmpty path then obj
        else
            path |> List.fold navNext obj

open TypeInfo

let navRule(rule:NavRuleMap) : NavRule = 
  let lookup = rule |> cacheTypeInfo
  fun name obj ->
    let t = if isNull obj then null else obj.GetType()
    let rule = lookup t
    rule name obj

let defaultNavRule : NavRule = navRule defaultNavRuleMap

let navPath = NavigatePath defaultNavRule

let NavigateContext(path:string)(n:NavContext) = 
    let isNotBlank(s:string) = not (String.IsNullOrWhiteSpace(s))
    let path = path.Split('/') |> List.ofArray |> List.filter isNotBlank
    let result = n |> navPath path
    result
