using System;

namespace StickyAnts.Util
{
    public class BitSet
    {
        private byte[] arr;
        public BitSet()
        {
            arr = new byte[0];
            Length = 0;
        }

        public BitSet(int noOfBits)
        {
            Length = noOfBits;
            var noOfBytes = (int)Math.Ceiling((double)noOfBits / 8);
            arr = new byte[noOfBytes];
        }

        public bool Get(int index)
        {
            var byteIndex = (int)Math.Floor((double)index / 8);
            var bitIndex = index % 8;
            return ((((arr[byteIndex]) >> bitIndex) & 0x1) > 0) ? true : false;
        }

        public BitSet Get(int startIndex, int endIndex)
        {
            var len = endIndex - startIndex;
            var bb = new BitSet(len);
            for (var i = 0; i < len; i++)
            {
                if (Get(startIndex + i))
                {
                    bb.Set(i + startIndex);
                }
            }
            return bb;
        }

        public void Clear(int index)
        {
            if (index >= Length)
            {
                expand(index + 1);
            }
            var byteIndex = (int)Math.Floor((double)index / 8);
            var bitIndex = index % 8;
            var val = ~(1 << bitIndex);
            arr[byteIndex] = (byte)(arr[byteIndex] & (byte)val);
        }

        public void Clear(int startIndex, int endIndex)
        {
            if (endIndex >= Length)
            {
                expand(endIndex);
            }
            for (var i = startIndex; i < endIndex; i++)
            {
                Clear(i);
            }
        }

        public void Clear()
        {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }
        }

        public void Flip(int index)
        {
            if (index >= Length)
            {
                expand(index + 1);
            }
            if (Get(index))
            {
                Clear(index);
            }
            else
            {
                Set(index);
            }
        }

        public void Flip(int startIndex, int endIndex)
        {
            if (endIndex >= Length)
            {
                expand(endIndex);
            }
            for (var i = startIndex; i < endIndex; i++)
            {
                Flip(i);
            }
        }

        public bool Equals(BitSet b)
        {
            if (b == null)
            {
                return false;
            }
            if (Length != b.Length)
            {
                return false;
            }
            for (var i = 0; i < arr.Length; i++)
            {
                if (b.arr[i] != arr[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int Size
        {
            get
            {
                return arr.Length;
            }
        }

        public void Xor(BitSet bitSet)
        {
            var len = bitSet.Length > Length ? Length : bitSet.Length;
            for (var i = 0; i < len; i++)
            {
                if (Get(i) == bitSet.Get(i))
                {
                    Clear(i);
                }
                else
                {
                    Set(i);
                }
            }
        }

        public void Or(BitSet bitSet)
        {
            var len = bitSet.Length > Length ? Length : bitSet.Length;
            for (var i = 0; i < len; i++)
            {
                if (Get(i) || bitSet.Get(i))
                {
                    Set(i);
                }
                else
                {
                    Clear(i);
                }
            }
        }

        public void And(BitSet bitSet)
        {
            var len = bitSet.Length > Length ? Length : bitSet.Length;
            for (var i = 0; i < len; i++)
            {
                if (Get(i) && bitSet.Get(i))
                {
                    Set(i);
                }
                else
                {
                    Clear(i);
                }
            }
        }

        public void AndNot(BitSet bitSet)
        {
            var len = bitSet.Length > Length ? Length : bitSet.Length;
            for (var i = 0; i < len; i++)
            {
                if (!Get(i) || !bitSet.Get(i))
                {
                    Set(i);
                }
                else
                {
                    Clear(i);
                }
            }
        }

        public bool Intersects(BitSet bitSet)
        {
            var len = bitSet.Length > Length ? Length : bitSet.Length;
            for (var i = 0; i < len; i++)
            {
                var v = Get(i);
                if (v == bitSet.Get(i) && v == true)
                {
                    return true;
                }
            }
            return false;
        }

        public void Fill(BitSet bitSet)
        {
            var len = bitSet.Length > Length ? Length : bitSet.Length;
            for (var i = 0; i < len; i++)
            {
                if (bitSet.Get(i))
                {
                    Set(i);
                }
                else
                {
                    Clear(i);
                }
            }
        }

        public BitSet Clone()
        {
            var bb = new BitSet(Length);
            bb.Fill(this);
            return bb;
        }

        public int Cardinality
        {
            get
            {
                var count = 0;
                for (var i = 0; i < Length; i++)
                {
                    if (Get(i))
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public bool IsEmpty()
        {
            return NextSetBit(0) == -1;
        }

        public void Set()
        {
            for (var i = 0; i < Length; i++)
            {
                Set(i);
            }
        }

        private void expand(int newLength)
        {
            Length = newLength;
            var oldArr = arr;
            var length = (int)Math.Ceiling((double)(newLength) / 8);
            if (length > arr.Length)
            {
                arr = new byte[length];
                for (var i = 0; i < oldArr.Length; i++)
                {
                    arr[i] = oldArr[i];
                }
            }
        }

        public void Set(int index)
        {
            if (index >= Length)
            {
                expand(index + 1);
            }
            var byteIndex = (int)Math.Floor((double)index / 8);
            var bitIndex = index % 8;
            var val = 1 << bitIndex;
            arr[byteIndex] = (byte)(arr[byteIndex] | ((byte)val));
        }

        public void Set(int index, bool v)
        {
            if (v)
            {
                Set(index);
            }
            else
            {
                Clear(index);
            }
        }

        public void Set(int startIndex, int endIndex)
        {
            if (endIndex >= Length)
            {
                expand(endIndex);
            }
            for (var i = startIndex; i < endIndex; i++)
            {
                Set(i);
            }
        }

        public void Set(int startIndex, int endIndex, bool v)
        {
            if (endIndex >= Length)
            {
                expand(endIndex);
            }

            for (var i = startIndex; i < endIndex; i++)
            {
                Set(i, v);
            }
        }

        private int nextBitThat(int startIndex, int val)
        {
            if (startIndex >= Length)
            {
                return -1;
            }
            var byteIndex = (int)Math.Floor((double)startIndex / 8);
            var bitIndex = startIndex % 8;
            var idx = startIndex;
            while (((arr[byteIndex] >> idx) & 0x1) == val && idx < Length)
            {
                idx++;
                byteIndex = (int)Math.Floor((double)idx / 8);
                bitIndex = idx % 8;
            }
            return idx == Length ? -1 : idx;
        }

        public int NextSetBit(int startIndex)
        {
            return nextBitThat(startIndex, 1);
        }

        public int NextClearBit(int startIndex)
        {
            return nextBitThat(startIndex, 0);
        }

        public int Length { get; private set; }

        public override string ToString()
        {
            var str = "";
            for (var i = 0; i < arr.Length; i++)
            {
                str = Convert.ToString(arr[i], 2) + str;
            }
            return $"{{{str} {Length}}}";
        }



    }

}
