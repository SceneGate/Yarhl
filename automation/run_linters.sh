#!/bin/bash
REPO_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/..
pushd "$REPO_DIR"

StyleCop.Baboon automation/Settings.StyleCop yarhl/ yarhl/bin yarhl/obj

StyleCop.Baboon automation/Settings.StyleCop yarhl.Media/ yarhl.Media/bin yarhl.Media/obj

# The converters file contains test classes and there is no way to disable the warning
StyleCop.Baboon automation/Settings.StyleCop yarhl.UnitTests/ yarhl.UnitTests/bin yarhl.UnitTests/obj yarhl.UnitTests/FileFormat/Converters.cs

gendarme --ignore automation/gendarme_yarhl.ignore --html automation/gendarme_yarhl_report.html yarhl/bin/Debug/yarhl.dll

gendarme --ignore automation/gendarme_yarhl.Media.ignore --html automation/gendarme_yarhl.Media_report.html yarhl.Media/bin/Debug/yarhl.Media.dll

popd