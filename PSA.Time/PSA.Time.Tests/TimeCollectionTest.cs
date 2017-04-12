using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSA.Time.Model;
using System;

namespace Common.Model.Tests
{
    [TestClass]
    public class TimeCollectionTest
    {
        /// <summary>
        /// Verify TimeCollection initial values
        /// </summary>
        [TestMethod]
        public void CreateTimeCollection()
        {
            DateTime date = new DateTime(2015, 7, 28);
            TimeCollection tc = new TimeCollection(date);

            Assert.IsNotNull(tc);
            Assert.AreEqual(date, tc.Date);
        }

        /// <summary>
        /// Verify the compare method of TimeCollection
        /// </summary>
        [TestMethod]
        public void TimeCollectionCompareTest()
        {
            DateTime date = new DateTime(2015, 7, 28);
            TimeCollection tc = new TimeCollection(date);

            DateTime date2 = new DateTime(2014, 1, 22);
            TimeCollection tc2 = new TimeCollection(date2);

            Assert.IsTrue(tc.CompareTo(date2) > 0);
            Assert.IsTrue(tc2.CompareTo(date) < 0);
            Assert.IsTrue(tc.CompareTo(date) == 0);
        }
    }
}
