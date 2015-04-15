namespace RecipeApp

open System
open System.IO
open System.Linq
open System.Collections.Generic
//open OperatorIntrinsics.

module Settings = 
  let rootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Recipes")

type FileCollection<'item,'key>(key: 'item -> 'key) =
  let filename = Path.Combine(Settings.rootFolder, sprintf "%s.json" (typeof<'item>.Name.Pluralize()))
  let list = new List<'item>(Utilities.readListFromFile(filename))
  member this.Index = list.AsEnumerable()
  member this.Filename = filename
  member this.Add(item) =
    list.Add(item)
    Utilities.saveListToFile filename (list.AsEnumerable())
    this

type Ingredient(name:string) = 
  member this.Name = name

type Recipe(name:string) =
  member this.Name = name

type App() = 
  let ingredients = FileCollection(fun (item:Ingredient) -> item.Name)
  let recipies = FileCollection(fun (item:Recipe) -> item.Name)
  member this.Ingredients = ingredients
  member this.Recipies = recipies
