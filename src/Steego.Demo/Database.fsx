#load "LoadWebServer.fsx"

//#r "FSharp.Data.dll"
#r "FSharp.Data.TypeProviders.dll"

open System.IO
open Steego.Web.Explorer
open Steego.Reflection.Navigators

open FSharp.Data.TypeProviders

[<Literal>]
let conn = "Data Source=.\SQL2014;Integrated Security=SSPI;Initial Catalog=SampleSupplier"

//type DatabaseType = SqlDataConnection<conn>
type DatabaseType = SqlEntityConnection<conn>

open System.Linq

let db = DatabaseType.GetDataContext()
let totalAmount = lazy db.Order.Sum(fun o -> o.TotalAmount)

type App() = 
    member this.Database = db
    member this.Customers = db.Customer
    member this.Orders = Orders()
and Orders() = 
    member this.Top = totalAmount.Value
    member this.All = db.Order

App().Explore(1)