# Async Test App

## Requirements
- .NET 8 SDK (8.0.x)
- .NET 8 runtime for executing the sample console app

## Local Development
1. Restore packages and build: `dotnet build AsyncTestApp.sln`
2. Run the console demo: `dotnet run --project AsyncDemo/AsyncDemo.csproj`
3. Execute the test suite: `dotnet test AsyncTestApp.sln`

## Dev Container
The repo includes `.devcontainer/devcontainer.json`, matching the preferred ".NET High-Performance Environment" image. To use it:
1. Open the folder in VS Code with the Dev Containers extension installed.
2. When prompted, reopen in container; VS Code will pull `mcr.microsoft.com/devcontainers/dotnet:8.0`.
3. The container automatically runs `dotnet restore AsyncTestApp.sln` (its `postCreateCommand`) and installs the requested extensions.

Once the container is ready, follow the same local development commands inside the container shell.

## Continuous Integration
Every push or pull request triggers `.github/workflows/dotnet-vulnerability-scan.yml`, which installs .NET 8 and runs the repository-wide PowerShell vulnerability scan against all non-test projects. Fix any reported vulnerabilities before merging.
