#r @".\bin\Debug\Steego.TypeProviders.dll"

open Steego.TypeProviders.RegexProvider

type Hello = RegexTyped< @"(?<AreaCode>^\d{3})-(?<PhoneNumber>\d{3}-\d{4}$)">

let matched = Hello.IsMatch("123-456-7890")

printfn "Hello World %b" matched