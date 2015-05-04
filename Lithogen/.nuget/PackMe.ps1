# Example of packing a project. For more, type 'Get-Help PowerPack' after importing the module.
# The individual functions also have help available.
Import-Module .\PowerPack -Force

# Get latest version of NuGet into this folder: nuget restore requires at least v2.7.
if (!(Test-Path "NuGet.exe"))
{
	Invoke-NuGetDownload
}

Find-File "*.sln" | Invoke-NuGetRestore


$version = "0.1.209.0"
$configs = "Release"
$feed = "LocalNuGetFeed"
#$feed = $null

Find-Project "Lithogen" | Get-Project | % { $_.VersionNumber = $version; $_ } |
	Invoke-DeepClean | 
	% { $project = $_ ; $configs | % { Invoke-MSBuild $project -Configuration $_ } } |
	Sort -Unique | 
	Invoke-NuGetPack -Version $version -Properties @("Configuration=Release") |
	Invoke-NuGetPush -NuGetFeed $feed
