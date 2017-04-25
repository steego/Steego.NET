#load "LoadWebServer.fsx"

open System.IO
open Steego.Web.Explorer
open Steego.Reflection.Navigators

type Folder(path:string) =
    member this.Name = Path.GetFileName(path)
    member this.Path = path
    member this.Files = Directory.EnumerateFiles(path)
    member this.SubFolders = 
        [ for p in Directory.EnumerateDirectories(path) -> Folder(p) ]

Folder(@"C:\Projects\github.com").Explore(2)