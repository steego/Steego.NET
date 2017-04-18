module Steego.TypeInfo

open System
open System.Collections
open System.Reflection
open System.Linq
open Fasterflect

let inline isNull o = o = null
let isPrimitiveType(t:Type) = t <> null && (t.IsValueType || t = typeof<string>)
let isPrimitiveObject(o:obj) = (isNull o || isPrimitiveType(o.GetType()))
let isEnumerable(e:obj) =
    match e with
    | null -> false
    | :? string as s -> false
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

let (|IsSeq|_|) (candidate : obj) =
    if isNull candidate then None
    else begin
        let t = candidate.GetType()
        if t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<seq<_>>
        then Some (candidate :?> System.Collections.IEnumerable)
        else None
    end

let (|IsNullable|_|) (candidate : obj) =
    let t = candidate.GetType()
    if t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<Nullable<_>>
    then Some (candidate)
    else None
let (|IsPrimitive|_|) (candidate : obj) =
    if isPrimitiveObject(candidate) then Some(candidate)
    else None
let (|GenericList|_|)(o:obj) =
    if isNull o then None
    else
        let t = o.GetType()
        let ti = TypeInfo(t)
        if ti.IsGenericSeq then
            if ti.IsPrimitive = false then
                let getters = ti.ElementType.Members
                let list = o :?> IEnumerable
                Some(getters, list)
            else 
                None
        elif t.IsArray && t.HasElementType then
            let ti = TypeInfo(t.GetElementType())
            if ti.IsPrimitive = false then
                let getters = ti.ElementType.Members
                let list = o :?> IEnumerable
                Some(getters, list)
            else 
                None                
        else None

let (|Object|_|)(o:obj) = 
    if isNull o then None
    elif isEnumerable(o) then None
    elif isPrimitiveObject(o) then None
    else
        
        let members = TypeInfo(o.GetType()).Members
        let primitiveMembers = members |> List.filter(fun m -> isPrimitiveType m.Type)
        let objectMembers = members |> List.filter(fun m -> (not (isPrimitiveType m.Type)) && (not (isSeq m.Type)))
        let enumerableMembers = members |> List.filter(fun m -> isSeq m.Type)
        Some(members, primitiveMembers, objectMembers, enumerableMembers, obj)

