#!/bin/bash
REPO_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/..
pushd "$REPO_DIR"

# Restore dependencies
nuget restore yarhl.sln
nuget install NUnit.Runners -OutputDirectory testrunner

# Compile Mono.Addins with custom changes
msbuild /v:minimal /p:Configuration=Release mono-addins/Mono.Addins/Mono.Addins.csproj
if [ $? -ne 0 ] ; then popd; exit 1; fi

# Compile the library
msbuild yarhl.sln

popd