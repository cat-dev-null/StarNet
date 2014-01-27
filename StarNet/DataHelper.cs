using System;

namespace StarNet
{
    public static class DataHelper
    {
        public static long ReadSignedVLQ(byte[] buffer, int index, out int length)
        {
            var vlq = ReadVLQ(buffer, index, out length);
            vlq += 1;
            vlq /= 2;
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
    }
}

