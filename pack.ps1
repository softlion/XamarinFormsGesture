if ($IsMacOS) {
    $msbuild = "msbuild"
} else {
    $vswhere = 'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe'
    $msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
    $msbuild = join-path $msbuild 'MSBuild\Current\Bin\MSBuild.exe'
}

#####################
#Build release config
$version="3.3.2"
$versionSuffix=""
$nugetVersion="$version$versionSuffix"
#$versionSuffix=".$env:BUILD_NUMBER" 

cd $PSScriptRoot
del *.nupkg
& $msbuild "XamarinFormsGestures.sln" /restore /p:Configuration=Release /p:Platform="Any CPU" /p:Version="$version" /p:VersionSuffix="$versionSuffix" /p:Deterministic=false /p:PackageOutputPath="$PSScriptRoot" --% /t:Clean;Build;Pack
if ($lastexitcode -ne 0) { exit $lastexitcode; }

####################
# PUSH
dotnet nuget push "Vapolia.XamarinFormsGesture.$nugetVersion.nupkg"
