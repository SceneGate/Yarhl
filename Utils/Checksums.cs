//-----------------------------------------------------------------------
// <copyright file="Checksums.cs" company="none">
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
// <date>11/10/2013</date>
//-----------------------------------------------------------------------
using System;
using Libgame.IO;

namespace Libgame.Utils
{
	public static class Checksums
	{
		/// <summary>
		/// Calculates CRC16 from the stream.
		/// Code from "blz.c" (NDS Compressors) by CUE
		/// </summary>
		/// <param name="data">Data to calculate the checksum</param>
		public static ushort Crc16(DataStream data, uint length)
		{
			ushort crc;
			uint   nbits;
		
			crc = 0xFFFF;
			while (length-- != 0) {
				crc ^= data.ReadByte();
				nbits = 8;
				while (nbits-- != 0) {
					if ((crc & 1) != 0) { crc = (ushort)((crc >> 1) ^ 0xA001); }
					else           	      crc = (ushort)(crc >> 1);
				}
			}

			return crc;
		}
	}
}

