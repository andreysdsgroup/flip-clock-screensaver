@echo off
setlocal

echo Building Fliqlo Offline Screensaver...

if not exist dist mkdir dist

where csc >nul 2>nul
if %ERRORLEVEL% equ 0 (
    set CSC_EXE=csc
    goto :COMPILE
)

if exist "E:\Soft\VisualStudio\MSBuild\Current\Bin\Roslyn\csc.exe" (
    set CSC_EXE="E:\Soft\VisualStudio\MSBuild\Current\Bin\Roslyn\csc.exe"
    goto :COMPILE
)

if exist "%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe" (
    set CSC_EXE="%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe"
    goto :COMPILE
)

if exist "%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\csc.exe" (
    set CSC_EXE="%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
    goto :COMPILE
)

echo Roslyn csc.exe compiler not found, falling back to dotnet build...
dotnet build Fliqlo.csproj -c Release
goto :DONE

:COMPILE
echo Using compiler: %CSC_EXE%
%CSC_EXE% /target:winexe /out:dist\Fliqlo.scr /platform:anycpu /optimize+ Fliqlo.cs LocalServer.cs WebAssets.cs Resources.cs

:DONE
if exist dist\Fliqlo.scr (
    echo [SUCCESS] dist\Fliqlo.scr built successfully!
) else (
    echo [ERROR] Build failed!
)
endlocal
