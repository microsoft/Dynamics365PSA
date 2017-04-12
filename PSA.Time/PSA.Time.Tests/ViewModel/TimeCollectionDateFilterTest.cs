using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSA.Time.ViewModel;
using System;
using System.Globalization;

namespace PSA.Time.Tests.ViewModel
{
    [TestClass]
    public class TimeCollectionDateFilterTest
    {
        /// <summary>
        /// Verify the default start date, end date, period.
        /// </summary>
        [TestMethod]
        public void DefaultFilterIsCurrentWeek()
        {
            DateTime today = DateTime.Today;
            DateTime sundayThisWeek = today - new TimeSpan((int)today.DayOfWeek, 0, 0, 0);
            DateTime saturdayThisWeek = sundayThisWeek + TimeSpan.FromDays(6);

            TimeCollectionDateFilter filter = new TimeCollectionDateFilter();
            Assert.AreEqual(sundayThisWeek, filter.StartDate, "Incorrect start date.");
            Assert.AreEqual(saturdayThisWeek, filter.EndDate, "Incorrect end date.");
            Assert.AreEqual(TimeSpan.FromDays(7), filter.Period, "Incorrect period length.");
        }

        /// <summary>
        /// Verify how changing start date updates the end date.
        /// </summary>
        [TestMethod]
        public void ChangingStartChangesEndDate()
        {
            TimeCollectionDateFilter filter = new TimeCollectionDateFilter(new DateTime(2015, 1, 1), new DateTime(2015, 1, 2));
            filter.StartDate = new DateTime(2015, 1, 3);
            Assert.AreEqual(new DateTime(2015, 1, 4), filter.EndDate);
        }

        /// <summary>
        /// Verify that you cannot set the end date before the start date.
        /// </summary>
        [TestMethod]
        public void CannotSetEndDateLessThanStart()
        {
            bool exceptionCaught = false;

            try
            {
                TimeCollectionDateFilter filter = new TimeCollectionDateFilter();
                filter.StartDate = new DateTime(2015, 1, 3);
                filter.EndDate = new DateTime(2015, 1, 2);
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);
        }

        /// <summary>
        /// Verify that changing the end date updates the period length.
        /// </summary>
        [TestMethod]
        public void SettingEndDateChangesPeriod()
        {
            TimeCollectionDateFilter filter = new TimeCollectionDateFilter();
            filter.StartDate = new DateTime(2015, 1, 3);
            filter.EndDate = new DateTime(2015, 1, 4);

            Assert.AreEqual(TimeSpan.FromDays(2), filter.Period);
        }

        /// <summary>
        /// Verify functionality of calling Increment
        /// </summary>
        [TestMethod]
        public void IncrementPeriodTest()
        {
            TimeCollectionDateFilter filter = new TimeCollectionDateFilter();
            filter.StartDate = new DateTime(2015, 1, 10);
            filter.EndDate = new DateTime(2015, 1, 14);

            filter.Increment();

            Assert.AreEqual(new DateTime(2015, 1, 15), filter.StartDate, "Incorrect start date.");
            Assert.AreEqual(new DateTime(2015, 1, 19), filter.EndDate, "Incorrect end date.");
        }

        /// <summary>
        /// Verify functionality of calling Decrement
        /// </summary>
        [TestMethod]
        public void DecrementPeriodTest()
        {
            TimeCollectionDateFilter filter = new TimeCollectionDateFilter();
            filter.StartDate = new DateTime(2015, 1, 10);
            filter.EndDate = new DateTime(2015, 1, 14);

            filter.Decrement();

            Assert.AreEqual(new DateTime(2015, 1, 5), filter.StartDate, "Incorrect start date.");
            Assert.AreEqual(new DateTime(2015, 1, 9), filter.EndDate, "Incorrect end date.");
        }

        /// <summary>
        /// Check the format of the FilterText property.
        /// </summary>
        [TestMethod]
        public void FilterText()
        {
            TimeCollectionDateFilter filter = new TimeCollectionDateFilter();
            filter.StartDate = new DateTime(2015, 1, 10);
            filter.EndDate = new DateTime(2015, 1, 14);

            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Assert.AreEqual("1/10/2015-1/14/2015", filter.FilterText);
        }

        /// <summary>
        /// Bindings to the FilterText property need notification when the dates change.
        /// </summary>
        [TestMethod]
        public void IncrementNotifiesFilterText()
        {
            TimeCollectionDateFilter filter = new TimeCollectionDateFilter();
            filter.StartDate = new DateTime(2015, 1, 10);
            filter.EndDate = new DateTime(2015, 1, 14);

            // Subscribe
            bool notified = false;
            filter.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName ==  "FilterText")
                {
                    notified = true;
                }
            };

            filter.Increment();

            Assert.IsTrue(notified);
        }

        /// <summary>
        /// Bindings to the FilterText property need notification when the dates change.
        /// </summary>
        [TestMethod]
        public void DecrementtNotifiesFilterText()
        {
            TimeCollectionDateFilter filter = new TimeCollectionDateFilter();
            filter.StartDate = new DateTime(2015, 1, 10);
            filter.EndDate = new DateTime(2015, 1, 14);

            // Subscribe
            bool notified = false;
            filter.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "FilterText")
                {
                    notified = true;
                }
            };

            filter.Decrement();

            Assert.IsTrue(notified);
        }
    }
}
