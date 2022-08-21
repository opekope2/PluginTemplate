@echo off

REM Enable running C# scripts
dotnet tool restore

REM Restore projects
dotnet restore

pause
