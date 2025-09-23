# CoreMvvmLib Integrated NuGet Package Build and Deploy Script

param(
    [switch]$SkipConfirm,
    [string]$Configuration = "Release",
    [switch]$PackOnly,
    [ValidateSet("major", "minor", "patch")]
    [string]$BumpVersion = ""
)

Write-Host "===============================================" -ForegroundColor Green
Write-Host "CoreMvvmLib Integrated NuGet Package Build and Deploy" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green

# Load environment variables from .env file
$envFile = ".env"
if (-not (Test-Path $envFile)) {
    Write-Error ".env file not found."
    exit 1
}

Write-Host "Loading environment variables from .env file..." -ForegroundColor Yellow
Get-Content $envFile | ForEach-Object {
    if ($_ -match '^([^#][^=]*)\s*=\s*(.*)$') {
        [Environment]::SetEnvironmentVariable($matches[1], $matches[2], "Process")
    }
}

# Check environment variables
$nugetApiKey = [Environment]::GetEnvironmentVariable("NUGET_API_KEY")
$nugetSource = [Environment]::GetEnvironmentVariable("NUGET_SOURCE")
$projectsToPack = [Environment]::GetEnvironmentVariable("PROJECTS_TO_PACK")

if (-not $nugetApiKey) {
    Write-Error "NUGET_API_KEY is not set."
    exit 1
}

if (-not $nugetSource) {
    $nugetSource = "https://api.nuget.org/v3/index.json"
}

# Update version
if ($BumpVersion) {
    Write-Host "Updating version..." -ForegroundColor Yellow
    & ./update-version.ps1 -BumpType $BumpVersion -SkipConfirm
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Version update failed"
        exit 1
    }
}

Write-Host "Build Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "NuGet Source: $nugetSource" -ForegroundColor Cyan
Write-Host "Deploy Packages: $projectsToPack" -ForegroundColor Cyan
Write-Host ""

# Clean artifacts directory
Write-Host "Cleaning existing artifacts..." -ForegroundColor Yellow
if (Test-Path "artifacts") {
    Remove-Item "artifacts" -Recurse -Force
}
New-Item -ItemType Directory -Path "artifacts/packages" -Force | Out-Null

# Clean and restore entire solution
Write-Host ""
Write-Host "===============================================" -ForegroundColor Green
Write-Host "Solution Clean and Restore" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green

Write-Host "Cleaning solution..." -ForegroundColor Yellow
& dotnet clean -c $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "Solution clean failed"
    exit 1
}

Write-Host "Restoring solution..." -ForegroundColor Yellow
& dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Solution restore failed"
    exit 1
}

# Build dependency projects first (DLLs to be included in packages)
Write-Host ""
Write-Host "===============================================" -ForegroundColor Green
Write-Host "Dependency Project Build" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green

$dependencyProjects = @(
    "src/CoreMvvmLib.Core.Common",
    "src/CoreMvvmLib.Core.IOC",
    "src/CoreMvvmLib.Core.Services",
    "src/CoreMvvmLib.SourceGeneration",
    "src/CoreMvvmLib.Design",
    "src/CoreMvvmLib.Component"
)

foreach ($project in $dependencyProjects) {
    Write-Host "[$project] Building..." -ForegroundColor Cyan
    & dotnet build $project -c $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error "$project build failed"
        exit 1
    }
}

# Build and package main packages
Write-Host ""
Write-Host "===============================================" -ForegroundColor Green
Write-Host "Main Package Build and Packaging" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green

$packProjects = $projectsToPack -split ";"

foreach ($project in $packProjects) {
    if (-not $project.Trim()) { continue }

    Write-Host ""
    Write-Host "[$project] Building..." -ForegroundColor Cyan
    & dotnet build $project -c $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error "$project build failed"
        exit 1
    }

    Write-Host "[$project] Packaging..." -ForegroundColor Cyan
    & dotnet pack $project -c $Configuration --no-build --output "artifacts/packages"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "$project packaging failed"
        exit 1
    }
}

# Display created package list
Write-Host ""
Write-Host "===============================================" -ForegroundColor Green
Write-Host "Created NuGet Packages" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green
Get-ChildItem "artifacts/packages/*.nupkg" | ForEach-Object {
    Write-Host "📦 $($_.Name)" -ForegroundColor Yellow
}
Write-Host ""
Write-Host "Integrated Package Structure:" -ForegroundColor Cyan
Write-Host "- CoreMvvmLib: Core MVVM + IOC + Services + SourceGeneration" -ForegroundColor White
Write-Host "- CoreMvvmLib.WPF: WPF UI + Design + Component" -ForegroundColor White
Write-Host ""

# Create packages only and exit
if ($PackOnly) {
    Write-Host "Package creation completed! (Deployment skipped)" -ForegroundColor Green
    Write-Host "Packages are created in artifacts/packages folder." -ForegroundColor Green
    exit 0
}

# Deployment confirmation
if (-not $SkipConfirm) {
    $deployConfirm = Read-Host "Do you want to deploy to NuGet? (y/N)"
    if ($deployConfirm -ne "y" -and $deployConfirm -ne "Y") {
        Write-Host "Deployment canceled." -ForegroundColor Yellow
        Write-Host "Packages are created in artifacts/packages folder." -ForegroundColor Yellow
        exit 0
    }
}

# NuGet deployment
Write-Host ""
Write-Host "===============================================" -ForegroundColor Green
Write-Host "Deploying to NuGet..." -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green

Get-ChildItem "artifacts/packages/*.nupkg" | ForEach-Object {
    Write-Host "📤 [$($_.BaseName)] Deploying..." -ForegroundColor Cyan
    & dotnet nuget push $_.FullName --api-key $nugetApiKey --source $nugetSource
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "❌ $($_.Name) deployment failed (version may already exist)"
    } else {
        Write-Host "✅ [$($_.BaseName)] Deployment completed!" -ForegroundColor Green
    }
    Write-Host ""
}

Write-Host ""
Write-Host "===============================================" -ForegroundColor Green
Write-Host "🎉 Deployment Complete!" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green
Write-Host "All packages have been deployed to NuGet." -ForegroundColor Green
Write-Host "Please wait a few minutes to check on NuGet." -ForegroundColor Yellow
Write-Host ""
Write-Host "Usage:" -ForegroundColor Cyan
Write-Host "Install-Package CoreMvvmLib           # Core MVVM functionality" -ForegroundColor White
Write-Host "Install-Package CoreMvvmLib.WPF       # WPF UI components" -ForegroundColor White