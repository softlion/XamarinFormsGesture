@rem NuGet.CommandLine is too old and does not support Xamarin.iOS libs
@rem @echo Please install the package "NuGet.CommandLine" from https://chocolatey.org/ before running this script
@rem @echo After chocolatey is installed, type: choco install NuGet.CommandLine

@echo Before running this script, download nuget.exe from @echo https://nuget.codeplex.com/releases/view/133091
@echo and put nuget.exe in the path.

@rem set /p nugetServer=Enter base nuget server url (with /): 
set nugetServer=https://www.nuget.org/api/v2/package/

del *.nupkg
set version=3.0.0
nuget pack XamarinFormsGesture.nuspec -Version "%version%"
nuget push Vapolia.XamarinFormsGesture.*.nupkg -Source %nugetServer%

rem pause

@rem set assembly info to version
@rem https://gist.github.com/derekgates/4678882
