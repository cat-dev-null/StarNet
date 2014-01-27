using System;
using System.Collections.Generic;

namespace StarNet
{
    public class Variant
    {
        public object Value { get; private set; }

        private Variant()
        {
        }

        public Variant(object value)
        {
            if (!(value == null ||
                  value.GetType() == typeof(string) ||
                  value.GetType() == typeof(double) ||
                  value.GetType() == typeof(bool) ||
                  value.GetType() == typeof(ulong) ||
                  value.GetType() == typeof(uint) ||
                  value.GetType() == typeof(ushort) ||
                  value.GetType() == typeof(byte) ||
                  value.GetType() == typeof(Variant[]) ||
                  value.GetType() == typeof(Dictionary<string, Variant>)))
                throw new InvalidCastException(string.Format("Variants are unable to represent {0}.", value.GetType()));
            if (value.GetType() == typeof(uint) || value.GetType() == typeof(ushort) || value.GetType() == typeof(byte))
                Value = (ulong)value;
            Value = value;
        }

        public static Variant FromStream(StarboundStream stream)
        {
            var variant = new Variant();
            byte type = stream.ReadUInt8();
            int discarded;
            switch (type)
            {
                case 1:
                    variant.Value = null;
                    break;
                case 2:
                    variant.Value = stream.ReadDouble();
                    break;
                case 3:
                    variant.Value = stream.ReadBoolean();
                    break;
                case 4:
                    variant.Value = stream.ReadVLQ(out discarded);
                    break;
                case 5:
                    variant.Value = stream.ReadString();
                    break;
                case 6:
                    var array = new Variant[stream.ReadVLQ(out discarded)];
                    for (int i = 0; i < array.Length; i++)
                        array[i] = Variant.FromStream(stream);
                    variant.Value = array;
                    break;
                case 7:
                    var dict = new Dictionary<string, Variant>();
                    var length = stream.ReadVLQ(out discarded);
                    while (length-- > 0)
                        dict[stream.ReadString()] = Variant.FromStream(stream);
                    variant.Value = dict;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown Variant type: 0x{0:X2}", type));
            }
            return variant;
        }

        public void WriteTo(StarboundStream stream)
        {
            if (Value == null)
                stream.WriteUInt8(1);
            else if (Value.GetType() == typeof(double))
            {
                stream.WriteInt8(2);
                stream.WriteDouble((double)Value);
            }
            else if (Value.GetType() == typeof(bool))
            {
                stream.WriteInt8(3);
                stream.WriteBoolean((bool)Value);
            }
            else if (Value.GetType() == typeof(ulong))
            {
                stream.WriteInt8(4);
                stream.WriteDouble((ulong)Value);
            }
            else if (Value.GetType() == typeof(string))
            {
                stream.WriteInt8(5);
                stream.WriteString((string)Value);
            }
            else if (Value.GetType() == typeof(Variant[]))
            {
                stream.WriteInt8(6);
                var array = (Variant[])Value;
                stream.WriteVLQ((ulong)array.Length);
                for (int i = 0; i < array.Length; i++)
                    array[i].WriteTo(stream);
            }
            else if (Value.GetType() == typeof(Dictionary<string, Variant>))
            {
                stream.WriteInt8(7);
                var dict = (Dictionary<string, Variant>)Value;
                stream.WriteVLQ((ulong)dict.Count);
                foreach (var kvp in dict)
                {
                    stream.WriteString(kvp.Key);
                    kvp.Value.WriteTo(stream);
                }
            }
        }
    }
}