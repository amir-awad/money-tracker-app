@echo off
dotnet build
dotnet ef migrations add "make migrations"
dotnet run