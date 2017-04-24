
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

Let's try it out

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