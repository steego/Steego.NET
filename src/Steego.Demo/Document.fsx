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

let fanPage =
    doc 2 [
        h1("My Taylor Swift Fan Pan written in F#")
        p("Taylor Swift the most dynamic and exciting musician.")
        { Name = "Taylor Swift"; DOB = DateTime(1989, 12, 13) }
    ]

fanPage.Explore(1)