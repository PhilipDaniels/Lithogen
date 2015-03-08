function Get-Assembly-Version()
{
    param([string] $filename)

	# Retrieve the assembly version without loading the assembly. A naive version using
	#   $asm = [Reflection.Assembly]::LoadFile($exePath)
    #   $asm = [Reflection.Assembly]::ReflectionOnlyLoadFrom($exePath)
    #   $asmName = $asm.GetName()
	# will load the assembly into the powershell's appdomain and keep it locked, meaning
	# that the script is not rerunnable.
	#
	# This version does not suffer from that problem.

    $version = [System.Reflection.AssemblyName]::GetAssemblyName($filename).Version;
	return $version
}

# Clean up any previously built packages.
if (Test-Path Lithogen.*.nupkg) {
	Remove-Item Lithogen.*.nupkg
}

$cfg = "Release"

# Locate MSBuild. Valid versions are [2.0, 3.5, 4.0]
$dotNetVersion = "4.0"
$regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$dotNetVersion"
$regProperty = "MSBuildToolsPath"
$msbuild = join-path -path (Get-ItemProperty $regKey).$regProperty -childpath "msbuild.exe"

# Build the projects.
$projPath = Resolve-Path "..\src\Lithogen.TaskShim\Lithogen.TaskShim.csproj"
$args = "$projPath /p:Configuration=$cfg /nologo /verbosity:minimal"
$build = $msBuild + " " + $args
Invoke-Expression $build

$projPath = Resolve-Path "..\src\Lithogen\Lithogen.csproj"
$args = "$projPath /p:Configuration=$cfg /nologo /verbosity:minimal"
$build = $msBuild + " " + $args
Invoke-Expression $build

# Get the version number out of the assembly.
$exePath = Resolve-Path "..\src\Lithogen\bin\$cfg\Lithogen.exe"
$asmVersion = Get-Assembly-Version($exePath)
Write-Host "Lithogen.exe is at version $asmVersion"

# Build the nuspec with that version number.
$nuget = ".\nuget.exe pack ..\src\Lithogen\Lithogen.nuspec -NoPackageAnalysis -tool -Properties Configuration=$cfg -verbosity normal -version $asmVersion"
Invoke-Expression $nuget

dir *.nupkg
