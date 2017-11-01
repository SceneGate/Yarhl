#!/bin/bash

# Compile Mono.Addins with custom changes
msbuild /p:Configuration=Release mono-addins/Mono.Addins/Mono.Addins.csproj

if [[ "$DOTNETCORE" -ne "1" ]]; then
    nuget restore
    msbuild /p:TargetFrameworks=net47
else
    dotnet restore
    dotnet build -f netcoreapp2.0
fi
