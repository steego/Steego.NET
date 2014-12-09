<Query Kind="Program">
  <Reference Relative="bin\Debug\FSharp.Core.dll">C:\Projects\github.com\steego\Steego.NET\Steego.Archives\bin\Debug\FSharp.Core.dll</Reference>
  <Reference Relative="bin\Debug\Steego.Archives.dll">C:\Projects\github.com\steego\Steego.NET\Steego.Archives\bin\Debug\Steego.Archives.dll</Reference>
  <NuGetReference>Rx-Main</NuGetReference>
  <Namespace>Steego.Archives</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
  <Namespace>System.Reactive.Joins</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.PlatformServices</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Reactive.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var folder = @"C:\Data\Test";
	
	var a = new FolderCompressor(folder)
		.Where(f => Path.GetExtension(f) == ".xml");
		
	a.ArchiveFiles
		.ToObservable()
	//	.Profile("Archive")
	//	.Profile("Get Files")
	//	.GroupBy (fm => fm.ArchiveFile)
	//	.Profile("Grouping")
	//	.Select (fm => new { fm.Key, Count = fm.Count() } )
	//	.OrderBy (fm => fm.Key)
	//	.Take(2)
	//	.Select (x => new { x.Key, Count = x.Count () })
		//.Profile()
		.Dump()
		;
}

