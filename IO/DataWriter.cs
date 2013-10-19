using System;
using System.Text;

namespace Libgame.IO
{
	public class DataWriter
	{
		public DataWriter(DataStream stream)
			: this(stream, EndiannessMode.LittleEndian, Encoding.UTF8)
		{
		}

		public DataWriter(DataStream stream, EndiannessMode endiannes, Encoding encoding)
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

		public void Write(byte val)
		{
			this.Stream.WriteByte(val);
		}

		public void Write(sbyte val)
		{
			this.Stream.WriteByte((byte)val);
		}

		public void Write(short val)
		{
			this.Write((ushort)val);
		}

		public void Write(ushort val)
		{
			if (this.Endiannes == EndiannessMode.LittleEndian) {
				this.Write((byte)((val >> 0) & 0xFF));
				this.Write((byte)((val >> 8) & 0xFF));
			} else if (this.Endiannes == EndiannessMode.BigEndian) {
				this.Write((byte)((val >> 8) & 0xFF));
				this.Write((byte)((val >> 0) & 0xFF));
			}
		}

		public void Write(int val)
		{
			this.Write((uint)val);
		}

		public void Write(uint val)
		{
			if (this.Endiannes == EndiannessMode.LittleEndian) {
				this.Write((ushort)((val >> 00) & 0xFFFF));
				this.Write((ushort)((val >> 16) & 0xFFFF));
			} else if (this.Endiannes == EndiannessMode.BigEndian) {
				this.Write((ushort)((val >> 16) & 0xFFFF));
				this.Write((ushort)((val >> 00) & 0xFFFF));
			}
		}

		public void Write(long val)
		{
			this.Write((ulong)val);
		}

		public void Write(ulong val)
		{
			if (this.Endiannes == EndiannessMode.LittleEndian) {
				this.Write((uint)((val >> 00) & 0xFFFFFFFF));
				this.Write((uint)((val >> 32) & 0xFFFFFFFF));
			} else if (this.Endiannes == EndiannessMode.BigEndian) {
				this.Write((uint)((val >> 32) & 0xFFFFFFFF));
				this.Write((uint)((val >> 00) & 0xFFFFFFFF));
			}
		}

		public void Write(byte[] vals)
		{
			this.Stream.Write(vals, 0, vals.Length);
		}

		public void Write(char ch)
		{
			this.Write(this.Encoding.GetBytes(new char[] { ch }));
		}

		public void Write(char[] chs)
		{
			this.Write(this.Encoding.GetBytes(chs));
		}

		public void Write(string s, int byteCount)
		{
			byte[] buffer = this.Encoding.GetBytes(s);
			if (buffer.Length >= byteCount) {
				buffer[byteCount - 1] = 0x00;	// Null terminator
				// TODO: Give warning instead of error
				//throw new ArgumentOutOfRangeException("s", s, "Text is so big");
			}

			Array.Resize(ref buffer, byteCount);
			this.Write(buffer);
		}

		public void Write(string s, int bytesCount, string tableName, bool originalToNew)
		{
			this.Write(s.ApplyTable(tableName, originalToNew), bytesCount);
		}

		public void Write(string s)
		{
			this.Write(this.Encoding.GetBytes(s));
		}

		public void Write(string s, string tableName, bool originalToNew)
		{
			this.Write(s.ApplyTable(tableName, originalToNew));
		}

		public void Write(string s, Type sizeType)
		{
			byte[] data = this.Encoding.GetBytes(s);
			this.Write(data.Length, sizeType);
			this.Write(data);
		}

		public void Write(string s, Type sizeType, string tableName, bool originalToNew)
		{
			this.Write(s.ApplyTable(tableName, originalToNew), sizeType);
		}

		public void Write(object o, Type type)
		{
			o = Convert.ChangeType(o, type);

			if (type == typeof(long))
				this.Write((long)o);
			else if (type == typeof(ulong))
				this.Write((ulong)o);
			else if (type == typeof(int))
				this.Write((int)o);
			else if (type == typeof(uint))
				this.Write((uint)o);
			else if (type == typeof(short))
				this.Write((short)o);
			else if (type == typeof(ushort))
				this.Write((ushort)o);
			else if (type == typeof(byte))
				this.Write((byte)o);
			else if (type == typeof(sbyte))
				this.Write((sbyte)o);
			else if (type == typeof(char))
				this.Write((char)o);
			else if (type == typeof(string))
				this.Write((string)o);
		}

		public void Flush()
		{
			this.Stream.Flush();
		}
	}
}

