#!/bin/bash
xbuild libgame.sln
mono testrunner/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe libgame.UnitTests/bin/Debug/libgame.UnitTests.dll $@