#!/bin/bash

xbuild /v:minimal libgame.sln
if [ $? -ne 0 ] ; then exit 3; fi

NUNIT=`ls testrunner/NUnit.ConsoleRunner.*/tools/nunit3-console.exe`
echo "^Libgame" > $NUNIT.covcfg

# From https://github.com/inorton/XR.Baboon/
covem $NUNIT libgame.UnitTests/bin/Debug/libgame.UnitTests.dll --process:Single $@
cov-html $NUNIT.covcfg.covdb Libgame
rm -rf coverage-report
mv html coverage-report