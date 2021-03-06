﻿#r "bin\Debug\Fasterflect.dll"
#r "bin\Debug\Steego.Fs.Reflection.dll"

#load "HTML.fs"
#load "Printer.fs"
#load "Browser.fs"

open Steego.Fs.WinDump.Browser
open System

let browser = Browser()

type Person(name:string, dob:DateTime) =
    member this.Name = name
    member this.DOB = dob

let bob = Person("Bob Jones", DateTime(1972, 3, 4))

open Steego.Fs
open Steego.Fs.Html.StandardTags

bob |> Printer.printHtml 3 
     |> fun h -> h.ToString()

browser.Dump(bob)
