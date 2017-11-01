#!/bin/bash

# Restore dependencies
nuget restore yarhl.sln
nuget install NUnit.Runners -OutputDirectory testrunner

# Compile Mono.Addins with custom changes
msbuild /p:Configuration=Release mono-addins/Mono.Addins/Mono.Addins.csproj

# Compile the library
msbuild /p:Configuration=Debug yarhl.sln
msbuild /p:Configuration=Release yarhl.sln
