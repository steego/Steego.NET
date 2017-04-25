module Steego.Web.Explorer

open System // 
open Suave //
open Steego
open Steego.Reflection.Navigators

Web.Server.start()

[<Literal>]
let HeadTemplate = @"
  <head>
    <!--<meta http-equiv='refresh' content='2' /> -->
    <link href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u"" crossorigin=""anonymous"">
    <script src=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"" integrity=""sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa"" crossorigin=""anonymous""></script>
  </head>
"

open Printer

let explore (level:int) (value:'a) = 
  Web.Server.update(fun (ctx:HttpContext) -> async {
        let path = ctx.request.path
        let o = value :> obj
        let navigatedObj = o |> toContext |> NavigateContext path
        let tag = navigatedObj |> print level
        let html = sprintf "<html>%s<body>%s</body></html>" HeadTemplate (tag.ToString())
        return! Successful.OK html ctx
    })

type System.Object with
    member this.Explore(level:int) = explore level this

