#!/bin/bash

# Compile Mono.Addins with custom changes
msbuild /p:Configuration=Release mono-addins/Mono.Addins/Mono.Addins.csproj

if [[ "$DOTNETCORE" -ne "1" ]]; then
    # Only newer nuget versions support restoring .NET SDK solutions
    # so if possible we try with dotnet that will work for sure.
    command -v dotnet >/dev/null 2>&1 && dotnet restore || nuget restore
    msbuild /p:TargetFrameworks=net47
else
    dotnet restore
    dotnet build -f netcoreapp2.0
fi
