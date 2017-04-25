module Steego.TypeInfo

open System
open System.Collections
open System.Reflection
open System.Linq
open Fasterflect

let cacheTypeInfo<'a>(getTypeInfo: Type -> 'a) = 
  let hashTable = new System.Collections.Hashtable()
  fun(t:Type) ->
    let result = hashTable.[t]
    if result <> null then result :?> 'a
    else
      let newEntry = getTypeInfo(t)
      lock hashTable (fun () ->
        hashTable.Add(t, newEntry)
      )
      newEntry

let inline isNull o = o = null
let isPrimitiveType(t:Type) = t <> null && (t.IsValueType || t = typeof<string>)
let isPrimitiveObject(o:obj) = (isNull o || isPrimitiveType(o.GetType()))
let isEnumerable(e:obj) =
    match e with
    | null -> false
    | :? string -> false
    | :? IEnumerable -> true
    | _ -> false

let isSeq(t:System.Type) = 
    t <> null &&
    t <> typeof<string> &&
    t.GetInterface(typeof<System.Collections.IEnumerable>.Name) |> isNull |> not

let isEnumerableType(t:Type) = if t <> null then isSeq(t) && t <> typeof<string> else false

let private implementsGeneric<'a>(t:Type) = 
  let find = typedefof<'a>.GetGenericTypeDefinition()
  if t = null then false
  elif t.IsGenericType = false then false
  elif t.IsInterface && t.GetGenericTypeDefinition() = find then true
  else
    t.GetInterfaces()
    |> Seq.filter(fun i -> i.IsGenericType)
    |> Seq.map(fun i -> i.GetGenericTypeDefinition())
    |> Seq.exists(fun i -> i = find)

let isGenericSeq(t:Type) = implementsGeneric<seq<_>>(t)

type MemberGetter(name:string, memberType:Type, getter:Func<obj,obj>) = 
  member this.Name = name
  member this.Type = memberType
  member this.Get(o) = getter.Invoke(o)
  member this.IsEnumerable = isSeq(memberType)

let getPropertyValue(o:obj, p:PropertyInfo) = 
    try p.GetValue(o, null)
    with ex -> null

let getMemberGetters(t:Type) =
    if t = null then []
    else
        let isGoodProperty (p:PropertyInfo) = 
            (not p.IsSpecialName)
            && p.GetIndexParameters().Length = 0
            && p.CanRead
            
        let flags = BindingFlags.Public ||| BindingFlags.Instance
        
        [ 
            // Return properties
            for p in t.GetProperties(flags) do
                if isGoodProperty p then
                    yield MemberGetter(p.Name, p.PropertyType, fun o -> getPropertyValue(o, p))
            // Return fields
            for f in t.GetFields() do
                if f.IsPublic && not f.IsSpecialName && not f.IsStatic then
                    yield MemberGetter(f.Name, f.FieldType, fun o -> f.GetValue(o))
        ]

let getElementType(t:Type) = if t = null then null else t.GetGenericArguments().FirstOrDefault()

type TypeInfo(t:Type) = 
    let elementType = getElementType(t)
    let members = getMemberGetters(t)
    let isEnumerable = isEnumerableType(t)
    member this.Name = t.Name
    member this.IsPrimitive = isPrimitiveType(t)
    member this.IsNull = t = null
    member this.Members = members
    member this.PrimitiveMembers = members |> List.filter(fun m -> isPrimitiveType m.Type)
    member this.ObjectMembers = members |> List.filter(fun m -> (not (isPrimitiveType m.Type)) && (not (isSeq m.Type)))
    member this.EnumerableMembers = members |> List.filter(fun m -> isSeq m.Type)    
    member this.IsSeq = isEnumerable
    member this.IsGenericSeq = isGenericSeq(t)
    member this.ElementType = TypeInfo(elementType)
    new(o:obj) = TypeInfo(if isNull o then null else o.GetType())

let getObjectInfo(o:obj) = if isNull o  then [] else TypeInfo(o.GetType()).Members

