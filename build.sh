#!/bin/bash

# Restore dependencies
nuget restore libgame.sln
nuget install NUnit.Runners -OutputDirectory testrunner

# Compile Mono.Addins with custom changes
msbuild mono-addins/Mono.Addins/Mono.Addins.csproj

# Compile the library
msbuild libgame.sln

