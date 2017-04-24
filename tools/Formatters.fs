module internal FsReveal.Formatters

open FSharp.Markdown
open FSharp.Literate

let transformation (value:obj, typ:System.Type) =
    match value with
    //| :? string as s -> Some([InlineBlock("<h1>String: " + s + "</h1>")])
    | _ -> None

let register(fsiEvalutor:FsiEvaluator) = 
    fsiEvalutor.RegisterTransformation(transformation)