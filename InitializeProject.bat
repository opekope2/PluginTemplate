@echo off
dotnet tool restore
dotnet script InitializeProject.csx
pause
