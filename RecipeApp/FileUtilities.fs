namespace RecipeApp

open System
open System.Linq
open System.IO
open Newtonsoft

module Utilities =
  let EnsureDirectory(filename:string) =
    let path = Path.GetDirectoryName(filename)
    Directory.CreateDirectory(path) |> ignore
    filename
  let readListFromFile (filename:string) =
    if File.Exists(filename) then
      use sr = new StreamReader(filename)
      use r = new Json.JsonTextReader(sr)
      let s = Json.JsonSerializer()
      s.Deserialize<'a[]>(r)
    else 
      [||]
  let saveListToFile (filename:string) (list:seq<'a>) =
    let array = list.ToArray()
    use sw = new StreamWriter(EnsureDirectory(filename))
    let s = Json.JsonSerializer()
    s.Serialize(sw, array)