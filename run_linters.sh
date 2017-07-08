#!/bin/bash

StyleCop.Baboon Settings.StyleCop libgame/ libgame/bin libgame/obj
if [ $? -ne 0 ] ; then exit 1; fi

# The converters file contains test classes and there is no way to disable the warning
StyleCop.Baboon Settings.StyleCop libgame.UnitTests/ libgame.UnitTests/bin libgame.UnitTests/obj libgame.UnitTests/FileFormat/Converters.cs
if [ $? -ne 0 ] ; then exit 1; fi

msbuild /v:minimal libgame.sln
if [ $? -ne 0 ] ; then exit 3; fi

gendarme --ignore libgame/gendarme.ignore --html gendarme_report.html libgame/bin/Debug/libgame.dll
if [ $? -ne 0 ] ; then exit 2; fi

