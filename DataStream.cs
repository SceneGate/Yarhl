//-----------------------------------------------------------------------
// <copyright file="DataStream.cs" company="none">
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
using System.IO;

namespace Libgame
{
	public enum SeekMode
	{
		Origin,
		Current,
		End
	}

	public class DataStream : IDisposable
	{
		private static Dictionary<Stream, int> Instances = new Dictionary<Stream, int>();

		public DataStream(Stream stream, long offset, long length)
		{
			if (!Instances.ContainsKey(stream))
				Instances.Add(stream, 1);
			else
				Instances[stream] += 1;

			this.BaseStream = stream;
			this.Length = (length != -1) ? length : stream.Length;
			this.Offset = offset;
		}

		public DataStream(string filePath, FileMode mode, FileAccess access)
			: this(File.Open(filePath, mode, access), 0, -1)
		{
		}

		public long Offset {
			get;
			private set;
		}

		public long Position {
			get;
			private set;
		}

		public long Length {
			get;
			private set;
		}

		public Stream BaseStream {
			get;
			private set;
		}

		public bool EOF {
			get {
				if (this.Position >= this.Offset + this.Length)
					return true;
				else
					return false;
			}
		}

		public void Dispose()
		{
			Instances[this.BaseStream] -= 1;

			if (Instances[this.BaseStream] == 0) {
				this.BaseStream.Close();
			}
		}

		public void Seek(int shift, SeekMode mode)
		{
			if (mode == SeekMode.Current)
				this.Position += shift;
			else if (mode == SeekMode.Origin)
				this.Position = this.Offset + shift;
			else if (mode == SeekMode.End)
				this.Position = this.Offset + this.Length - shift;

			if (this.Position < this.Offset)
				this.Position = this.Offset;
			if (this.Position > this.Offset + this.Length)
				this.Position = this.Offset + this.Length;
		}

		public byte ReadByte()
		{
			if (this.Position >= this.Offset + this.Length)
				throw new EndOfStreamException();

			this.BaseStream.Position = this.Position++;
			return (byte)this.BaseStream.ReadByte();
		}

		public void WriteByte(byte val)
		{
			if (this.Position > this.Offset + this.Length)
				throw new EndOfStreamException();

			if (this.Position == this.Offset + this.Length)
				this.Length++;

			this.BaseStream.Position = this.Position++;
			this.BaseStream.WriteByte(val);
		}
	}
}
