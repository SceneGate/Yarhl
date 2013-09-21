//-----------------------------------------------------------------------
// <copyright file="DataReader.cs" company="none">
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
// <date>11/06/2013</date>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Libgame.IO
{
	public class DataReader
	{
		public DataReader(DataStream stream)
			: this(stream, EndiannessMode.LittleEndian, Encoding.UTF8)
		{
		}

		public DataReader(DataStream stream, EndiannessMode endiannes, Encoding encoding)
		{
			this.Stream    = stream;
			this.Endiannes = endiannes;
			this.Encoding  = encoding;
		}

		public DataStream Stream {
			get;
			private set;
		}

		public EndiannessMode Endiannes {
			get;
			private set;
		}

		public Encoding Encoding {
			get;
			private set;
		}

		public byte ReadByte()
		{
			return this.Stream.ReadByte();
		}

		public sbyte ReadSByte()
		{
			return (sbyte)this.Stream.ReadByte();
		}

		public ushort ReadUInt16()
		{
			if (this.Endiannes == EndiannessMode.LittleEndian)
				return (ushort)((this.ReadByte() << 0) | (this.ReadByte() << 8));
			else if (this.Endiannes == EndiannessMode.BigEndian)
				return (ushort)((this.ReadByte() << 8) | (this.ReadByte() << 0));

			return 0xFFFF;
		}

		public short ReadInt16()
		{
			return (short)this.ReadUInt16();
		}

		public uint ReadUInt32()
		{
			if (this.Endiannes == EndiannessMode.LittleEndian)
				return (uint)((this.ReadUInt16() << 00) | (this.ReadUInt16() << 16));
			else if (this.Endiannes == EndiannessMode.BigEndian)
				return (uint)((this.ReadUInt16() << 16) | (this.ReadUInt16() << 00));

			return 0xFFFFFFFF;
		}

		public int ReadInt32()
		{
			return (int)this.ReadUInt32();
		}

		public ulong ReadUInt64()
		{
			if (this.Endiannes == EndiannessMode.LittleEndian)
				return (ulong)((this.ReadUInt32() << 00) | (this.ReadUInt32() << 32));
			else if (this.Endiannes == EndiannessMode.BigEndian)
				return (ulong)((this.ReadUInt32() << 32) | (this.ReadUInt32() << 00));

			return 0xFFFFFFFFFFFFFFFF;
		}

		public long ReadInt64()
		{
			return (long)this.ReadUInt64();
		}

		public byte[] ReadBytes(int count)
		{
			byte[] buffer = new byte[count];
			this.Stream.Read(buffer, 0, count);
			return buffer;
		}

		public char ReadChar()
		{
			return this.ReadChars(1)[0];
		}

		public char[] ReadChars(int count)
		{
			long pos1 = this.Stream.Position;
			int charLength = this.Encoding.GetMaxByteCount(count);
			byte[] buffer = this.ReadBytes(charLength);

			char[] charArray = this.Encoding.GetChars(buffer);
			Array.Resize(ref charArray, count);	// In case we get more chars than asked

			// Adjust position
			charLength = this.Encoding.GetByteCount(charArray);
			this.Stream.Seek(pos1 + charLength, SeekMode.Absolute);

			return charArray;
		}

		/// <summary>
		/// Read until 0x00 byte reached
		/// </summary>
		/// <returns>The string.</returns>
		public string ReadString()
		{
			List<byte> list = new List<byte>();

			byte b = this.ReadByte();
			while (b != 0x00)
				list.Add(b);

			return this.Encoding.GetString(list.ToArray()).Replace("\0", "");
		}

		public string ReadString(int bytesCount)
		{
			byte[] buffer = this.ReadBytes(bytesCount);
			string s = this.Encoding.GetString(buffer);
			s = s.Replace("\0", "");
			return s;
		}
	}
}

