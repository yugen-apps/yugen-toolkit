# msbuild C:\Dev\Yugen.Toolkit\Yugen.Toolkit.Uwp\Yugen.Toolkit.Uwp.csproj /p:Configuration=Release /p:TargetFramwork=uap10.0.18362 
# msbuild C:\Dev\Yugen.Toolkit\Yugen.Toolkit.Uwp\Yugen.Toolkit.Uwp.csproj /p:Configuration=Release /p:TargetFramwork=uap10.0.18362 /p:AppTargetFramework=netstandard2.0
# msbuild C:\Dev\Yugen.Toolkit\Yugen.Toolkit.Uwp\Yugen.Toolkit.Uwp.csproj /p:Configuration=Release /p:TargetFramwork=netstandard2.0 /p:AppTargetFramework=uap10.0.18362

$MsBuild = "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
$Configuration = "Release"
$Version = "1.0.0"
$PackageOutputPath = "D:\packages\NuGetLocal"
$Projects = @(
    "D:\yugen-toolkit\Yugen.Toolkit.Standard",
    "D:\yugen-toolkit\Yugen.Toolkit.Standard.Core",
    "D:\yugen-toolkit\Yugen.Toolkit.Standard.Data",
    "D:\yugen-toolkit\Yugen.Toolkit.Uwp",
    "D:\yugen-toolkit\Yugen.Toolkit.Uwp.Audio.Controls",
    "D:\yugen-toolkit\Yugen.Toolkit.Uwp.Audio.Services.Abstractions",
    "D:\yugen-toolkit\Yugen.Toolkit.Uwp.Audio.Services.Bass",
    "D:\yugen-toolkit\Yugen.Toolkit.Uwp.Controls", 
    "D:\yugen-toolkit\Yugen.Toolkit.Uwp.Mvvm"
)

foreach ($Project in $Projects) {
    Write-Host "--- Packing $Project... ---"

    & "$MsBuild" "$Project" `
        /p:Configuration="$Configuration" `
        /p:PackageVersion="$Version" `
        /p:Version="$Version" `
        /p:PackageOutputPath="$PackageOutputPath" `
        /t:pack `
        /v:q
}