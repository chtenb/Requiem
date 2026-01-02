# justfile for Requiem library
# https://github.com/casey/just

# Default recipe to display help information
default:
    @just --list

# Build the Requiem solution
build:
    dotnet build Requiem.slnx

# Build in release mode
build-release:
    dotnet build Requiem.slnx -c Release

# Clean build artifacts
clean:
    dotnet clean Requiem.slnx
    find . -type d -name "bin" -o -name "obj" | xargs rm -rf

# Restore NuGet packages
restore:
    dotnet restore Requiem.slnx

# Run all tests
test-all:
    dotnet test Requiem.Tutorial/Requiem.Tutorial.csproj

# Run a specific test by containing word
test WORD:
    dotnet test Requiem.Tutorial/Requiem.Tutorial.csproj --filter "FullyQualifiedName~{{WORD}}_"

# Run tests with verbose output
test-verbose:
    dotnet test Requiem.Tutorial/Requiem.Tutorial.csproj -v detailed

# Format code using dotnet format
format:
    dotnet format Requiem.slnx

# Check code formatting without making changes
format-check:
    dotnet format Requiem.slnx --verify-no-changes

# Generate README.md from tutorial files
readme:
    dotnet script generate-readme.csx

# Pack the Requiem library as NuGet package
pack:
    dotnet pack Requiem/Requiem.csproj -c Release -o ./artifacts

# Pack with specific version
pack-version VERSION:
    dotnet pack Requiem/Requiem.csproj -c Release -o ./artifacts /p:Version={{VERSION}}

# Check for outdated packages
outdated:
    dotnet list Requiem.slnx package --outdated

# Full CI pipeline: restore, build, test
ci: restore build test-all

# Full local check: format, build, test, readme
check: format build test-all readme

# Clean, restore, and build
rebuild: clean restore build
