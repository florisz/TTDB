using System;
using NUnit.Framework;

namespace TimeTraveller.General.Patterns.Range.Test
{
    /// <summary>
    /// Summary description for UnitTestTimePoint
    /// </summary>
    [TestFixture]
    public class UnitTestTimePoint
    {
        [Test]
        public void TestConstructor()
        {
            TimePoint TimePoint = new TimePoint(2000, 1, 1, 0, 0, 0);

            // object equality test
            Assert.IsTrue(TimePoint.Equals(TimePoint));

            // object instance comparison
            Assert.IsTrue(TimePoint.Equals(new TimePoint(2000, 1, 1, 0, 0, 0)));
            Assert.IsFalse(TimePoint.Equals(new TimePoint(2000, 1, 2, 0, 0, 0)));
            Assert.IsTrue(TimePoint.Equals(new TimePoint(new DateTime(2000, 1, 1, 0, 0, 0))));
        }
        [Test]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void TestInvalidDates()
        {
            // an invalid date should generate an exception
            TimePoint TimePoint = new TimePoint(2000, 2, 30, 0, 0, 0);
        }

        [Test]
        public void TestBefore()
        {
            TimePoint TimePoint1 = new TimePoint(2000, 1, 1, 0, 0, 0);
            TimePoint TimePoint2 = new TimePoint(2000, 1, 2, 0, 0, 0);

            Assert.IsTrue(TimePoint1.Before(TimePoint2));
        }

        [Test]
        public void TestAfter()
        {
            TimePoint TimePoint1 = new TimePoint(2000, 1, 1, 0, 0, 0);
            TimePoint TimePoint2 = new TimePoint(2000, 1, 2, 0, 0, 0);

            Assert.IsTrue(TimePoint2.After(TimePoint1));
        }

        [Test]
        public void TestCompareToEqual()
        {
            TimePoint TimePoint1 = new TimePoint(2000, 1, 1, 0, 0, 0);
            TimePoint TimePoint2 = new TimePoint(2000, 1, 2, 0, 0, 0);

            Assert.IsTrue(TimePoint1.CompareTo(TimePoint1) == 0);
        }

        [Test]
        public void TestCompareToSmaller()
        {
            TimePoint TimePoint1 = new TimePoint(2000, 1, 1, 0, 0, 0);
            TimePoint TimePoint2 = new TimePoint(2000, 1, 2, 0, 0, 0);

            Assert.IsTrue(TimePoint1.CompareTo(TimePoint2) < 0);
        }

        [Test]
        public void TestCompareToGreater()
        {
            TimePoint TimePoint1 = new TimePoint(2000, 1, 1, 0, 0, 0);
            TimePoint TimePoint2 = new TimePoint(2000, 1, 2, 0, 0, 0);

            Assert.IsTrue(TimePoint2.CompareTo(TimePoint1) > 0);
        }

        [Test]
        public void TestAddSeconds()
        {
            TimePoint TimePoint1 = new TimePoint(2000, 1, 1, 0, 0, 0);
            TimePoint TimePoint2 = new TimePoint(2000, 1, 1, 0, 0, 1);

            TimePoint TimePoint3 = TimePoint1.AddSeconds(1);

            Assert.IsTrue(TimePoint3.After(TimePoint1));
            Assert.IsTrue(TimePoint3.Equals(TimePoint2));
        }

        [Test]
        public void TestMinusSeconds()
        {
            TimePoint TimePoint1 = new TimePoint(2000, 1, 2, 0, 0, 0);
            TimePoint TimePoint2 = new TimePoint(2000, 1, 1, 23, 59, 59);

            TimePoint TimePoint3 = TimePoint1.MinusSeconds(1);

            Assert.IsTrue(TimePoint3.Before(TimePoint1));
            Assert.IsTrue(TimePoint3.Equals(TimePoint2));
        }

        [Test]
        public void TestStartYearAddSeconds1()
        {
            // accros a year boundary with MinusSeconds
            TimePoint TimePoint1 = new TimePoint(2000, 1, 1, 0, 0, 0);
            TimePoint TimePoint2 = new TimePoint(1999, 12, 31, 23, 59, 59);

            TimePoint TimePoint3 = TimePoint1.MinusSeconds(1);

            Assert.IsTrue(TimePoint3.Before(TimePoint1));
            Assert.IsTrue(TimePoint3.Equals(TimePoint2));
        }

        [Test]
        public void TestStartYearAddSeconds2()
        {
            // accros a year boundary with AddSeconds(-1)
            TimePoint TimePoint1 = new TimePoint(2000, 1, 1, 0, 0, 0);
            TimePoint TimePoint2 = new TimePoint(1999, 12, 31, 23, 59, 59);

            TimePoint TimePoint3 = TimePoint1.AddSeconds(-1);

            Assert.IsTrue(TimePoint3.Before(TimePoint1));
            Assert.IsTrue(TimePoint3.Equals(TimePoint2));
        }
    }
}
