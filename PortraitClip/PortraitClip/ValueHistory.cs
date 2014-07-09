using System;
using System.Collections.Generic;
using System.Linq;

namespace PortraitClip
{
    public class ValueShortCache<T>
    {
        public T Previous { get; private set; }
        public T Current { get; private set; }

        public ValueShortCache(T defaultValue)
        {
            Previous = defaultValue;
            Current = defaultValue;
        }

        public void UpdateValue(T value)
        {
            Previous = Current;
            Current = value;
        }
    }

    public class ValueHistory<T>
    {
        public int MaxLength { get; private set; }
        public Queue<T> History { get; private set; }
        public bool IsFull { get { return History.Count == MaxLength; } }

        public ValueHistory(int maxLength)
        {
            if (maxLength <= 0) throw new ArgumentOutOfRangeException("maxLength", maxLength, "The value must be larger than 0.");

            MaxLength = maxLength;
            History = new Queue<T>(maxLength);
        }

        public void UpdateValue(T value)
        {
            if (IsFull)
            {
                History.Dequeue();
            }
            History.Enqueue(value);
        }
    }
}
