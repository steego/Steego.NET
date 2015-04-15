# Steego.Trees

A C# library implementing a lazy tree object.

## Usage

```csharp

using Steego.Trees;

//  Create a simple node
var node = Trees.Create(1);

//  Explore folders 4 levels deep, but only folders whose name doesn't start with "$"
var folders = Trees.Create(@"C:\", f => Directory.GetDirectories(f))
  .Select(f => new { Parent = Path.GetDirectoryName(f), Name = Path.GetFileName(f) })
  .Where(f => !f.Name.StartsWith("$"))
  .MaxDepth(4);

```
