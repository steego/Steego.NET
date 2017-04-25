#load "LoadWebServer.fsx"

open System.IO
open Steego.Web.Explorer
open Steego.Reflection.Navigators

type Tree<'a>(value:'a, getChildren: 'a -> seq<'a>) =
  member this.Value = value
  member this.Children = 
    [ for item in getChildren(value)->Tree(item, getChildren) ]

Tree(@"C:\Projects\github.com", Directory.EnumerateDirectories).Explore(2)