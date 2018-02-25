#!/bin/bash
REPO_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/..
pushd "$REPO_DIR"

mono testrunner/NUnit.ConsoleRunner.*/tools/nunit3-console.exe Yarhl.UnitTests/bin/Debug/Yarhl.UnitTests.dll $@

popd