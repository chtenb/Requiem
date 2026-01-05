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
Even for custom generators it is not necessary to write shrinkers for finding the minimal reproduction, [this happens automatically](#credits).

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

The generator API can be found in Requiem/Generate.cs. Have a look.

## Credits

- Requiem is currently built on top of [CsCheck](https://github.com/AnthonyLloyd/CsCheck), an excellent property-based testing library for C#.
  Requiem provides its own opiniated API and generators with enhanced edge case bias to make property-based testing more effective.
  The CsCheck library tracks size information of generated values, and skips larger values once a failure for a smaller value has been found.
  This process statistically converges to the smallest reproduction scenario, given enough time.
- Requiem utilizes the list of [NaughtyStrings](https://github.com/SimonCropp/NaughtyStrings) to help finding edge cases in string generation.

## License

See [LICENSE](LICENSE) file for details.
""");

// Write README
File.WriteAllText(readmePath, readme.ToString());
Console.WriteLine($"README.md generated successfully at {readmePath}");
