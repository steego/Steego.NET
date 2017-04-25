#load "LoadWebServer.fsx"

open System
open Steego.Html
open Steego.Printer
open Steego.Web.Explorer

let doc (level:int) (objects:obj list) : Tag = 
    makeTag "div" [] [ for o in objects -> printHtml level o ]

let h1 t = makeTag "h1" [] [ Text(t) ]
let h2 t = makeTag "h2" [] [ Text(t) ]
let p t = makeTag "p" [] [ Text(t) ]

type PersonRecord = { Name:string; DOB: DateTime } with
    member this.Age = int((DateTime.Now - this.DOB).TotalDays / 365.0)

type Person(name:string, dob:DateTime) = 
    member this.Name = name
    member this.DOB = dob
    member this.GetHtml() = 
        doc 2 [
            h1("My " + name + " Fan Pan written in F#")
            p(name + " the most dynamic and exciting musician.")
            { Name = name; DOB = dob }
        ]
    interface IHtmlObject with
        member this.ToHtml = this.GetHtml()

let taylor = Person("Taylor Swift", DateTime(1989, 12, 13))

let jimmy = Person("Jimmy Page", DateTime(1944, 1, 9))

taylor.Explore(1)
jimmy.Explore(1)

