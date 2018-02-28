#!/bin/bash
REPO_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/..
pushd "$REPO_DIR"

StyleCop.Baboon automation/Settings.StyleCop Yarhl/ Yarhl/bin Yarhl/obj
if [ $? -ne 0 ] ; then popd; exit 1; fi

StyleCop.Baboon automation/Settings.StyleCop Yarhl.Media/ Yarhl.Media/bin Yarhl.Media/obj
if [ $? -ne 0 ] ; then popd; exit 1; fi

# The converters file contains test classes and there is no way to disable the warning
StyleCop.Baboon automation/Settings.StyleCop Yarhl.UnitTests/ Yarhl.UnitTests/bin Yarhl.UnitTests/obj Yarhl.UnitTests/FileFormat/Converters.cs
if [ $? -ne 0 ] ; then popd; exit 1; fi

gendarme --ignore automation/gendarme_yarhl.ignore --html automation/gendarme_yarhl_report.html Yarhl/bin/Debug/Yarhl.dll
if [ $? -ne 0 ] ; then popd; exit 1; fi

gendarme --ignore automation/gendarme_yarhl.Media.ignore --html automation/gendarme_yarhl.Media_report.html Yarhl.Media/bin/Debug/Yarhl.Media.dll
if [ $? -ne 0 ] ; then popd; exit 1; fi

popd