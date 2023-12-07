using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HEXPLAY
{
	public class MemoryBuffer : IDisposable
	{
		MemoryStream stream;
		static byte[] bytes = new byte[1024];
		Stack<long> blockEndPos;

		public long GetSize() { return stream.Length; }
		public long GetPos() { return stream.Position; }
		public void SetPos(long pos) { stream.Position = pos; }
		public void SetSize(long size) { stream.SetLength(size); }
		public void Rest() { SetPos(0); SetSize(0); }

		public MemoryBuffer(int capacity)
		{
			stream = new MemoryStream(capacity);
			blockEndPos = new Stack<long>();
		}

		public MemoryBuffer(byte[] buffer)
		{
			stream = new MemoryStream(buffer);
			blockEndPos = new Stack<long>();
		}

		bool disposed;

		protected virtual void Dispose(bool disposing)
		{
			if(!disposed)
			{
				if(disposing)
				{
					//dispose managed resources
					stream.Dispose();
					stream = null;
				}
			}
			//dispose unmanaged resources
			disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public byte[] buffer { get { return stream.ToArray(); } }

		public void Set(byte[] array)
		{
			Rest();
			stream.Write(array, 0, array.Length);
			SetPos(0);
		}

		public void StartWriteBlock()
		{
			blockEndPos.Push(stream.Position);
			int blockSize = 0;
			Write(blockSize);
		}

		public void EndWriteBlock()
		{
			long blockStartPos = blockEndPos.Pop();
			long curPos = stream.Position;
			stream.Position = blockStartPos;
			Write((int)curPos);
			stream.Position = curPos;
		}

		public bool StartReadBlock()
		{
			int pos;

			if(!Read(out pos))
				return false;

			blockEndPos.Push(pos);

			return true;
		}

		public bool EndReadBlock()
		{
			if(blockEndPos.Count == 0)
				return false;

			long pos = blockEndPos.Pop();

			if(stream.Position > pos)
				return false;

			if(pos > stream.Length)
				return false;

			stream.Position = pos;

			return true;
		}

		public void Write(bool value)
		{
			bytes[0] = (byte)(value ? 1 : 0);
			stream.Write(bytes, 0, 1);
		}

		public bool Read(out bool value)
		{
			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + 1)
					goto skip;

			int total = stream.Read(bytes, 0, 1);

			if(total == 1)
			{
				value = bytes[0] == 1 ? true : false;
				return true;
			}
			skip:
			value = default(bool);
			return false;
		}

		public void Write(byte value)
		{
			bytes[0] = value;
			stream.Write(bytes, 0, 1);
		}

		public bool Read(out byte value)
		{
			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + 1)
					goto skip;

			int total = stream.Read(bytes, 0, 1);

			if(total == 1)
			{
				value = bytes[0];
				return true;
			}
			skip:
			value = default(byte);
			return false;
		}

		public void Write(sbyte value)
		{
			bytes[0] = (byte)value;
			stream.Write(bytes, 0, 1);
		}

		public bool Read(out sbyte value)
		{
			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + 1)
					goto skip;

			int total = stream.Read(bytes, 0, 1);

			if(total == 1)
			{
				value = (sbyte)bytes[0];
				return true;
			}
			skip:
			value = default(sbyte);
			return false;
		}

		public void Write(short value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		public bool Read(out short value)
		{
			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + sizeof(short))
					goto skip;

			int total = stream.Read(bytes, 0, sizeof(short));

			if(total == sizeof(short))
			{
				value = BitConverter.ToInt16(bytes, 0);
				return true;
			}
			skip:
			value = default(short);
			return false;
		}

		public void Write(ushort value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		public bool Read(out ushort value)
		{
			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + sizeof(ushort))
					goto skip;

			int total = stream.Read(bytes, 0, sizeof(ushort));

			if(total == sizeof(ushort))
			{
				value = BitConverter.ToUInt16(bytes, 0);
				return true;
			}
			skip:
			value = default(ushort);
			return false;
		}

		public void Write(int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		public bool Read(out int value)
		{
			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + sizeof(int))
					goto skip;

			int total = stream.Read(bytes, 0, sizeof(int));

			if(total == sizeof(int))
			{
				value = BitConverter.ToInt32(bytes, 0);
				return true;
			}
			skip:
			value = default(int);
			return false;
		}

		public void Write(HEXInt value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		public bool Read(out HEXInt value)
		{
			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + sizeof(int))
					goto skip;

			int total = stream.Read(bytes, 0, sizeof(int));

			if(total == sizeof(int))
			{
				value = BitConverter.ToInt32(bytes, 0);
				return true;
			}
			skip:
			value = default(int);
			return false;
		}

		public void Write(uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		public bool Read(out uint value)
		{
			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + sizeof(uint))
					goto skip;

			int total = stream.Read(bytes, 0, sizeof(uint));

			if(total == sizeof(uint))
			{
				value = BitConverter.ToUInt32(bytes, 0);
				return true;
			}
			skip:
			value = default(uint);
			return false;
		}

		public void Write(long value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		public bool Read(out long value)
		{
			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + sizeof(long))
					goto skip;

			int total = stream.Read(bytes, 0, sizeof(long));

			if(total == sizeof(long))
			{
				value = BitConverter.ToInt64(bytes, 0);
				return true;
			}
			skip:
			value = default(long);
			return false;
		}

		public void Write(DateTime value)
		{
			long utc = value.ToFileTimeUtc();
			Write(utc);
		}

		public bool Read(out DateTime value)
		{
			long utc;

			if(Read(out utc))
			{
				value = DateTime.FromFileTimeUtc(utc);
				return true;
			}

			value = default(DateTime);
			return false;
		}

		public void Write(ulong value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}

		public bool Read(out ulong value)
		{
			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + sizeof(ulong))
					goto skip;

			int total = stream.Read(bytes, 0, sizeof(ulong));

			if(total == sizeof(ulong))
			{
				value = BitConverter.ToUInt64(bytes, 0);
				return true;
			}
			skip:
			value = default(ulong);
			return false;
		}

		public void Write(string value)
		{
			if(value == null || value.Length == 0)
			{
				Write(0);
				return;
			}

			byte[] bytes = Encoding.UTF8.GetBytes(value);
			Write(bytes.Length);
			stream.Write(bytes, 0, bytes.Length);
		}

		public bool Read(out string value)
		{
			int stringSize;

			if(!Read(out stringSize))
			{
				value = default(string);
				return false;
			}

			if(stringSize == 0)
			{
				value = "";
				return true;
			}

			if(stringSize < 0 || stringSize > stream.Length - stream.Position)
			{
				value = default(string);
				return false;
			}

			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + stringSize)
					goto skip;

			if(bytes.Length < stringSize)
				bytes = new byte[stringSize];

			int total = stream.Read(bytes, 0, stringSize);

			if(total == stringSize)
			{
				value = Encoding.UTF8.GetString(bytes, 0, stringSize);
				return true;
			}
			skip:
			value = default(string);
			return false;
		}

		public void Write(byte[] array)
		{
			Write(array.Length);
			stream.Write(array, 0, array.Length);
		}

		public bool Read(out byte[] array)
		{
			int arraySize;

			if(!Read(out arraySize))
			{
				array = default(byte[]);
				return false;
			}

			if(arraySize < 0 || arraySize > stream.Length - stream.Position)
			{
				array = default(byte[]);
				return false;
			}

			if(blockEndPos.Count != 0)
				if(blockEndPos.Peek() < stream.Position + arraySize)
					goto skip;

			array = new byte[arraySize];

			int total = stream.Read(array, 0, arraySize);

			if(total == arraySize)
				return true;

			skip:
			array = default(byte[]);
			return false;
		}

		public void Write(Fixed value)
		{
			Write(value.raw);
		}

		public bool Read(out Fixed value)
		{
			long raw;

			if(Read(out raw))
			{
				value = Fixed.F(raw);
				return true;
			}

			value = default(Fixed);
			return false;
		}

		public void Write(Vector2 value)
		{
			Write(value.x);
			Write(value.y);
		}

		public bool Read(out Vector2 value)
		{
			if(Read(out value.x) && Read(out value.y))
				return true;

			value = default(Vector2);
			return false;
		}

		public void Write(AABB2D aabb)
		{
			Write(aabb.t);
			Write(aabb.b);
			Write(aabb.l);
			Write(aabb.r);
		}

		public bool Read(out AABB2D aabb)
		{
			if(Read(out aabb.t) && Read(out aabb.b) && Read(out aabb.l) && Read(out aabb.r))
				return true;

			aabb = AABB2D.Rest;
			return false;
		}

		public void Write<T>(T[] t, Action<MemoryBuffer, T> write)
		{
			StartWriteBlock();

			if(t != null)
			{
				Write(t.Length);

				for(int i = 0; i < t.Length; i++)
					write(this, t[i]);
			}
			else
				Write(0);

			EndWriteBlock();
		}

		public void Write<T>(List<T> t, Action<MemoryBuffer, T> write)
		{
			StartWriteBlock();

			if(t != null)
			{
				Write(t.Count);

				int ic = t.Count;
				for(int i = 0; i < ic; i++)
					write(this, t[i]);
			}
			else
				Write(0);

			EndWriteBlock();
		}

		public bool Read<T>(out T[] t, Func<MemoryBuffer, T> read)
		{
			StartReadBlock();

			t = default(T[]);

			while(true)
			{
				int len;
				if(!Read(out len))
					break;

				t = new T[len];

				for(int i = 0; i < len; i++)
				{
					t[i] = read(this);
					if(t[i] == null)
						break;
				}

				break;
			}

			return EndReadBlock();
		}

		public bool Read<T>(List<T> t, Func<MemoryBuffer, T> read)
		{
			StartReadBlock();

			t.Clear();

			while(true)
			{
				int len;
				if(!Read(out len))
					break;

				for(int i = 0; i < len; i++)
				{
					T e = read(this);
					if(e == null)
						break;
					t.Add(e);
				}

				break;
			}

			return EndReadBlock();
		}

		public delegate bool ReadData<T>(MemoryBuffer mb, out T t);

		public bool Read<T>(out T[] t, ReadData<T> read)
		{
			StartReadBlock();

			t = default(T[]);

			while(true)
			{
				int len;
				if(!Read(out len))
					break;

				t = new T[len];

				for(int i = 0; i < len; i++)
					if(!read(this, out t[i]))
						break;

				break;
			}

			return EndReadBlock();
		}
	}
}