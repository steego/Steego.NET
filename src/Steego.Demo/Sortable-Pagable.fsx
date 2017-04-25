#load "LoadWebServer.fsx"

//#r "FSharp.Data.dll"
#r "FSharp.Data.TypeProviders.dll"

open System.IO
open Steego.Web.Explorer
open Steego.Reflection.Navigators

module SharedQueryShit = begin
    open System.Linq

    type E = System.Linq.Expressions.Expression

    type SortDirection = Ascending | Descending

    type System.Linq.IQueryable<'a> with
        member this.Sort(column:string, direction:SortDirection) = 
            let param = E.Parameter(typeof<'a>, "query")
            let getProp = E.PropertyOrField(param, column)
            let sortExpr = E.Lambda<System.Func<'a,_>>(getProp, param)

            match direction with
            | Ascending -> this.OrderBy(sortExpr).AsQueryable()
            | Descending -> this.OrderByDescending(sortExpr).AsQueryable()


    type PageRequest = { Page: int; PageSize: int; Sort: string; SortDirection: SortDirection }

    type PagedList<'a>(query:IQueryable<'a>, r:PageRequest) = 
        let count = query.Count()
        let pageCount = int(System.Math.Ceiling(float(count / r.PageSize)))
        let row = ((r.Page - 1) |> max 0) * (r.PageSize |> max 0)
        let sortedQuery = if r.Sort = null then query else query.Sort(r.Sort, r.SortDirection).AsQueryable()
        let list = query.Skip(row).Take(r.PageSize) |> List.ofSeq
        member this.List = list
        member this.CurrentPage = r.Page
        member this.PageSize = r.PageSize
        member this.PageCount = pageCount
    



    type SortedPageList<'a>(query:IQueryable<'a>) = 
        member this.GetList(r:PageRequest) = PagedList(query, r)
end 

open SharedQueryShit

module DAL = begin
    open FSharp.Data.TypeProviders
    [<Literal>]
    let conn = "Data Source=.\SQL2014;Integrated Security=SSPI;Initial Catalog=SampleSupplier"

    type DatabaseType = SqlDataConnection<conn>
    //type DatabaseType = SqlEntityConnection<conn>
    let db = DatabaseType.GetDataContext()
end


module BusinessLayer = begin

    open DAL

    type App() = 
        member this.Database = db
        member this.Customers = db.Customer
        member this.Orders = Orders()
    and Orders() = 
        //member this.Top = totalAmount.Value
        member this.All = db.Order

end

//let r = { Page = 0; PageSize = 10; Sort = null; SortDirection = Ascending }
//let sorted = SortedPageList(db.Customer).GetList(r)
//sorted.Explore(1)

type Explorer() = 
    member this.DAL = DAL.db
    member this.AppLayer = BusinessLayer.App()

App().Explore(1)