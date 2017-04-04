#!/bin/bash

xbuild /v:minimal libgame.sln
if [ $? -ne 0 ] ; then exit 3; fi

mono testrunner/NUnit.ConsoleRunner.*/tools/nunit3-console.exe libgame.UnitTests/bin/Debug/libgame.UnitTests.dll $@
