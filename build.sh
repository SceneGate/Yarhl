#!/bin/bash

# Compile Mono.Addins with custom changes
msbuild /p:Configuration=Release mono-addins/Mono.Addins/Mono.Addins.csproj

# Restore dependencies
dotnet restore

if [[ "$DOTNETCORE" -ne "1" ]]; then
    msbuild
else
    dotnet build
fi
