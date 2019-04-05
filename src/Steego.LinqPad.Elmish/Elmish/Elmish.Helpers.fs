namespace Steego.LinqPad.Elmish

open Steego.LinqPad.Elmish.Props
open Steego.LinqPad.Elmish

type ReactElementType<'props> = interface end

type ReactComponentType<'props> =
    inherit ReactElementType<'props>
    abstract displayName: string option with get, set

type ServerElementType =
    | Tag
    | Fragment
    | Component

type ReactElementTypeWrapper<'P> =
    | Comp of obj
    | Fn of ('P -> ReactElement)
    | HtmlTag of string
    interface ReactElementType<'P>


[<AutoOpen>]
module Helpers =



    [<RequireQualifiedAccess>]
    module ServerRendering =
        let [<Literal>] private ChildrenName = "children"

        let private createServerElementPrivate(tag: obj, props: obj, children: obj seq, elementType: ServerElementType) =
            match elementType with
            | ServerElementType.Tag ->
                HTMLNode.Node (string tag, props :?> IProp seq, children) :> ReactElement
            | ServerElementType.Fragment ->
                HTMLNode.List children :> ReactElement
            | ServerElementType.Component ->
                let tag = tag :?> System.Type
                let comp = System.Activator.CreateInstance(tag, props)
                let childrenProp = tag.GetProperty(ChildrenName)
                childrenProp.SetValue(comp, children |> Seq.toArray)
                let render = tag.GetMethod("render")
                render.Invoke(comp, null) :?> ReactElement

        // In most cases these functions are inlined (mainly for Fable optimizations)
        // so we create a proxy to avoid inlining big functions every time

        let createServerElement(tag: obj, props: obj, children: obj seq, elementType: ServerElementType) =
            createServerElementPrivate(tag, props, children, elementType)

    [<RequireQualifiedAccess>]
    module ReactElementType =
        let inline ofHtmlElement<'props> (name: string): ReactElementType<'props> =
            HtmlTag name :> ReactElementType<'props>


// TODO: Move conditional compilation to the body of the functions?

    /// Alias of `ofString`
    let inline str (s: string): ReactElement = HTMLNode.Text s :> ReactElement

    /// Cast a string to a React element (erased in runtime)
    let inline ofString (s: string): ReactElement = str s

    /// Cast an option value to a React element (erased in runtime)
    let inline ofOption (o: ReactElement option): ReactElement =
        match o with Some o -> o | None -> null // Option.toObj(o)

    /// OBSOLETE: Use `ofOption`
    [<System.Obsolete("Use ofOption")>]
    let inline opt (o: ReactElement option): ReactElement = ofOption o

    
    
    /// Cast an int to a React element (erased in runtime)
    let inline ofInt (i: int): ReactElement = HTMLNode.RawText (string i) :> ReactElement

    /// Cast a float to a React element (erased in runtime)
    let inline ofFloat (f: float): ReactElement = HTMLNode.RawText (string f) :> ReactElement

    /// Returns a list **from .render() method**
    let inline ofList (els: obj list): ReactElement = HTMLNode.List els :> ReactElement

    /// Returns an array **from .render() method**
    let inline ofArray (els: obj array): ReactElement = HTMLNode.List els :> ReactElement

    /// A ReactElement when you don't want to render anything (null in javascript)
    let nothing: ReactElement = HTMLNode.Empty :> ReactElement

    /// Instantiate a DOM React element
    let inline domEl (tag: string) (props: IHTMLProp seq) (children: obj seq): ReactElement =
        ServerRendering.createServerElement(tag, (props |> Seq.cast<IProp>), children, ServerElementType.Tag)

    /// Instantiate a DOM React element (void)
    let inline voidEl (tag: string) (props: IHTMLProp seq) : ReactElement =
        ServerRendering.createServerElement(tag, (props |> Seq.cast<IProp>), [], ServerElementType.Tag)

    /// Instantiate an SVG React element
    let inline svgEl (tag: string) (props: IProp seq) (children: obj seq): ReactElement =
        ServerRendering.createServerElement(tag, (props |> Seq.cast<IProp>), children, ServerElementType.Tag)

    // Class list helpers
    let classBaseList baseClass classes =
        classes
        |> Seq.choose (fun (name, condition) ->
            if condition && not(System.String.IsNullOrEmpty(name)) then Some name
            else None)
        |> Seq.fold (fun state name -> state + " " + name) baseClass
        |> ClassName

    let classList classes = classBaseList "" classes

