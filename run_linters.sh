#!/bin/bash

StyleCop.Baboon Settings.StyleCop yarhl/ yarhl/bin yarhl/obj
if [ $? -ne 0 ] ; then exit 1; fi

StyleCop.Baboon Settings.StyleCop yarhl.Media/ yarhl.Media/bin yarhl.Media/obj
if [ $? -ne 0 ] ; then exit 1; fi

# The converters file contains test classes and there is no way to disable the warning
StyleCop.Baboon Settings.StyleCop yarhl.UnitTests/ yarhl.UnitTests/bin yarhl.UnitTests/obj yarhl.UnitTests/FileFormat/Converters.cs
if [ $? -ne 0 ] ; then exit 1; fi

msbuild /v:minimal yarhl.sln
if [ $? -ne 0 ] ; then exit 3; fi

gendarme --ignore yarhl/gendarme.ignore --html gendarme_report.html yarhl/bin/Debug/yarhl.dll
if [ $? -ne 0 ] ; then exit 2; fi

gendarme --ignore yarhl.Media/gendarme.ignore --html gendarme_report.html yarhl.Media/bin/Debug/yarhl.Media.dll
if [ $? -ne 0 ] ; then exit 2; fi

