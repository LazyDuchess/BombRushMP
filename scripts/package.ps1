param(
    [switch]$Release = $False
)

#Requires -Version 7.4
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

Set-Location $PSScriptRoot/..
[Environment]::CurrentDirectory = (Get-Location -PSProvider FileSystem).ProviderPath

$csprojPath = "BombRushMP.Plugin/BombRushMP.Plugin.csproj"
$projxml = [xml](Get-Content -Path $csprojPath)
$version = $projxml.Project.PropertyGroup[0].Version

if($Release) {
    $Configuration='Release'
} else {
    $Configuration='Debug'
}

function EnsureDir($path) {
    if(!(Test-Path $path)) { New-Item -Type Directory $path > $null }
}

function Clean() {
    if(Test-Path "Build/$Configuration") {
        Remove-Item -Recurse "Build/$Configuration"
    }
}

function CreateZip($zipPath) {
    if(Test-Path $zipPath) { Remove-Item $zipPath }
    $zip = [System.IO.Compression.ZipFile]::Open($zipPath, 'Create')
    return $zip
}

function ExtractZip($zipPath){
    $targetPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($zipPath),[System.IO.Path]::GetFileNameWithoutExtension($zipPath))
    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipPath, $targetPath)
}

function AddToZip($zip, $path, $pathInZip=$path) {
    if(Test-Path $path){
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $path, $pathInZip) > $Null
    }
}

function CreatePluginZip(){
    $zipPath = "Build/$Configuration/BombRushMP.Plugin.$Configuration-$version.zip"
    $readmePath = "README.md"
    $bundlePath = "BombRushMP.Editor/Build/assets"
    $zip = CreateZip $zipPath

    Push-Location "Thunderstore"
    Get-ChildItem -Recurse -File './' | ForEach-Object {
        $path = ($_ | Resolve-Path -Relative).Replace('.\', '')
        AddToZip $zip $_.FullName $path
    }
    Pop-Location

    Push-Location "BombRushMP.Plugin/bin/$Configuration/net462"
    Get-ChildItem -Recurse -File -Filter *.dll | ForEach-Object {
        $path = ($_ | Resolve-Path -Relative).Replace('.\', '')
        AddToZip $zip $_.FullName $path
    }
    Get-ChildItem -Recurse -File -Filter *.config | ForEach-Object {
        $path = ($_ | Resolve-Path -Relative).Replace('.\', '')
        AddToZip $zip $_.FullName $path
    }
    if (!$Release){
        Get-ChildItem -Recurse -File -Filter *.pdb | ForEach-Object {
            $path = ($_ | Resolve-Path -Relative).Replace('.\', '')
            AddToZip $zip $_.FullName $path
        }
    }
    Pop-Location

    AddToZip $zip $readmePath $readmePath

    AddToZip $zip $bundlePath "assets"

    $zip.Dispose()

    ExtractZip $zipPath
}

Clean
dotnet build -c $Configuration
EnsureDir "Build/$Configuration"
CreatePluginZip