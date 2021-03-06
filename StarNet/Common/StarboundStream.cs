using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StarNet.Common
{
    public class StarboundStream : Stream
    {
        public StarboundStream(Stream baseStream)
        {
            BaseStream = baseStream;
            StringEncoding = Encoding.UTF8;
        }

        public Encoding StringEncoding { get; set; }

        public Stream BaseStream { get; set; }

        public override bool CanRead { get { return BaseStream.CanRead; } }

        public override bool CanSeek { get { return BaseStream.CanSeek; } }

        public override bool CanWrite { get { return BaseStream.CanWrite; } }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override long Length
        {
            get { return BaseStream.Length; }
        }

        public override long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        public byte ReadUInt8()
        {
            int value = BaseStream.ReadByte();
            if (value == -1)
                throw new EndOfStreamException();
            return (byte)value;
        }

        public void WriteUInt8(byte value)
        {
            WriteByte(value);
        }

        public sbyte ReadInt8()
        {
            return (sbyte)ReadUInt8();
        }

        public void WriteInt8(sbyte value)
        {
            WriteUInt8((byte)value);
        }

        public ushort ReadUInt16()
        {
            return (ushort)(
                (ReadUInt8() << 8) |
                ReadUInt8());
        }

        public void WriteUInt16(ushort value)
        {
            Write(new[]
                  {
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF)
            }, 0, 2);
        }

        public short ReadInt16()
        {
            return (short)ReadUInt16();
        }

        public void WriteInt16(short value)
        {
            WriteUInt16((ushort)value);
        }

        public uint ReadUInt32()
        {
            return (uint)(
                (ReadUInt8() << 24) |
                (ReadUInt8() << 16) |
                (ReadUInt8() << 8 ) |
                ReadUInt8());
        }

        public void WriteUInt32(uint value)
        {
            Write(new[]
                  {
                (byte)((value & 0xFF000000) >> 24),
                (byte)((value & 0xFF0000) >> 16),
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF)
            }, 0, 4);
        }

        public int ReadInt32()
        {
            return (int)ReadUInt32();
        }

        public void WriteInt32(int value)
        {
            WriteUInt32((uint)value);
        }

        public ulong ReadUInt64()
        {
            return unchecked(
                ((ulong)ReadUInt8() << 56) |
                ((ulong)ReadUInt8() << 48) |
                ((ulong)ReadUInt8() << 40) |
                ((ulong)ReadUInt8() << 32) |
                ((ulong)ReadUInt8() << 24) |
                ((ulong)ReadUInt8() << 16) |
                ((ulong)ReadUInt8() << 8)  |
                (ulong)ReadUInt8());
        }

        public void WriteUInt64(ulong value)
        {
            Write(new[]
                  {
                (byte)((value & 0xFF00000000000000) >> 56),
                (byte)((value & 0xFF000000000000) >> 48),
                (byte)((value & 0xFF0000000000) >> 40),
                (byte)((value & 0xFF00000000) >> 32),
                (byte)((value & 0xFF000000) >> 24),
                (byte)((value & 0xFF0000) >> 16),
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF)
            }, 0, 8);
        }

        public long ReadInt64()
        {
            return (long)ReadUInt64();
        }

        public void WriteInt64(long value)
        {
            WriteUInt64((ulong)value);
        }

        public byte[] ReadUInt8Array()
        {
            int discarded;
            int length = (int)ReadVLQ(out discarded);
            return ReadUInt8Array(length);
        }

        public byte[] ReadUInt8Array(int length)
        {
            var result = new byte[length];
            if (length == 0) return result;
            int n = length;
            while (true) {
                n -= Read(result, length - n, n);
                if (n == 0)
                    break;
                System.Threading.Thread.Sleep(1);
            }
            return result;
        }

        public void WriteUInt8Array(byte[] value, bool includeLength = true)
        {
            if (includeLength)
                WriteVLQ((ulong)value.Length);
            Write(value, 0, value.Length);
        }

        public sbyte[] ReadInt8Array()
        {
            return (sbyte[])(Array)ReadUInt8Array();
        }

        public sbyte[] ReadInt8Array(int length)
        {
            return (sbyte[])(Array)ReadUInt8Array(length);
        }

        public void WriteInt8Array(sbyte[] value, bool includeLength)
        {
            if (includeLength)
                WriteVLQ((ulong)value.Length);
            Write((byte[])(Array)value, 0, value.Length);
        }

        public ushort[] ReadUInt16Array()
        {
            int discarded;
            int length = (int)ReadVLQ(out discarded);
            return ReadUInt16Array(length);
        }

        public ushort[] ReadUInt16Array(int length)
        {
            var result = new ushort[length];
            if (length == 0) return result;
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt16();
            return result;
        }

        public void WriteUInt16Array(ushort[] value, bool includeLength = true)
        {
            if (includeLength)
                WriteVLQ((ulong)value.Length);
            for (int i = 0; i < value.Length; i++)
                WriteUInt16(value[i]);
        }

        public short[] ReadInt16Array()
        {
            return (short[])(Array)ReadUInt16Array();
        }

        public short[] ReadInt16Array(int length)
        {
            return (short[])(Array)ReadUInt16Array(length);
        }

        public void WriteInt16Array(short[] value, bool includeLength = true)
        {
            WriteUInt16Array((ushort[])(Array)value, includeLength);
        }

        public uint[] ReadUInt32Array()
        {
            int discarded;
            int length = (int)ReadVLQ(out discarded);
            return ReadUInt32Array(length);
        }

        public uint[] ReadUInt32Array(int length)
        {
            var result = new uint[length];
            if (length == 0) return result;
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt32();
            return result;
        }

        public void WriteUInt32Array(uint[] value, bool includeLength = true)
        {
            if (includeLength)
                WriteVLQ((ulong)value.Length);
            for (int i = 0; i < value.Length; i++)
                WriteUInt32(value[i]);
        }

        public int[] ReadInt32Array()
        {
            return (int[])(Array)ReadUInt32Array();
        }

        public int[] ReadInt32Array(int length)
        {
            return (int[])(Array)ReadUInt32Array(length);
        }

        public void WriteInt32Array(int[] value, bool includeLength = true)
        {
            WriteUInt32Array((uint[])(Array)value, includeLength);
        }

        public ulong[] ReadUInt64Array()
        {
            int discarded;
            int length = (int)ReadVLQ(out discarded);
            return ReadUInt64Array(length);
        }

        public ulong[] ReadUInt64Array(int length)
        {
            var result = new ulong[length];
            if (length == 0) return result;
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt64();
            return result;
        }

        public void WriteUInt64Array(ulong[] value, bool includeLength = true)
        {
            if (includeLength)
                WriteVLQ((ulong)value.Length);
            for (int i = 0; i < value.Length; i++)
                WriteUInt64(value[i]);
        }

        public long[] ReadInt64Array()
        {
            return (long[])(Array)ReadUInt64Array();
        }

        public long[] ReadInt64Array(int length)
        {
            return (long[])(Array)ReadUInt64Array(length);
        }

        public void WriteInt64Array(long[] value, bool includeLength = true)
        {
            WriteUInt64Array((ulong[])(Array)value, includeLength);
        }

        public unsafe float ReadSingle()
        {
            uint value = ReadUInt32();
            return *(float*)&value;
        }

        public unsafe void WriteSingle(float value)
        {
            WriteUInt32(*(uint*)&value);
        }

        public unsafe double ReadDouble()
        {
            ulong value = ReadUInt64();
            return *(double*)&value;
        }

        public unsafe void WriteDouble(double value)
        {
            WriteUInt64(*(ulong*)&value);
        }

        public bool ReadBoolean()
        {
            return ReadUInt8() != 0;
        }

        public void WriteBoolean(bool value)
        {
            WriteUInt8(value ? (byte)1 : (byte)0);
        }

        public void WriteVLQ(ulong value)
        {
            var buffer = CreateVLQ(value);
            Write(buffer, 0, buffer.Length);
        }

        public void WriteSignedVLQ(long value)
        {
            var buffer = CreateSignedVLQ(value);
            Write(buffer, 0, buffer.Length);
        }

        public ulong ReadVLQ(out int length)
        {
            byte[] buffer = new byte[5];
            int i;
            for (i = 0; i < buffer.Length; i++)
            {
                buffer[i] = ReadUInt8();
                if ((buffer[i] & 0x80) == 0)
                    break;
            }
            if (i == buffer.Length)
                throw new IndexOutOfRangeException("VLQ exceeds maximum allowable length.");
            return ReadVLQ(buffer, 0, out length);
        }

        public long ReadSignedVLQ(out int length)
        {
            byte[] buffer = new byte[5];
            int i;
            for (i = 0; i < buffer.Length; i++)
            {
                buffer[i] = ReadUInt8();
                if ((buffer[i] & 0x80) == 0)
                    break;
            }
            if (i == buffer.Length)
                throw new IndexOutOfRangeException("VLQ exceeds maximum allowable length.");
            return ReadSignedVLQ(buffer, i + 1, out length);
        }

        public string ReadString()
        {
            return StringEncoding.GetString(ReadUInt8Array());
        }

        public void WriteString(string value)
        {
            WriteUInt16((ushort)value.Length);
            if (value.Length > 0)
                WriteUInt8Array(StringEncoding.GetBytes(value));
        }

        public Variant ReadVariant()
        {
            return Variant.FromStream(this);
        }

        public void WriteVariant(Variant variant)
        {
            variant.WriteTo(this);
        }

        public static long ReadSignedVLQ(byte[] buffer, int index, out int length)
        {
            var vlq = ReadVLQ(buffer, index, out length);
            bool negative = vlq % 2 == 1;
            if (negative)
                vlq += 1;
            vlq /= 2;
            if (negative)
                return -(long)vlq;
            return (long)vlq;
        }

        public static ulong ReadVLQ(byte[] buffer, int index, out int length)
        {
            length = 0;
            ulong value = 0;
            for (int i = 0; i + index < buffer.Length; i++)
            {
                length++;
                if ((buffer[i + index] & 0x80) == 0)
                {
                    value |= buffer[i + index];
                    return value;
                }
                value |= buffer[i + index] & 0x7FUL;
                value <<= 7;
            }
            throw new IndexOutOfRangeException("VLQ extends beyond end of array.");
        }

        public static byte[] CreateVLQ(ulong value)
        {
            int length = GetVLQLength(value);
            byte[] result = new byte[length];
            WriteVLQ(result, 0, value, out length);
            return result;
        }

        public static byte[] CreateSignedVLQ(long value)
        {
            int length = GetSignedVLQLength(value);
            byte[] result = new byte[length];
            WriteSignedVLQ(result, 0, value, out length);
            return result;
        }

        public static void WriteSignedVLQ(byte[] buffer, int index, long value, out int length)
        {
            ulong toWrite = (ulong)Math.Abs(value);
            toWrite *= 2;
            if (value < 0)
                toWrite -= 1;
            WriteVLQ(buffer, index, toWrite, out length);
        }

        public static void WriteVLQ(byte[] buffer, int index, ulong value, out int length)
        {
            value <<= 1;
            int actualLength = 56;
            while (actualLength > 0)
            {
                if ((value & 0xFE00000000000000ul) > 0)
                    break;
                value <<= 7;
                actualLength -= 7;
            }
            actualLength /= 7;
            length = 0;
            do
            {
                length++;
                byte upper7 = (byte)((value & 0xFE00000000000000ul) >> 57);
                if (index >= buffer.Length)
                    throw new IndexOutOfRangeException("VLQ extends beyond end of array.");
                if ((value & ~0xFE00000000000000ul) > 0 || actualLength > 0) // There's more to be written after this octet
                {
                    buffer[index++] = (byte)(upper7 | 0x80u);
                    actualLength--;
                }
                else
                    buffer[index++] = upper7;
                value <<= 7;
            } while (value > 0 || actualLength > 0);
        }

        public static int GetSignedVLQLength(long value)
        {
            ulong toTest = (ulong)Math.Abs(value);
            toTest *= 2;
            if (value < 0)
                toTest -= 1;
            return GetVLQLength(toTest);
        }

        public static int GetVLQLength(ulong value)
        {
            value <<= 1;
            int actualLength = 56;
            while (actualLength > 0)
            {
                if ((value & 0xFE00000000000000ul) > 0)
                    break;
                value <<= 7;
                actualLength -= 7;
            }
            actualLength /= 7;
            return actualLength + 1;
        }
    }
}

