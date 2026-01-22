namespace Requiem;

/// <summary>
/// Simple utilities for test scenarios
/// </summary>
public class TestUtils
{
    private static DirectoryInfo? _solutionRoot;
    
    /// <summary>
    /// Find the directory of the solution. Useful for locating files that are not local to the test project.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static DirectoryInfo FindSolutionRoot()
    {
        if (_solutionRoot is not null)
            return _solutionRoot;
        
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir != null)
        {
            if (dir.GetFiles("*.sln").Length > 0 || dir.GetFiles("*.slnx").Length > 0)
            {
                _solutionRoot = dir;
                return dir;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate solution root.");
    }
}
