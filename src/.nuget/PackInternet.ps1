Import-Module "$PSScriptRoot\PowerPack" -Force

# Edit this file to suit your projects. See the README for more info.
$projects = @("Lithogen.Core", "Lithogen.Engine", "Lithogen")
$options.NuGetFeed = ""
$options.DoPush = $true
PowerPack $projects

