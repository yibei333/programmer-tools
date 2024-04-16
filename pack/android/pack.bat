@echo off
cd /d "%~dp0"

set projectPath=%cd%\..\..\src\ProgrammerTools
set packagePath=%projectPath%\bin\packages
set binaryPath=%packagePath%\android
set versionPath=%cd%\..\version.txt
set /p version=<"%versionPath%"

IF EXIST "%binaryPath%" (
    rd /s /q "%binaryPath%"
) 

cd "%cd%"
git pull

dotnet publish "%projectPath%" -o "%binaryPath%" -f net8.0-android -c Release -p:AndroidSigningKeyStore="%cd%\demo.keystore" -p:AndroidSigningKeyAlias=demo123 -p:AndroidSigningKeyPass=demo123 -p:AndroidSigningStorePass=demo123

copy %binaryPath%\com.yibei.programmertools-Signed.apk "%packagePath%\ProgrammerTools.android.%version%.apk"

IF EXIST "%binaryPath%" (
    rd /s /q "%binaryPath%"
) 

::pause 