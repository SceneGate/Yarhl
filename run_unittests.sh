#!/bin/bash

if [[ "$DOTNETCORE" -ne "1" ]]; then
    nuget install NUnit.Runners -OutputDirectory testrunner
    mono testrunner/NUnit.ConsoleRunner.*/tools/nunit3-console.exe yarhl.UnitTests/bin/Debug/net4*/yarhl.UnitTests.dll $@
else
    dotnet test yarhl.UnitTests/yarhl.UnitTests.csproj
fi
