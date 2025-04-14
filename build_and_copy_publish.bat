@echo off
REM === Step 1: Publish the project as a single-file self-contained executable ===
dotnet publish -c Release -r win-x64 --self-contained true ^
    /p:PublishSingleFile=true ^
    /p:IncludeNativeLibrariesForSelfExtract=true

REM === Step 2: Define the output publish path ===
set PUBLISH_DIR=BigFileSynchronizer\bin\Release\net8.0\win-x64\publish

REM === Step 3: Delete old EXE and PDB in project root (if exist) ===
if exist BigFileSynchronizer.exe del BigFileSynchronizer.exe
if exist BigFileSynchronizer.pdb del BigFileSynchronizer.pdb

REM === Step 4: Copy new EXE and PDB to project root ===
copy "%PUBLISH_DIR%\BigFileSynchronizer.exe" BigFileSynchronizer.exe
if exist "%PUBLISH_DIR%\BigFileSynchronizer.pdb" copy "%PUBLISH_DIR%\BigFileSynchronizer.pdb" BigFileSynchronizer.pdb

echo Done.
pause
