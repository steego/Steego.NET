#r "bin\Debug\Fasterflect.dll"
#r "bin\Debug\Steego.Fs.Reflection.dll"

#load "HTML.fs"
#load "Printer.fs"
#load "Browser.fs"
//#r "bin\Debug\Steego.Fs.WinDump.dll"

open Steego.Fs.WinDump.Browser
open System

let browser = Browser()


type Person(name:string, dob:DateTime) =
    member this.Name = name
    member this.DOB = dob

let bob = Person("Bob Jones", DateTime(1972, 3, 4))

open Steego.Fs
open Steego.Fs.Html
open Steego.Fs.Printer



template |> print 3

browser.Dump(bob)

//let doTest() = async {
//        for i in 1..100 do
//            do! Async.Sleep(1000)
//            browser.Dump((i, DateTime.Now)) |> ignore
//    }

//doTest() |> Async.Start
