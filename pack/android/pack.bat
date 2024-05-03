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

dotnet publish "%projectPath%" --artifacts-path "%binaryPath%" -o "%binaryPath%" -f net8.0-android -c Release -p:ApplicationVersion="1" -p:ApplicationDisplayVersion="%version%" -p:ApplicationTitle="ProgrammerTools" -p:ApplicationId="com.yibei333.programmertools" -p:AndroidKeyStore=true -p:AndroidSigningKeyStore="%cd%\demo.keystore" -p:AndroidSigningKeyAlias=demo123 -p:AndroidSigningKeyPass=demo123 -p:AndroidSigningStorePass=demo123

copy "%binaryPath%\com.yibei333.programmertools-Signed.apk" "%packagePath%\ProgrammerTools.android.%version%.apk"

IF EXIST "%binaryPath%" (
    rd /s /q "%binaryPath%"
) 

::pause 