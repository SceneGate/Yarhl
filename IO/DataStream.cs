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

namespace Libgame.IO
{
	public enum SeekMode
	{
		Absolute,
		Origin,
		Current,
		End
	}

	public enum EndiannessMode 
	{
		LittleEndian,
		BigEndian
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
			this.Position = this.Offset;
		}

		public DataStream(string filePath, FileMode mode, FileAccess access)
			: this(File.Open(filePath, mode, access), 0, -1)
		{
		}

		public DataStream(DataStream stream, long offset, long length)
			: this(stream.BaseStream, offset, length)
		{
		}

		~DataStream()
		{
			this.Dispose(false);
		}

		public long Offset {
			get;
			private set;
		}

		public long Position {
			get;
			private set;
		}

		public long RelativePosition {
			get { return this.Position - this.Offset; }
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

		public static bool Compare(DataStream ds1, DataStream ds2)
		{
			if (ds1.Length != ds2.Length)
				return false;

			ds1.Seek(0, SeekMode.Origin);
			ds2.Seek(0, SeekMode.Origin);

			while (!ds1.EOF) {
				if (ds1.ReadByte() != ds2.ReadByte())
					return false;
			}

			return true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool freeManagedResourcesAlso)
		{
			Instances[this.BaseStream] -= 1;

			if (Instances[this.BaseStream] == 0) {
				this.BaseStream.Close();
			}
		}

		public void Flush()
		{
			this.BaseStream.Flush();
		}

		public void Seek(long shift, SeekMode mode)
		{
			if (mode == SeekMode.Current)
				this.Position += shift;
			else if (mode == SeekMode.Origin)
				this.Position = this.Offset + shift;
			else if (mode == SeekMode.End)
				this.Position = this.Offset + this.Length - shift;
			else if (mode == SeekMode.Absolute)
				this.Position = shift;

			if (this.Position < this.Offset)
				this.Position = this.Offset;
			if (this.Position > this.Offset + this.Length)
				this.Position = this.Offset + this.Length;
		}

		public void SetLength(long length)
		{
			if (length > this.BaseStream.Length)
				throw new ArgumentOutOfRangeException("length", length, "Length is bigger than BaseStream");
			if (length < 0)
				throw new ArgumentOutOfRangeException("length", length, "Length can not be negative");

			this.Length = length;

			if (this.RelativePosition > this.Length)
				this.Seek(0, SeekMode.End);
		}

		public byte ReadByte()
		{
			if (this.Position >= this.Offset + this.Length)
				throw new EndOfStreamException();

			this.BaseStream.Position = this.Position++;
			return (byte)this.BaseStream.ReadByte();
		}

		public int Read(byte[] buffer, int index, int count)
		{
			if (this.Position > this.Offset + this.Length + count)
				throw new EndOfStreamException();

			this.BaseStream.Position = this.Position;
			int read = this.BaseStream.Read(buffer, index, count);
			this.Position += count;

			return read;
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

		public void Write(byte[] buffer, int index, int count)
		{
			// If we're trying to write out the stream
			if (this.Position > this.Offset + this.Length)
				throw new EndOfStreamException();

			// If it's in the end the file, increment it
			if (this.Position == this.Offset + this.Length)
				this.Length += count;

			this.BaseStream.Position = this.Position;
			this.BaseStream.Write(buffer, index, count);
			this.Position += count;
		}

		public void WriteTimes(byte val, long times)
		{
			const int BufferSize = 5 * 1024;
			byte[] buffer = new byte[BufferSize];
			for (int i = 0; i < BufferSize; i++)
				buffer[i] = val;

			int written = 0;
			int toWrite = 0;
			do {
				if (written + BufferSize > times)
					toWrite = (int)(times - written);
				else
					toWrite = BufferSize;

				written += toWrite;
				this.Write(buffer, 0, toWrite);
				this.Flush();
			} while (written != times);
		}

		public void WriteUntilLength(byte val, long length)
		{
			long times = length - this.Length;
			this.Seek(0, SeekMode.End);
			this.WriteTimes(val, times);
		}

		public void WritePadding(byte val, int padding)
		{
			int times = (int)(padding - (this.Position % padding));
			if (times != padding)	// Else it's already padded
				this.WriteTimes(val, times);
		}

		public void WriteTo(string fileOut)
		{
			DataStream stream = new DataStream(fileOut, FileMode.Create, FileAccess.Write);
			this.WriteTo(stream);
			stream.Flush();
			stream.Dispose();
		}

		public void WriteTo(DataStream stream)
		{
			this.WriteTo(stream, this.Length);
		}

		public void WriteTo(DataStream stream, long count)
		{
			long currPos = this.Position;

			this.Seek(0, SeekMode.Origin);
			this.BaseStream.Position = this.Position;

			const int BufferSize = 5 * 1024;
			byte[] buffer = new byte[BufferSize];

			int written = 0;
			int toRead = 0;
			do {
				if (written + BufferSize > count)
					toRead = (int)(count - written);
				else
					toRead = BufferSize;

				written += this.Read(buffer, 0, toRead);
				stream.Write(buffer, 0, toRead);
				stream.Flush();
			} while (written != count);

			this.Seek(currPos, SeekMode.Absolute);
		}
	}
}
