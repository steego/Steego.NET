namespace Steego.LinqPad.Elmish.Components

open Steego.LinqPad.Elmish
open Steego.LinqPad.Elmish.Props
open Steego.LinqPad.Elmish.Helpers   

module CSS = 

    let resetCss = 
        link [ Rel "stylesheet"
               Href "https://cdnjs.cloudflare.com/ajax/libs/meyer-reset/2.0/reset.css"
               Integrity "sha256-7VVaJ5GDwFQiLIc+eNksQLUSSY5JNZtqv9o2BI8UGYg="
               CrossOrigin "anonymous" ]

    let bulmaCss = 
        link [ Rel "stylesheet"
               Href "https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.4/css/bulma.min.css"
               Integrity "sha256-8B1OaG0zT7uYA572S2xOxWACq9NXYPQ+U5kHPV1bJN4="
               CrossOrigin "anonymous" ]

    let bootstrapCss = 
        link [ Rel "stylesheet"
               Href "https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"
               Integrity "sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T"
               CrossOrigin "anonymous" ]

    let jQuery = 
        script [ Src "https://code.jquery.com/jquery-3.3.1.slim.min.js"
                 Integrity "sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo"
                 CrossOrigin "anonymous" ]
               [ ]
    
    let popperJs =
        script [ Src "https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"
                 Integrity "sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1"
                 CrossOrigin "anonymous" ]
               [ ]

    let bootStrapJs =
        script [ Src "https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"
                 Integrity "sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM"
                 CrossOrigin "anonymous" ]
               [ ]

module Helpers = 
    open System
    open System.Web

    //let raw(s:string) = 
    //    { new ReactElement with 
    //        member this.ToHtmlString() = s }

    //let inline htmlStringToElement(h:IHtmlString) = 
    //    { new ReactElement with 
    //        member this.ToHtmlString() = h.ToHtmlString() }

    let renderPrimitive(value:obj) = 
        match value with
        | :? ReactElement as t -> t
        | value -> HTMLNode.Object(value) :> ReactElement

module UI = 
    open Helpers
    open Reflection.Utils

    [<AbstractClass>]
    type BaseComponent() = 
        abstract member Render: unit -> ReactElement
        member this.ToDump() = this.Render() :> obj
        interface ReactElement with
            // member this.ToHtmlString() = this.Render().ToHtmlString()
            member this.ToDump() = this.ToDump()

    type Grid<'a>(value:'a) = 
        inherit BaseComponent()
        member this.Value = value
        override this.Render() = 
            let getters = TypeInfo<'a>.GetMemberGetters()
            table [ Class("table table-bordered table-striped") ] [
                for g in getters do
                    let o = g.Get.Invoke(value)
                    yield tr [] [
                            yield th [] [ str(g.Name) ]
                            yield td [] [ renderPrimitive(o) ]
                    ]
            ]
        //interface ReactElement with
        //    member this.ToHtmlString() = this.Render().ToHtmlString()

    type Tabs<'a>(prefix:string, value:'a) = 
        inherit BaseComponent()
        let properties = TypeInfo<'a>.GetMemberGetters()
                            |> Array.mapi(fun id x -> (id, x))

        override this.Render() = 
            div [] [

                ul [Class("nav nav-tabs"); Role("tablist")] [
                    for (id, p) in properties do
                        yield li [Role("presentation"); Class((if id = 0 then "active nav-item" else "nav-item"))] [
                                
                                a [ Class((if id = 0 then "active nav-link" else "nav-link")); 
                                    Href("#" + p.Name); AriaControls(p.Name); Role("tab"); DataToggle("tab")] [
                                    str(p.Name) 
                                ]
                            ]
                ]

                div [Class("tab-content")] [
                    for (id, p) in properties do
                        let o = p.Get.Invoke(value)
                        let className = if id = 0 then "tab-pane active" else "tab-pane"
                        yield div [Role("tabpanel"); Class(className); Id(p.Name); ] [ 
                            renderPrimitive(o)
                        ] 
                    ]
            ]
        //interface ReactElement with
        //    member this.ToHtmlString() = this.Render().ToHtmlString()

    type Panels<'a>(value:'a) = 
        inherit BaseComponent()
        let properties = TypeInfo<'a>.GetMemberGetters()
        override this.Render() = 
            div [] [
                for p in properties do
                    let o = p.Get.Invoke(value)
                    yield div [Class("panel panel-primary")] [
                            div [Class("panel-heading")] [ 
                                    h1 [Class("panel-title")] [ str(p.Name) ]
                                ]
                            div [Class("panel-body")] [ renderPrimitive(o) ]
                    ]
            ]
        //interface ReactElement with
        //    member this.ToHtmlString() = this.Render().ToHtmlString()

    type Table<'a>(source:seq<'a>) = 
        inherit BaseComponent()
        let properties = TypeInfo<'a>.GetMemberGetters()
        member this.Source = source
        member this.Properties = properties
        override this.Render() = 
            table [Class("table table-bordered table-striped")] [
                yield tr [] [
                        for g in properties do
                            yield th [] [ str(g.Name) ] 
                    ]
                
                for item in source do
                    yield tr [] [
                        for g in properties do
                            let o = g.Get.Invoke(item)
                            yield td [] [ renderPrimitive(o) ]
                    ]
            ]
        //interface ReactElement with
        //    member this.ToHtmlString() = this.Render().ToString()


    type Page<'a>(value:'a) = 
        inherit BaseComponent()
        let properties = TypeInfo<'a>.GetMemberGetters()
        override this.Render() = 
            div [] [            
                for g in properties do
                    let o = g.Get.Invoke(value)
                    yield h1 [] [ str(g.Name) ]
                    yield div [] [ renderPrimitive(o) ]
            ]


