module Steego.Fs.Html

open System
open System.IO

module Encoders =
    open System.Web
    let inline html(s:string) = System.Net.WebUtility.HtmlEncode(s)
    let inline attribute(s:string) = System.Net.WebUtility.HtmlEncode(s)
    let inline url(s:string) = System.Net.WebUtility.UrlEncode(s)

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

type IHtmlObject = 
    abstract member ToHtml: Tag

module StandardTags = 
    let html = makeTag "html"
    let head = makeTag "head"
    let style = makeTag "style"
    let link = makeTag "link"
    let body = makeTag "body"
    let a = makeTag "a"
    let div = makeTag "div"
    let table = makeTag "table"
    let thead = makeTag "thead"
    let tbody = makeTag "tbody"
    let tr = makeTag "tr"
    let th = makeTag "th"
    let td = makeTag "td"

module BasicTemplates = 
    open StandardTags

    let styleSheet href = link [("rel", "stylesheet"); ("type", "text/css"); ("href", href); ] []

    let cssPage cssHrefs content =
        html [] [
          head [] [
              for href in cssHrefs do
                yield styleSheet href
            ]
          body [] content
          ]