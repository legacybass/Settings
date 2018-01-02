@echo off

echo Building Settings

dotnet build --configuration Release ..\Settings
REM dotnet pack ..\src\Settings --output ..\Releases\ --configuration Release

echo Building Settings

nuget pack ..\Settings\Settings.nuspec -OutputDirectory ..\Releases\ -Build -Properties Configuration=Release



