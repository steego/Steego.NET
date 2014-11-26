del *.nupkg
msbuild Steego.Net.sln /m /t:rebuild /p:Configuration=Release
.nuget\nuget pack Steego.Conversions.nuspec
.nuget\nuget pack Steego.LinqPad.nuspec