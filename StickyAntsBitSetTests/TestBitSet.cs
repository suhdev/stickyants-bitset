using System;
using System.Diagnostics;
using Xunit;

namespace StickyAnts.Util.Tests
{
    public class TestBitSet
    {
        private static Random generator = new Random();
        private static bool failure = false;

        private static void fail(String diagnostic)
        {
            Trace.TraceError(diagnostic);
            failure = true;
        }

        private static void check(bool condition)
        {
            check(condition, "something's fishy");
        }

        private static void check(bool condition, String diagnostic)
        {
            Assert.True(condition);
        }

        private static void checkEmpty(BitSet s)
        {
            check(s.IsEmpty(), "IsEmpty");
            check(s.Length == 0, "Length");
            check(s.Cardinality == 0, "Cardinality");
            check(s.Equals(new BitSet()), "Equals");
            check(s.Equals(new BitSet(0)), "Equals");
            check(s.Equals(new BitSet(127)), "Equals");
            check(s.Equals(new BitSet(128)), "Equals");
            check(s.NextSetBit(0) == -1, "NextSetBit");
            check(s.NextSetBit(127) == -1, "NextSetBit");
            check(s.NextSetBit(128) == -1, "NextSetBit");
            check(s.NextClearBit(0) == 0, "NextClearBit");
            check(s.NextClearBit(127) == 127, "NextClearBit");
            check(s.NextClearBit(128) == 128, "NextClearBit");
            check(s.ToString().Equals("{}"), "ToString");
            check(!s.Get(0), "Get");
        }

        private static BitSet makeSet(params int[] elts)
        {
            BitSet s = new BitSet();
            foreach (int elt in elts)
                s.Set(elt);
            return s;
        }

        private static void checkEquality(BitSet s, BitSet t)
        {
            checkSanity(s, t);
            check(s.Equals(t), "equals");
            check(s.ToString().Equals(t.ToString()), "equal strings");
            check(s.Length == t.Length, "equal lengths");
            check(s.Cardinality == t.Cardinality, "equal cardinalities");
        }

        private static void checkSanity(params BitSet[] sets)
        {
            foreach (BitSet s in sets)
            {
                int len = s.Length;
                int cardinality1 = s.Cardinality;
                int cardinality2 = 0;
                for (int i = s.NextSetBit(0); i >= 0; i = s.NextSetBit(i + 1))
                {
                    check(s.Get(i));
                    cardinality2++;
                }
                check(s.NextSetBit(len) == -1, "last set bit");
                check(s.NextClearBit(len) == len, "last set bit");
                check(s.IsEmpty() == (len == 0), "emptiness");
                check(cardinality1 == cardinality2, "cardinalities");
                check(len <= s.Size, "length <= size");
                check(len >= 0, "length >= 0");
                check(cardinality1 >= 0, "cardinality >= 0");
            }
        }

        [Fact]
        public void TestInternals()
        {
            var bs = new BitSet(256);
            Assert.Equal(256 / 8,bs.Size);
            Assert.Equal(256,bs.Length);
        }


        [Fact]
        public void TestClear()
        {
            int failCount = 0;

            for (int i = 0; i < 1000; i++)
            {
                BitSet b1 = new BitSet();

                // Make a fairly random bitset
                int numberOfSetBits = generator.Next(100) + 1;
                int highestPossibleSetBit = generator.Next(1000) + 1;

                for (int x = 0; x < numberOfSetBits; x++)
                    b1.Set(generator.Next(highestPossibleSetBit));

                BitSet b2 = (BitSet)b1.Clone();

                // Clear out a random range
                int rangeStart = generator.Next(100);
                int rangeEnd = rangeStart + generator.Next(100);

                // Use the clear(int, int) call on b1
                b1.Clear(rangeStart, rangeEnd);

                // Use a loop on b2
                for (int x = rangeStart; x < rangeEnd; x++)
                    b2.Clear(x);

                // Verify their equality
                if (!b1.Equals(b2))
                {
                    failCount++;
                }
                Assert.Equal(b1.ToString(), b2.ToString());
                Assert.True(b1.Equals(b2));
            }
        }
    }
}
