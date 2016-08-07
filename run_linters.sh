#!/bin/bash

StyleCop.Baboon libgame/Settings.StyleCop libgame/ libgame/bin libgame/obj
gendarme --html gendarme_report.html libgame/bin/Debug/libgame.dll
