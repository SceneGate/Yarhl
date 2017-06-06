#!/bin/bash

msbuild /v:minimal libgame.sln
if [ $? -ne 0 ] ; then exit 3; fi

StyleCop.Baboon libgame/Settings.StyleCop libgame/ libgame/bin libgame/obj
if [ $? -ne 0 ] ; then exit 1; fi

gendarme --ignore libgame/gendarme.ignore --html gendarme_report.html libgame/bin/Debug/libgame.dll
if [ $? -ne 0 ] ; then exit 2; fi

