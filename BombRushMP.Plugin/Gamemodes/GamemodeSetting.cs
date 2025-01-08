using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Gamemodes
{
    public class GamemodeSetting
    {
        public string Label;
        public int Value;
        public int MinValue = 0;
        public int MaxValue = 0;
        public int AddSteps = 1;
        public Type EnumType = null;

        public GamemodeSetting(string label, int value, int min, int max, int addSteps = 1) 
        {
            Label = label;
            Value = value;
            MinValue = min;
            MaxValue = max;
            AddSteps = addSteps;
        }

        public GamemodeSetting(string label, Enum value, int addSteps = 1)
        {
            Label = label;
            Value = Convert.ToInt32(value);
            var vals = Enum.GetValues(value.GetType());
            var lowestMin = int.MaxValue;
            var highestMax = int.MinValue;
            foreach(var val in vals)
            {
                var intVal = Convert.ToInt32(val);
                if (intVal <= lowestMin)
                    lowestMin = intVal;
                if (intVal >= highestMax)
                    highestMax = intVal;
            }
            MinValue = lowestMin;
            MaxValue = highestMax;
            AddSteps = addSteps;
            EnumType = value.GetType();
        }

        public override string ToString()
        {
            if (EnumType != null)
            {
                return Enum.GetName(EnumType, Value).Replace('_', ' ');
            }
            return $"{Label} = {Value}";
        }

        public virtual void Next()
        {
            Value++;
            if (Value > MaxValue)
                Value = MinValue;
        }

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        public virtual void Read(BinaryReader reader)
        {
            Value = reader.ReadInt32();
        }
    }
}
