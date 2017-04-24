module Steego.Html

open System
open System.IO

module Encoders =
    open System.Web
    let inline html(s:string) = HttpUtility.HtmlEncode(s)
    let inline attribute(s:string) = HttpUtility.HtmlAttributeEncode(s)

type Tag = 
    | Tag of Name:string * Attributes:Map<string, string> * Body:Tag list
    | Text of string
    member this.Write(w:TextWriter) = 
        match this with
        | Text(s) -> 
            w.Write("<span>")
            w.WriteLine(Encoders.html(s))
            w.Write("</span>")        
        | Tag(name, attributes, body) -> 
            w.Write(sprintf "<%s" name)
            for (name,value) in attributes |> Map.toList do
                w.Write(sprintf " %s=\"%s\"" (Encoders.attribute name) (Encoders.attribute value))
            w.WriteLine(">")
            for child in body do
                child.Write(w)
            w.WriteLine(sprintf "</%s>" name)
    override this.ToString() = 
        use sw = new StringWriter()
        this.Write(sw)
        sw.ToString()
        
let makeTag (name:string) = 
    let tag(attributes:(string * string) list) (body:Tag list) = 
        Tag(name, (Map.ofList attributes), body)    
    tag