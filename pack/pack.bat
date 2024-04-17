@echo off

cd /d "%~dp0"
set sourcePath=%cd%\..\src
set packagePath=%sourcePath%\ProgrammerTools\bin\packages
set targetPackagePath=%cd%\packages

cd /d "%~dp0"
call "%cd%/windows/pack.bat"

cd /d "%~dp0"
call "%cd%/android/pack.bat"

cd /d "%~dp0"
xcopy /E /I /Y "%packagePath%\*" "%targetPackagePath%"

IF EXIST "%packagePath%" (
    rd /s /q "%packagePath%"
) 

pause 