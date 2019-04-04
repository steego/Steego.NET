module Reflection.Utils

open System
open System.Reflection

let isReadProperty(p:PropertyInfo) = 
    p.IsSpecialName = false && p.GetIndexParameters().Length = 0
    && p.CanRead && p.IsSpecialName = false

type E = System.Linq.Expressions.Expression

type MemberGetter = { Name: string; Type: Type; Get: Func<Object, Object> }

let GetMemberGetters(objectType:Type) = 
    let flags = BindingFlags.Public ||| BindingFlags.Instance

    [| for p in objectType.GetProperties(flags) do
       if isReadProperty(p) then
            let param = E.Parameter(typeof<Object>, "x")
            let cparam = E.Convert(param, objectType)
            let IsType = E.TypeIs(param, objectType)
            let GetProp = E.Convert(E.Property(cparam, p), typeof<Object>)
            let Check = E.Lambda<Func<Object, Object>>(E.Condition(IsType, GetProp, E.Constant(null)), param).Compile()
            yield { Name = p.Name; Type = p.PropertyType; Get = Check } 
    |]

type TypeInfo<'a>() = 
    static let memberGetters = GetMemberGetters(typeof<'a>)
    static member GetMemberGetters() = memberGetters