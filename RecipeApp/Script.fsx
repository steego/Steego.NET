// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "../packages/Inflector.1.0.0.0/lib/net45/Inflector.dll"
#r "../packages/Newtonsoft.Json.6.0.6/lib/net45/Newtonsoft.Json.dll"

#load "FileUtilities.fs"
#load "App.fs"
open RecipeApp

let a = App()
a.Ingredients.Filename
a.Ingredients.Add(Ingredient("Apple"))
a.Ingredients.Index