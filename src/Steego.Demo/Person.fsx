#load "LoadWebServer.fsx"

open System
open Steego.Web.Explorer
open Steego.Reflection.Navigators

type Person(name:string, dob:DateTime) = 
    member this.Name = name
    member this.DOB = dob
    member this.Tasks = 
        [ for n in 1..3 do
            let name = "My Task " + string(n)
            let due = DateTime.Today.AddDays(30.0 * float(n))
            yield Task(name, due) ]
and Task(name:string, due:DateTime) = 
    member this.Name = name
    member this.Due = due
    
let bob = Person("Robert", DateTime(1980, 1, 1)) 

bob.Explore(2)
