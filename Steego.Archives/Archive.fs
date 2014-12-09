namespace Steego.Archives

open System
open System.IO
open System.Linq
open System.IO.Compression

type FileArchiveMap(file:string, archiveFile:string) = 
  member this.File = file
  member thie.ArchiveFile = archiveFile

module FileHelpers  =
  let Hourly prefix (file:string) =
    let created = File.GetLastWriteTime(file)
    let datePart = created.ToString("yyyy-MM-dd")
    sprintf "%s-%s.zip" prefix datePart
  let zipFiles (zipFile:string) (files:string[]) =
    use a = ZipFile.Open(zipFile, ZipArchiveMode.Create)
    for sourceFile in files do
      let entryName = Path.GetFileName(sourceFile)
      a.CreateEntryFromFile(sourceFile, entryName, CompressionLevel.Optimal) |> ignore
    zipFile
  let deleteFiles (files:string[]) =
    for f in files do File.Delete(f)

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
    seq { for (archiveFile, files) in this.FileMap do 
            FileHelpers.zipFiles archiveFile files |> ignore
            yield sprintf "Archived %s" archiveFile
            FileHelpers.deleteFiles files
            yield sprintf "Deleted %i files for %s" (files.Count()) archiveFile
        }
  member this.SetMap(getFileName:Func<string,string>) = 
    FolderCompressor(folder, getFileName.Invoke, filter)
  member this.Where(filter:Func<string,bool>) =
    FolderCompressor(folder, getArchiveFile, filter.Invoke)
  public new(folder) = 
    let folderName = Path.GetFileName(folder)
    FolderCompressor(folder, FileHelpers.Hourly "archive", (fun filename -> true))
  
  

