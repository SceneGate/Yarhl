//
//  AssemblyInfo.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System.Reflection;
using Mono.Addins;

[assembly: AssemblyTitle("yarhl.UnitTests")]
[assembly: AssemblyDescription("Unit tests for Yarhl library")]
[assembly: AssemblyCompany("SceneGate development team")]
[assembly: AssemblyProduct("SceneGate")]
[assembly: AssemblyCopyright("Copyright (c) 2017 Benito Palacios (aka pleonex)")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif

// Mono.Addins
[assembly: Addin("yarhl.UnitTests", "1.0")]
[assembly: AddinDependency("yarhl", "1.0")]
