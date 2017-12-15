module Steego.Demo.Reflection

open System
open System.Reflection

module internal Property = begin
    let isPublic(p:PropertyInfo) = not p.IsSpecialName
end

module internal Method = begin
    let isPublic(m:MethodInfo) = not m.IsSpecialName && m.IsPublic
end

module TypeInfo = begin
    let getPublicMethods(t:Type) = 
        t.GetMethods() |> Array.filter Method.isPublic |> Array.filter (fun m -> m.DeclaringType = t)
    let getPublicProperties(t:Type) = 
        t.GetProperties() |> Array.filter Property.isPublic |> Array.filter (fun m -> m.DeclaringType = t)
end