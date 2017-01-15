#!/bin/bash

xbuild /v:minimal libgame.sln
if [ $? -ne 0 ] ; then exit 3; fi

mono testrunner/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe libgame.UnitTests/bin/Debug/libgame.UnitTests.dll $@
