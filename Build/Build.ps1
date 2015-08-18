#The ultimate build script to build release version of Terminals

# Define variables
[String[]]$packageFiles = .\PackageFiles.ps1

$outputDir = ".\Output\";
$binOutput = "..\Source\Terminals\bin\Release\";
$commonAssembly = "..\Source\Terminals\Properties\Common.AssemblyInfo.cs";
$setupPath = "..\Source\TerminalsSetup\bin\Release\TerminalsSetup.msi";
$msbuild = "c:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe";

# Extract the variable from the Common.AssemblyInfo.cs
$versionLine = (Select-String -path $commonAssembly "Version").Line
$versionStart = $versionLine.IndexOf("(""");
$versionEnd = $versionLine.IndexOf(".*");
$version = $versionLine.SubString($versionStart+2, $versionEnd - $versionStart -2);

#Define output paths
$installerName = "TerminalsSetup_$version.msi";
$zipName = "Terminals_v$version.zip";
$zipPath = Join-Path $outputDir $zipName;
$installerTargetPath = Join-Path $outputDir $installerName;

.\InstallPrerequisities.ps1

# Compile the solution - the distributionrelease configuration contains installer, which is not normal configurations
& $msbuild ..\Source\Terminals.sln /m /p:configuration=DistributionRelease /toolsversion:4.0 /t:clean /t:build
 
# Collect the outputs
if(!(Test-Path $outputDir))
{
    New-Item $outputDir -itemtype directory;
}

Remove-Item $outputDir\*.* -Recurse -Force;
Move-Item $setupPath $installerTargetPath;
#zip the deployable bits
for($index=0; $index -lt $packageFiles.Length;$index++)
{
   $packageFiles[$index] = Join-Path $binOutput $packageFiles[$index];
}

Write-Zip -Path $packageFiles $zipPath;




