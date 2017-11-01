#!/bin/bash

msbuild /v:minimal yarhl.sln
if [ $? -ne 0 ] ; then exit 3; fi

mono testrunner/NUnit.ConsoleRunner.*/tools/nunit3-console.exe yarhl.UnitTests/bin/Debug/yarhl.UnitTests.dll $@
