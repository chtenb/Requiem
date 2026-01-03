#!/usr/bin/env dotnet-script

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// Configuration
var tutorialDir = Path.Combine(Directory.GetCurrentDirectory(), "Requiem.Tutorial");
var readmePath = Path.Combine(Directory.GetCurrentDirectory(), "README.md");

// Read all tutorial files in order
var tutorialFiles = Directory.GetFiles(tutorialDir, "*.cs")
    .OrderBy(f => Path.GetFileName(f))
    .ToList();

var readme = new StringBuilder();

// Header
readme.Append("""
# Requiem

A property-based testing library for C# with built-in edge case bias. Requiem makes it easier to find bugs by automatically generating problematic values that often break code.
Works with any unit test framework.

## Tutorial

""");

// Process each tutorial file
foreach (var file in tutorialFiles)
{
    var content = File.ReadAllText(file);
    var fileName = Path.GetFileName(file);
    
    // Include the entire file content
    readme.AppendLine("```csharp");
    readme.AppendLine(content);
    readme.AppendLine("```");
    readme.AppendLine();
}

// Footer
readme.Append("""
## Running the Examples

The generator API can be found in Gens.cs. Have a look.

## Credits

- Requiem is currently built on top of [CsCheck](https://github.com/AnthonyLloyd/CsCheck), an excellent property-based testing library for C#.
  Requiem provides its own opiniated API and generators with enhanced edge case bias to make property-based testing more effective.
- Requiem utilizes the list of [NaughtyStrings](https://github.com/SimonCropp/NaughtyStrings) to help finding edge cases in string generation.

## License

See [LICENSE](LICENSE) file for details.
""");

// Write README
File.WriteAllText(readmePath, readme.ToString());
Console.WriteLine($"README.md generated successfully at {readmePath}");
