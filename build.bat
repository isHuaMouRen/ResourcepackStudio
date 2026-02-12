@echo off
setlocal enabledelayedexpansion

rd publish /s

dotnet publish "RresourcepackStudio\RresourcepackStudio.csproj" -c Release -o "publish\bin"
dotnet publish "ExecuteShell\ExecuteShell.csproj" -c Release -o "publish"
dotnet publish "UpdateService\UpdateService.csproj" -c Release -o "publish\bin"

del "publish\Resourcepack Studio.deps.json"
del "publish\Resourcepack Studio.pdb"
del "publish\bin\RresourcepackStudio.deps.json"
del "publish\bin\RresourcepackStudio.pdb"
del "publish\bin\UpdateService.deps.json"
del "publish\bin\UpdateService.pdb"

pause