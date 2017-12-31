# Steego.Fs.WinDump

This project is a prototype of an object printing utility.  

## Usage

To use:

```fsharp
open Steego.Fs.WinDump.Browser

// Opens an object browser window
let browser = Browser()

type Person(name:string, dob:DateTime) =
    member this.Name = name
    member this.DOB = dob

let bob = Person("Bob Jones", DateTime(1972, 3, 4))

// Dump the object to the browser
browser.Dump(bob)
```
