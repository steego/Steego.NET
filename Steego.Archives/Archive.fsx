// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "./bin/Debug/Ionic.Zip.dll"

#load "Archive.fs"
open Steego.Archives

open System.IO
open System.Linq

let archiveFiles (map:seq<string> -> seq<FileArchiveMap>) folder = 
  let files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
  map files

let folder = "C:\Backups\Machines\72.3.254.99\d\Backup\Logs\W3SVC11"
let files = Directory.EnumerateFiles(folder, "*.log", SearchOption.AllDirectories)
files.ToArray()
//FileHelpers.archiveFolder (fun files -> for file in files)

