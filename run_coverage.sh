#!/bin/bash

xbuild /v:minimal libgame.sln
if [ $? -ne 0 ] ; then exit 3; fi

NUNIT=`ls testrunner/NUnit.ConsoleRunner.*/tools/nunit3-console.exe`
echo '^Libgame\.[^U][\.A-Za-z0-9`]+$' > $NUNIT.covcfg

# From https://github.com/inorton/XR.Baboon/
covem $NUNIT libgame.UnitTests/bin/Debug/libgame.UnitTests.dll --process:Single > /dev/null
MATCHED=`sqlite3 testrunner/NUnit.ConsoleRunner.3.6.0/tools/nunit3-console.exe.covcfg.covdb "select count(hits) from lines;"`
COVERED=`sqlite3 testrunner/NUnit.ConsoleRunner.3.6.0/tools/nunit3-console.exe.covcfg.covdb "select count(hits) from lines where hits > 0;"`
echo "Result: $COVERED/$MATCHED ($((COVERED * 100 / MATCHED))%)"
cov-html $NUNIT.covcfg.covdb Libgame
rm -rf coverage-report
mv html coverage-report
