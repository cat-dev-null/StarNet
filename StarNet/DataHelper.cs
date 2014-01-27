using System;

namespace StarNet
{
    public static class DataHelper
    {
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
            for (int i = 0; i < buffer.Length; i++)
            {
                length++;
                if ((buffer[i] & 0x80) == 0)
                {
                    value |= buffer[i];
                    return value;
                }
                value |= buffer[i] & 0x7FUL;
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
            if (value == 129)
                Console.Write("");
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

