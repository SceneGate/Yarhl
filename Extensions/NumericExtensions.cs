//-----------------------------------------------------------------------
// <copyright file="GameFolder.cs" company="none">
// Copyright (C) 2013
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>20/09/2013</date>
//-----------------------------------------------------------------------
using System;

namespace Libgame
{
	public static class NumericExtensions
	{
		public static ushort Pad(this ushort num, int padding)
		{
			return (ushort)(padding - (num % padding));
		}

		public static uint Pad(this uint num, int padding)
		{
			return (uint)(padding - (num % padding));
		}

		public static ulong Pad(this ulong num, int padding)
		{
			return (ulong)padding - (num % (ulong)padding);
		}
	}
}

