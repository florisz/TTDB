using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Luminis.Patterns.Range.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class UnitTestDateRange
    {
        [Test]
        public void TestToString()
        {
            TimePointRange daterange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 1, 0, 0, 0));

            // this is with format
            Assert.IsTrue(daterange.ToString("dd-MM-yyyy") == "01-01-2000 - 01-02-2000");
        }

        [Test]
        public void TestIsEmpty()
        {
            TimePointRange daterange = new TimePointRange(new TimePoint(2000, 2, 1, 0, 0, 0), new TimePoint(2000, 1, 1, 0, 0, 0));
            Assert.IsTrue(daterange.IsEmpty());
        }

        [Test]
        public void TestIncludesTimePoints()
        {
            TimePointRange daterange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 1, 0, 0, 0));

            // include start?
            TimePoint TimePoint = new TimePoint(2000, 1, 1, 0, 0, 0);
            Assert.IsTrue(daterange.Includes(TimePoint));

            // include end?
            TimePoint = new TimePoint(2000, 2, 1, 0, 0, 0);
            Assert.IsTrue(daterange.Includes(TimePoint));

            // include a TimePoint in the middle
            TimePoint = new TimePoint(2000, 1, 15, 0, 0, 0);
            Assert.IsTrue(daterange.Includes(TimePoint));

            // not include TimePoints outside?
            TimePoint = new TimePoint(1999, 12, 12, 0, 0, 0);
            Assert.IsFalse(daterange.Includes(TimePoint));
            TimePoint = new TimePoint(2000, 2, 2, 0, 0, 0);
            Assert.IsFalse(daterange.Includes(TimePoint));
        }

        [Test]
        public void TestIncludesDateRanges()
        {
            TimePointRange daterange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 1, 0, 0, 0));

            // include same range?
            TimePointRange includerange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 1, 0, 0, 0));
            Assert.IsTrue(daterange.Includes(includerange));

            // include smaller range?
            includerange = new TimePointRange(new TimePoint(2000, 1, 2, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));
            Assert.IsTrue(daterange.Includes(includerange));

            // start is not part of the range
            includerange = new TimePointRange(new TimePoint(1999, 12, 31, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));
            Assert.IsFalse(daterange.Includes(includerange));

            // end is not part of the range
            includerange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 2, 0, 0, 0));
            Assert.IsFalse(daterange.Includes(includerange));

            // both start and end are not part of the range
            includerange = new TimePointRange(new TimePoint(1999, 12, 31, 0, 0, 0), new TimePoint(2000, 2, 2, 0, 0, 0));
            Assert.IsFalse(daterange.Includes(includerange));
        }

        [Test]
        public void TestEqualsDateRanges()
        {
            TimePointRange daterange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 1, 0, 0, 0));

            // exactly same range
            TimePointRange equalrange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 1, 0, 0, 0));
            Assert.IsTrue(daterange.Equals(equalrange));

            // start differs
            equalrange = new TimePointRange(new TimePoint(1999, 12, 31, 0, 0, 0), new TimePoint(2000, 2, 1, 0, 0, 0));
            Assert.IsFalse(daterange.Equals(equalrange));

            // end differs
            equalrange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 2, 0, 0, 0));
            Assert.IsFalse(daterange.Equals(equalrange));

            // both start and end differ
            equalrange = new TimePointRange(new TimePoint(1999, 12, 31, 0, 0, 0), new TimePoint(2000, 2, 2, 0, 0, 0));
            Assert.IsFalse(daterange.Equals(equalrange));
        }

        [Test]
        public void TestOverlapsDateRanges()
        {
            TimePointRange daterange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 1, 0, 0, 0));

            // same range overlaps?
            TimePointRange overlaprange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 1, 0, 0, 0));
            Assert.IsTrue(daterange.Overlaps(overlaprange));

            // end is later
            overlaprange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 3, 1, 0, 0, 0));
            Assert.IsTrue(daterange.Overlaps(overlaprange));

            // start is earlier
            overlaprange = new TimePointRange(new TimePoint(1999, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 15, 0, 0, 0));
            Assert.IsTrue(daterange.Overlaps(overlaprange));

            // end is earlier and start is later
            overlaprange = new TimePointRange(new TimePoint(1999, 1, 1, 0, 0, 0), new TimePoint(2000, 2, 15, 0, 0, 0));
            Assert.IsTrue(daterange.Overlaps(overlaprange));

            // range is completely before
            overlaprange = new TimePointRange(new TimePoint(1999, 1, 1, 0, 0, 0), new TimePoint(1999, 2, 15, 0, 0, 0));
            Assert.IsFalse(daterange.Overlaps(overlaprange));

            // range is completely after
            overlaprange = new TimePointRange(new TimePoint(2001, 1, 1, 0, 0, 0), new TimePoint(2001, 2, 15, 0, 0, 0));
            Assert.IsFalse(daterange.Overlaps(overlaprange));
        }

        [Test]
        public void TestGapDateRanges1()
        {
            // 1 januari t/m 31 januari
            TimePointRange daterange1 = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 31, 23, 59, 59));
            // 1 februari t/m 1 maart
            TimePointRange expectedrange = new TimePointRange(new TimePoint(2000, 2, 1, 0, 0, 0), new TimePoint(2000, 3, 1, 23, 59, 59));
            // 2 maart t/m 31 maart
            TimePointRange daterange2 = new TimePointRange(new TimePoint(2000, 3, 2, 0, 0, 0), new TimePoint(2000, 3, 31, 0, 0, 0));

            // same range overlaps?
            TimePointRange gaprange = daterange1.Gap(daterange2);
            Assert.IsTrue(gaprange.Equals(expectedrange));
        }

        [Test]
        public void TestGapDateRanges2()
        {
            // overlapping ranges should not have a gap
            // 1 januari t/m 31 januari
            TimePointRange daterange1 = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));
            // 15 januari t/m 31 maart
            TimePointRange daterange2 = new TimePointRange(new TimePoint(2000, 1, 15, 0, 0, 0), new TimePoint(2000, 3, 31, 0, 0, 0));

            // same range overlaps?
            TimePointRange gaprange = daterange1.Gap(daterange2);
            Assert.IsTrue(gaprange.Equals(TimePointRange.EMPTY));
        }

        [Test]
        public void TestGapDateRanges3()
        {
            // second range is before the first range (see TestGapDateRanges1)
            // 1 januari t/m 31 januari
            TimePointRange daterange1 = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 31, 23, 59, 59));
            // 1 februari t/m 1 maart
            TimePointRange expectedrange = new TimePointRange(new TimePoint(2000, 2, 1, 0, 0, 0), new TimePoint(2000, 3, 1, 23, 59, 59));
            // 2 maart t/m 31 maart
            TimePointRange daterange2 = new TimePointRange(new TimePoint(2000, 3, 2, 0, 0, 0), new TimePoint(2000, 3, 31, 0, 0, 0));

            // same range overlaps?
            TimePointRange gaprange = daterange2.Gap(daterange1);
            Assert.IsTrue(gaprange.Equals(expectedrange));
        }

        [Test]
        public void TestCompareToEqual()
        {
            // 1 januari t/m 31 januari
            TimePointRange daterange1 = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));
            // 1 januari t/m 31 januari
            TimePointRange daterange2 = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));

            Assert.IsTrue(daterange1.CompareTo(daterange2) == 0);
        }

        [Test]
        public void TestCompareToSmaller()
        {
            // 1 januari t/m 31 januari
            TimePointRange daterange1 = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));
            // 15 januari t/m 31 januari
            TimePointRange daterange2 = new TimePointRange(new TimePoint(2000, 1, 15, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));

            Assert.IsTrue(daterange1.CompareTo(daterange2) < 0);
        }

        [Test]
        public void TestCompareToGreater()
        {
            // 15 januari t/m 31 januari
            TimePointRange daterange1 = new TimePointRange(new TimePoint(2000, 1, 15, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));
            // 1 januari t/m 31 januari
            TimePointRange daterange2 = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));

            Assert.IsTrue(daterange1.CompareTo(daterange2) > 0);
        }

        [Test]
        public void TestConnectingTrue()
        {
            // 1 januari t/m 15 januari
            TimePointRange daterange1 = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 15, 23, 59, 59));
            // 16 januari t/m 31 januari
            TimePointRange daterange2 = new TimePointRange(new TimePoint(2000, 1, 16, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));

            Assert.IsTrue(daterange1.Connecting(daterange2));
        }

        [Test]
        public void TestConnectingFalse()
        {
            // 1 januari t/m 14 januari
            TimePointRange daterange1 = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 14, 0, 0, 0));
            // 16 januari t/m 31 januari
            TimePointRange daterange2 = new TimePointRange(new TimePoint(2000, 1, 16, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0));

            Assert.IsFalse(daterange1.Connecting(daterange2));
        }

        [Test]
        public void TestContiguousTrueSorted()
        {
            TimePointRange[] dataranges = 
                {
                    new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 15, 23, 59, 59)),
                    new TimePointRange(new TimePoint(2000, 1, 16, 0, 0, 0), new TimePoint(2000, 1, 31, 10, 10, 10)),
                    new TimePointRange(new TimePoint(2000, 1, 31, 10, 10, 11), new TimePoint(2000, 3, 11, 23, 59, 59)),
                    new TimePointRange(new TimePoint(2000, 3, 12, 0, 0, 0), new TimePoint(2000, 4, 15, 0, 0, 0))
                };

            Assert.IsTrue(TimePointRange.IsContiguous(dataranges));
        }

        [Test]
        public void TestContiguousTrueNonSorted()
        {
            TimePointRange[] dataranges = 
                {
                    new TimePointRange(new TimePoint(2000, 3, 12, 0, 0, 0), new TimePoint(2000, 4, 15, 0, 0, 0)),
                    new TimePointRange(new TimePoint(2000, 1, 31, 10, 10, 11), new TimePoint(2000, 3, 11, 23, 59, 59)),
                    new TimePointRange(new TimePoint(2000, 1, 16, 0, 0, 0), new TimePoint(2000, 1, 31, 10, 10, 10)),
                    new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 15, 23, 59, 59))
                };

            Assert.IsTrue(TimePointRange.IsContiguous(dataranges));
        }

        [Test]
        public void TestContiguousFalse()
        {
            TimePointRange[] dataranges = 
                {
                    new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 15, 0, 0, 0)),
                    new TimePointRange(new TimePoint(2000, 1, 17, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0)),
                    new TimePointRange(new TimePoint(2000, 2, 1, 0, 0, 0), new TimePoint(2000, 3, 11, 0, 0, 0)),
                    new TimePointRange(new TimePoint(2000, 3, 12, 0, 0, 0), new TimePoint(2000, 4, 15, 0, 0, 0))
                };

            Assert.IsFalse(TimePointRange.IsContiguous(dataranges));
        }

        [Test]
        public void TestCombination()
        {
            TimePointRange[] dataranges = 
                {
                    new TimePointRange(new TimePoint(2000, 3, 12, 0, 0, 0), new TimePoint(2000, 4, 15, 12, 23, 11)),
                    new TimePointRange(new TimePoint(2000, 2, 1, 0, 0, 0), new TimePoint(2000, 3, 11, 23, 59, 59)),
                    new TimePointRange(new TimePoint(2000, 1, 16, 0, 0, 0), new TimePoint(2000, 1, 31, 23, 59, 59)),
                    new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 15, 23, 59, 59))
                };
            TimePointRange datarange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 4, 15, 12, 23, 11));

            Assert.IsTrue(TimePointRange.Combination(dataranges).Equals(datarange));
        }

        [Test]
        [ExpectedException(typeof(System.ArgumentException))]
        public void TestCombinationException()
        {
            TimePointRange[] dataranges = 
                {
                    new TimePointRange(new TimePoint(2000, 3, 12, 0, 0, 0), new TimePoint(2000, 4, 15, 0, 0, 0)),
                    new TimePointRange(new TimePoint(2000, 2, 1, 0, 0, 0), new TimePoint(2000, 3, 11, 0, 0, 0)),
                    new TimePointRange(new TimePoint(2000, 1, 17, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0)),
                    new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 15, 0, 0, 0))
                };
            TimePointRange datarange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 4, 15, 0, 0, 0));

            TimePointRange.Combination(dataranges).Equals(datarange);
        }

        [Test]
        public void TestPartionedByTrue()
        {
            TimePointRange[] dataranges = 
                {
                    new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 15, 23, 59, 59)),
                    new TimePointRange(new TimePoint(2000, 1, 16, 0, 0, 0), new TimePoint(2000, 1, 31, 23, 59, 59)),
                    new TimePointRange(new TimePoint(2000, 2, 1, 0, 0, 0), new TimePoint(2000, 3, 11, 23, 59, 59)),
                    new TimePointRange(new TimePoint(2000, 3, 12, 0, 0, 0), new TimePoint(2000, 4, 15, 0, 0, 0))
                };

            TimePointRange datarange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 4, 15, 0, 0, 0));

            Assert.IsTrue(datarange.PartitionedBy(dataranges));
        }

        [Test]
        public void TestPartionedByFalse()
        {
            TimePointRange[] dataranges = 
                {
                    new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 1, 15, 0, 0, 0)),
                    new TimePointRange(new TimePoint(2000, 1, 17, 0, 0, 0), new TimePoint(2000, 1, 31, 0, 0, 0)),
                    new TimePointRange(new TimePoint(2000, 2, 1, 0, 0, 0), new TimePoint(2000, 3, 11, 0, 0, 0)),
                    new TimePointRange(new TimePoint(2000, 3, 12, 0, 0, 0), new TimePoint(2000, 4, 15, 0, 0, 0))
                };

            TimePointRange datarange = new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), new TimePoint(2000, 4, 15, 0, 0, 0));

            Assert.IsFalse(datarange.PartitionedBy(dataranges));
        }

        [Test]
        public void TestUpTo()
        {
            TimePointRange datarange = TimePointRange.UpTo(new TimePoint(2000, 1, 1, 0, 0, 0));

            Assert.IsTrue(datarange.Equals(new TimePointRange(TimePoint.Past, new TimePoint(2000, 1, 1, 0, 0, 0))));
        }

        [Test]
        public void TestStartingOn()
        {
            TimePointRange datarange = TimePointRange.StartingOn(new TimePoint(2000, 1, 1, 0, 0, 0));

            Assert.IsTrue(datarange.Equals(new TimePointRange(new TimePoint(2000, 1, 1, 0, 0, 0), TimePoint.Future)));
        }
    }
}
