namespace rec Steego.LinqPad.Elmish

open System.IO
open LINQPad

type [<AllowNullLiteral>] ReactElement =
    //abstract member ToHtmlString: unit -> string
    abstract member ToDump: unit -> obj

type HTMLNode =
    | Text of string
    | RawText of string
    | Node of string * IProp seq * obj seq
    | List of obj seq
    | DumpContainer of DumpContainer
    | Object of obj
    | Empty
    override this.ToString() = HTMLNode.renderToString(this)
    member this.ToDump() = LINQPad.Util.RawHtml(HTMLNode.renderToString(this))
    interface ReactElement with
        //member this.ToHtmlString() = HTMLNode.renderToString(this)
        member this.ToDump() = LINQPad.Util.RawHtml(HTMLNode.renderToString(this))


module HTMLNode = 
    open System.IO
    open Steego.LinqPad.Html.Utilities
    open Steego.LinqPad.Elmish.Render

    /// Cast a ReactElement safely to an HTMLNode.
    /// Returns an empty node if input is not an HTMLNode.


    
    let rec writeTo (html: TextWriter) (htmlNode: HTMLNode) : unit =
        match htmlNode with
        | HTMLNode.Text str -> escapeHtml html str
        | HTMLNode.RawText str -> html.Write str
        | HTMLNode.Node (tag, attrs, children) ->
          html.Write '<'
          html.Write tag

          let child = renderAttrs html attrs tag

          if voidTags.Contains tag then
            html.Write "/>"
          else
            html.Write '>'

            match child with
            | Some c -> html.Write c
            | None ->
              for child in children do
                writeElement html child

            html.Write "</"
            html.Write tag
            html.Write '>'
        | HTMLNode.List nodes ->
            for node in nodes do
                writeElement html node

        | HTMLNode.Empty -> ()
        | HTMLNode.DumpContainer(c) -> html.Write(Util.ToHtmlString(c))
        | HTMLNode.Object(c) -> html.Write(Util.ToHtmlString(c))

    and writeElement (html: TextWriter) (htmlNode: obj) =
      match htmlNode with
      | :? HTMLNode as node -> writeTo html node
      | r -> html.Write(Util.ToHtmlString(r))

    let renderToString (htmlNode: HTMLNode): string =
        use html = new StringWriter()
        htmlNode |> writeTo html
        html.ToString()
