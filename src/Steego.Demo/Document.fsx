#load "LoadWebServer.fsx"

open System
open Steego.Html
open Steego.Printer
open Steego.Web.Explorer

let doc (level:int) (objects:obj list) : Tag = 
    makeTag "div" [] [ for o in objects -> printHtml level o ]

let h1 t = makeTag "h1" [] [ Text(t) ]
let p t = makeTag "p" [] [ Text(t) ]

type Person = { Name: string; DOB: DateTime }

let myPage = doc 2 [
    h1 "Welcome"
    p "This is a paragraph"
    { Name = "Taylor Swift"; DOB = DateTime(1989, 12, 19) }
]

myPage.Explore(1)

