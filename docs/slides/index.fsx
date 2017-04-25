
(**
- title : Exploring POCOs with Reflection!
- description : Introduction to React Native with F#
- author : Steve Goguen
- theme : night
- transition : default
**)
(*** hide ***)
open System

(**

## Exploring POCOs with Reflection

<br />
#### and
<br />

### Automatically Generating Interfaces with F#

<br />
<br />
Steve Goguen - [@sgoguen](http://www.twitter.com/sgoguen)

' Time: 1 minute

---

## Disclaimer

* This talk is about exploring an idea and sparking a discussion.
* I'm not an expert and I don't advocate the methods or code in this talk.
* Specifically, some examples and ideas will likely introduce attack vectors to your systems
* Despite all this:  I think these ideas are worth discussing.

' Time: 1 minute

---

### A Little Forewarning

* This talk takes a paper mache & scaffold first approach.
* I will be a little heritical at times.
* Some code may offend type-safe sensibilities.
* Some of it will be fixed to type-safe versions throughout the talk.

<br/>
<br/>
#### I want to hear objections, but please keep an open mind.

---

### Tonight's Agenda

* Exploring POCOs - Reflecting objects to map URLs
* Displaying POCOs - Creating an extensible Object Dumper
* Meta Programming - Using Pattern Matching with Reflection to Create Useful Functions

' Time: 1 minute

***

## Act 1
# Exploring POCOs

---

### Act 1 - Scene 1
## Using LINQPad to Explore Lazy Trees

---

### An Lazy N-Ary Tree-like Structure

    type Tree<'a>(value:'a, getChildren: 'a -> seq<'a>) = 
        member this.Value = value
        member this.Children = 
            [ for item in getChildren(value) -> yield Tree(item, getChildren) ]

' Time: 1.5 minute

---

## LINQPad is a helluva tool

<img data-gifffer="images/lazy-tree-fast.gif" src="images/lazy-tree-fast.gif" style="background: transparent; border-style: none;"  width=800 />

' Time: 1.5 minute

---

<strike>Let's try it out</strike>

My virus scanner deleted LINQPad :'(



' Time: 1 minute

---

### Act 1 - Scene 2
## Navigating Objects with URL Paths

---

### What do we need to navigate an object?

    let TryGetProperty (name:string) (o: obj) : obj option = 
        try Some(Fasterflect.PropertyExtensions.GetPropertyValue(o, name))
        with ex -> None

* It simply accepts any .NET object
* It returns Some(value) if the property exists
* It uses the Fasterflect library for simplicity

' Time: 1 minute

---

### What do we need to navigate an object?

    let TryGetProperty (name:string) (o: obj) : obj option = 
        try Some(Fasterflect.PropertyExtensions.GetPropertyValue(o, name))
        with ex -> None

* It's also not type safe
* Uses exception handling for a common case

' Time: 1 minute

---


### Navigating a Property

    let TryGetProperty : string -> obj -> obj option = fun name o ->
        try Some(Fasterflect.PropertyExtensions.GetPropertyValue(o, name))
        with ex -> None

### How to use it

    type Person = { Name: string; DOB: DateTime }
    let taylor = { Name = "Taylor"; DOB = new DateTime(1989, 12, 13) }

    taylor |> TryGetProperty "Name" //  Some("Taylor")
    taylor |> TryGetProperty "FirstName" //  None

---

### How about primary indexed properties?

    let TryGetIndexer : string -> obj -> obj option = fun name o ->
        try Some(Fasterflect.PropertyExtensions.GetIndexer(o, name))
        with ex -> None

### How to use it

    type Folder(path:string) = 
        member this.Path = path
        member this.Subfolders = 
            Directory.EnumerateDirectories(path)
        member this.Item with get(name:string) = 
            Folder(Path.Combine(path, name))

    Folder(root)  |> TryGetIndexer "github.com"

Let's see how this works.

---

###  Navigating a path could be useful

    let TryNavPath (path:string list) (o:obj) : obj option = begin
        let navSegment (o:obj option) (segment:string) = 
            match o with
            | Some(o:obj) when o <> null -> TryGetIndexer segment o
            | _ -> None

        path |> List.fold navSegment (Some(o))
    end

### How to use it

    Folder(root) 
        |> TryNavPath ["github.com"; "steego"; "toychest"]
        |> Dump

---

## Let's Map URL's to it

    let NavigatePath(path:string)(o:obj) = 
        let path = path.Split("/") |> List.ofArray
        o |> TryNavPath path

What could possibly go wrong?

---

##  Let's create a simple web server

    open Suave

    let app2 = (fun(ctx : HttpContext) ->
        async {
            let message = "Welcome " + ctx.request.path
            return! Successful.OK message  ctx
        })

    let url = defaultConfig.bindings.First().ToString()

    System.Diagnostics.Process.Start(url)

    startWebServer defaultConfig app2


*** 

# Object Printers

---

#### Give me a type, I'll give you an unsafe HTML printer

    open Fasterflect

    let safeGet(name)(o:obj) = 
        try PropertyExtensions.GetPropertyValue(o, name)
        with _ -> null

    let makePrinter<'a>() : 'a -> string = 
        let properties = [for p in typeof<'a>.Properties() do
                            yield (p.Name, safeGet) ]

        fun (input:'a) : string -> 
            let sb = new StringBuilder()
            sb.AppendLine("<table>")
            for (propName, safeGet) in properties do
            sb.AppendLine("<tr>")
            sb.AppendLine(sprintf "<th>%s</th>" propName)
            let propValue = safeGet input
            sb.AppendLine(sprintf "<td>%s</td>"  propValue)
            sb.AppendLine("</tr>")
            sb.AppendLine("</table>")
            sb.ToString()

---

##  Can we approach approach the problem with typed HTML?

    type Tag = 
        | Tag of Name:string * Attributes:Map<string, string> * Body:Tag list
        | Text of string
    
---

## We could write it to a stream...

    let writeTo(w:TextWriter) (t:Html.Tag) =
        match t with
        
        //  Match regular text
        | Text(s) ->  
            w.Write("<span>")
            w.WriteLine(Encoders.html(s))
            w.Write("</span>")        
        
        //  Match a tag
        | Tag(name, attributes, body) -> 
            w.Write(sprintf "<%s" name)
            for (name,value) in attributes |> Map.toList do
                let encName = Encoders.attribute name
                let encValue = Encoders.attribute name
                w.Write(sprintf " %s=\"%s\"" encName encValue)
            w.WriteLine(">")
            for child in body do
                child.Write(w)
            w.WriteLine(sprintf "</%s>" name)

---

#### Give me a type, I'll give you an safer HTML printer

    let makePrinter<'a>()  = 
        let properties = [for p in typeof<'a>.Properties() do
                            yield (p.Name, safeGet) ]

        fun (input:'a) : Html.Tag ->
            table [] [
                for (propName, safeGet) in properties do
                    let propValue = safeGet input
                    tr [] [
                        th [] [ Html.Text(propName) ]
                        td [] [ Html.Text(propValue) ]
                    ]
            ]

---

#### Or we can simply do type tests to get started

    let rec print (level:int) (o:obj) : Html.Tag =         
        let nextLevel = level - 1
        match o with
        | null -> Html.Text("<null>")
        | IsPrimitive(n) -> Html.Text(n.ToString())
        | IsNullable(n) -> print level n
        | GenericList(getters, list) -> 
                printGenericList nextLevel  getters list
        | IsSeq(s) -> printList nextLevel s
        | Object(members, _, _, _, _) -> 
                printObject nextLevel members o
        | _ -> Html.Text("...")        

---

####  Dumping with some Context

    type NavContext = { Value:obj; Path: string list }
    
    let ToContext(value:'a) = { Value = value :> obj; Path = [] }
    let addPath(segment,newValue) (ctx:NavContext) = 
        { Value = newValue :> obj; Path = segment::ctx.Path }


***

# Reflection
### Give me a Type, I'll give you a...

    type TypeInfoGetter<'a> = Type -> 'a

---

<img src="images/type-to-func-fast.gif" style="background: transparent; border-style: none;"  width=800 />



*** 


### Thank you!

* https://github.com/fable-compiler/fable-elmish
* https://ionide.io
* https://facebook.github.io/react-native/


**)