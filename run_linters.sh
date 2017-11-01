#!/bin/bash

StyleCop.Baboon Settings.StyleCop yarhl/ yarhl/bin yarhl/obj
if [ $? -ne 0 ] ; then exit 1; fi

# The converters file contains test classes and there is no way to disable the warning
StyleCop.Baboon Settings.StyleCop yarhl.UnitTests/ yarhl.UnitTests/bin yarhl.UnitTests/obj yarhl.UnitTests/FileFormat/Converters.cs
if [ $? -ne 0 ] ; then exit 1; fi

if [[ "$DOTNETCORE" -ne "1" ]]; then
    gendarme --ignore yarhl/gendarme.ignore --html gendarme_report.html yarhl/bin/Debug/net4*/yarhl.dll
else
    gendarme --ignore yarhl/gendarme.ignore --html gendarme_report.html yarhl/bin/Debug/netcoreapp*/yarhl.dll
fi

if [ $? -ne 0 ] ; then exit 2; fi
