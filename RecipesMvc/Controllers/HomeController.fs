namespace RecipesMvc.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax

type HomeController() =
    inherit Controller()
    member this.Index () = this.View()
    member this.Test() = this.Json("Test Controller", JsonRequestBehavior.AllowGet)
    member this.Path(id) = this.Json("Path: " + id, JsonRequestBehavior.AllowGet)
