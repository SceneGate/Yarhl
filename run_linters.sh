#!/bin/bash

xbuild libgame.sln
StyleCop.Baboon libgame/Settings.StyleCop libgame/ libgame/bin libgame/obj
gendarme --ignore libgame/gendarme.ignore --html gendarme_report.html libgame/bin/Debug/libgame.dll
