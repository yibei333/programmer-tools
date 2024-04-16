@echo off

cd /d "%~dp0"
set sourcePath=%cd%\..\src
set binaryPath=%sourcePath%\ProgrammerTools\bin\packages
set targetPath=%cd%\packages

cd /d "%~dp0"
call "%cd%/windows/pack.bat"

cd /d "%~dp0"
call "%cd%/android/pack.bat"

cd /d "%~dp0"
xcopy /E /I /Y "%binaryPath%\*" "%targetPath%"

IF EXIST "%binaryPath%" (
    rd /s /q "%binaryPath%"
) 

pause 