using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSA.Time.View.ValueConverter;

namespace PSA.Time.Tests.Converters
{
    [TestClass]
    public class TimeEntryToTitleConverterTest
    {
        [TestMethod]
        public void NullTimeEntryConvertsToEmptyString()
        {
            TimeEntryToTitleConverter converter = new TimeEntryToTitleConverter();

            Assert.AreEqual(string.Empty, converter.Convert(null, null, null, System.Globalization.CultureInfo.CurrentCulture));
        }
    }
}
