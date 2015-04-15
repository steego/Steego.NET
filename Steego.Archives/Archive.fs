namespace Steego.Archives

open System
open System.IO
open System.Linq
open Ionic.Zip

type FileArchiveMap = { Filename:string; ArchiveFile:string }

module FileHelpers  =
  let Hourly prefix (file:string) =
    let created = File.GetLastWriteTime(file)
    let datePart = created.ToString("yyyy-MM-dd")
    sprintf "%s-%s.zip" prefix datePart
  let ZipFiles (zipFile:string) (files:string[]) =
    use a = new ZipFile(zipFile)
    for sourceFile in files do
      let entryName = Path.GetFileName(sourceFile)
      a.AddFile(sourceFile, "") |> ignore
    zipFile
  let DeleteFiles (files:string[]) =
    for f in files do File.Delete(f)
  let ArchiveFolder (archiveFileMap: seq<string> -> seq<FileArchiveMap>) (folder:string) =
    let mappedFiles = archiveFileMap(Directory.EnumerateFiles(folder))
    let fileMap = query { for m in mappedFiles do
                          groupValBy m.Filename m.ArchiveFile into archives
                          select (archives.Key,archives.ToArray()) }
    seq { for (archiveFile, files) in fileMap do 
          ZipFiles archiveFile files |> ignore
          yield sprintf "Archived %s" archiveFile
          DeleteFiles files
          yield sprintf "Deleted %i files for %s" (files.Count()) archiveFile }

type FolderCompressor(folder:string,getArchiveFile:string -> string,filter:string -> bool) =
  member this.Folder = folder
  member this.FileMap =
    query { for file in Directory.EnumerateFiles(folder) do
            where(filter(file))
            let archive = Path.Combine(folder, getArchiveFile(file))
            groupValBy file archive into archives
            let archiveFile = archives.Key
            let files = archives |> Seq.sort |> Seq.toArray
            sortBy archiveFile
            select (archiveFile, files) }
  member this.ArchiveFiles = 
    let getArchiveFileMap files = 
      seq { for file in files do
            if filter(file) then
              let archive = Path.Combine(folder, getArchiveFile(file))
              yield { ArchiveFile = archive; Filename = file } }
    FileHelpers.ArchiveFolder getArchiveFileMap folder
  member this.SetMap(getFileName:Func<string,string>) = 
    FolderCompressor(folder, getFileName.Invoke, filter)
  member this.Where(filter:Func<string,bool>) =
    FolderCompressor(folder, getArchiveFile, filter.Invoke)
  public new(folder) = 
    let folderName = Path.GetFileName(folder)
    FolderCompressor(folder, FileHelpers.Hourly "archive", (fun filename -> true))
  
  

