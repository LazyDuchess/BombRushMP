using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public class Bitfield
    {
        public bool this[Enum key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }
        private bool[] _values = [];

        public Bitfield(Enum size)
        {
            _values = new bool[Convert.ToInt32(size)];
        }

        public Bitfield(int size)
        {
            _values = new bool[size];
        }

        public static Bitfield ReadByte(BinaryReader reader)
        {
            var val = reader.ReadByte();
            var bitcount = sizeof(byte) * 8;
            var field = new Bitfield(bitcount);
            for (var i = 0; i < bitcount; i++)
            {
                var mask = (byte)(1 << i);
                field._values[i] = (val & mask) != 0;
            }
            return field;
        }

        public static Bitfield ReadShort(BinaryReader reader)
        {
            var val = reader.ReadInt16();
            var bitcount = sizeof(short) * 8;
            var field = new Bitfield(bitcount);
            for (var i = 0; i < bitcount; i++)
            {
                var mask = (short)(1 << i);
                field._values[i] = (val & mask) != 0;
            }
            return field;
        }

        public static Bitfield ReadInteger(BinaryReader reader)
        {
            var val = reader.ReadInt32();
            var bitcount = sizeof(int) * 8;
            var field = new Bitfield(bitcount);
            for (var i = 0; i < bitcount; i++)
            {
                var mask = (1 << i);
                field._values[i] = (val & mask) != 0;
            }
            return field;
        }

        public void WriteByte(BinaryWriter writer)
        {
            byte byteVal = 0;
            for (var i = 0; i < _values.Length; i++)
            {
                if (_values[i])
                    byteVal |= (byte)(1 << i);
            }
            writer.Write(byteVal);
        }

        public void WriteShort(BinaryWriter writer)
        {
            short shortVal = 0;
            for (var i = 0; i < _values.Length; i++)
            {
                if (_values[i])
                    shortVal |= (short)(1 << i);
            }
            writer.Write(shortVal);
        }

        public void WriteInteger(BinaryWriter writer)
        {
            var intVal = 0;
            for(var i=0;i<_values.Length;i++)
            {
                if (_values[i])
                    intVal |= (1 << i);
            }
            writer.Write(intVal);
        }

        public bool GetValue(Enum index)
        {
            return _values[Convert.ToInt32(index)];
        }

        public void SetValue(Enum index, bool value)
        {
            _values[Convert.ToInt32(index)] = value;
        }
    }
}
