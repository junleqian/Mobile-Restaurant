@echo off

setlocal 
%~d0
cd "%~dp0"

"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MsBuild" "%~dp0\..\..\..\Setup\acs\AcsSetup.sln" /p:Configuration=Debug
cd ..\..\..\Setup\acs\AcsSetup\bin\Debug\
cls

AcsSetup.exe %~dp0 "CRUDSqlAzure"

cd ..\..\..\..\Scripts\Samples

CALL CreateDatabaseCRUDSQLAzure.cmd "%~dp0"

:EXIT
PAUSE