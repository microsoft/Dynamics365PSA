using Common.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Samples;
using PSA.Time.ViewModel;
using System;

namespace PSA.Time.Tests.ViewModel
{
    [TestClass]
    public class TimeViewModelTest
    {        
        /// <summary>
        /// Validate that SetDefaultValues put the right values in the msdyn_timeentry.
        /// </summary>
        [TestMethod]
        public void SetDefaultValuesTest()
        {
            msdyn_timeentry timeEntry = new msdyn_timeentry();
            TimeViewModel entry = new TimeViewModel(timeEntry);
            DateTime todayUTC = DateTime.Today;
            entry.SetDefaultValues();

            // validate entry data
            Assert.AreEqual(new DateTime(todayUTC.Year, todayUTC.Month, todayUTC.Day, 12, 0, 0), timeEntry.msdyn_date, "Incorrect default date of noon today.");
            Assert.AreEqual(60, timeEntry.msdyn_duration, "Incorrect default duration");
            Assert.AreEqual((int)msdyn_timeentrytype.Work, timeEntry.msdyn_type.Value, "Incorrect default time entry type");
            Assert.AreEqual((int)msdyn_timeentry_msdyn_entrystatus.Draft, timeEntry.msdyn_entryStatus.Value, "Incorrect default time entry status");
        }

        /// <summary>
        /// Validate that calling SetDefaultValues only updates a new time entry.
        /// </summary>
        [TestMethod]
        public void SetDefaultValuesOnlyWorksOnNewEntries()
        {
            msdyn_timeentry timeEntry = new msdyn_timeentry();
            TimeViewModel entry = new TimeViewModel(timeEntry);

            timeEntry.Id = Guid.NewGuid();
            entry.SetDefaultValues();

            Assert.IsNull(timeEntry.msdyn_date);
            Assert.IsNull(timeEntry.msdyn_duration);
            Assert.IsNull(timeEntry.msdyn_type);
            Assert.IsNull(timeEntry.msdyn_entryStatus);
        }

        /// <summary>
        /// Validates that the method returns true for all except submitted and approved time entries.
        /// </summary>
        [TestMethod]
        public void CanEditTimeEntryTest()
        {
            msdyn_timeentry timeEntry = new msdyn_timeentry();
            TimeViewModel entry = new TimeViewModel(timeEntry);

            Assert.IsTrue(entry.CanEdit(), "A time entry that hasn't been saved (has null values) should be editable.");

            timeEntry.Id = Guid.NewGuid();
            entry.SetDefaultValues();
            Assert.IsTrue(entry.CanEdit(), "A time entry that has been saved with values should be editable.");

            timeEntry.msdyn_entryStatus = new OptionSetValue((int)msdyn_timeentry_msdyn_entrystatus.Submitted);
            Assert.IsFalse(entry.CanEdit(), "A time entry that has been submitted with values should be editable.");
        }

        /// <summary>
        /// Validates that the method returns true for only submitted time entries.
        /// </summary>
        [TestMethod]
        public void IsSubmittedTimeEntryTest()
        {
            msdyn_timeentry timeEntry = new msdyn_timeentry();
            TimeViewModel entry = new TimeViewModel(timeEntry);

            Assert.IsFalse(entry.IsSubmitted(), "A time entry that hasn't been saved (has null values) should return false.");

            timeEntry.Id = Guid.NewGuid();
            entry.SetDefaultValues();
            Assert.IsFalse(entry.IsSubmitted(), "A time entry that has been saved with values should return false.");

            timeEntry.msdyn_entryStatus = new OptionSetValue((int)msdyn_timeentry_msdyn_entrystatus.Submitted);
            Assert.IsTrue(entry.IsSubmitted(), "A time entry that has been submitted should return true.");

            timeEntry.msdyn_entryStatus = new OptionSetValue((int)msdyn_timeentry_msdyn_entrystatus.Approved);
            Assert.IsFalse(entry.IsSubmitted(), "A time entry that has been saved with values should return false.");
        }

        /// <summary>
        /// Validate duration test when saving time entries.
        /// </summary>
        [TestMethod]
        public void TimeEntryDurationValidationTest()
        {
            // Prepare the time entries for test purposes. 
            DateTime dateTime = DateTime.Today;
                       
            msdyn_timeentry timeEntry1 = new msdyn_timeentry();
            timeEntry1.msdyn_timeentryId = new Guid();
            timeEntry1.msdyn_description = "entry 1";
            timeEntry1.msdyn_date = dateTime;
            timeEntry1.msdyn_duration = 20 * 60;    // 20 hours.

            msdyn_timeentry timeEntry2 = new msdyn_timeentry();
            timeEntry2.msdyn_timeentryId = new Guid();
            timeEntry2.msdyn_description = "entry 2";
            timeEntry2.msdyn_date = dateTime;
            timeEntry2.msdyn_duration = 4 * 60 + 1; // 4 hours and 1 minute.

            TimeViewModel entry = new TimeViewModel(timeEntry1);
            TimeCollectionViewModel viewModel = new TimeCollectionViewModel();
            viewModel.AddTimeEntries(new msdyn_timeentry[] { timeEntry1, timeEntry2 });
            
            Assert.IsFalse(entry.CanTimeBeSaved(timeEntry1), "The time entry should not be able to save since the total duration is 24:01");

            timeEntry1.msdyn_duration = 19 * 60 + 59;    // 19 hours and 59 minutes.
            Assert.IsTrue(entry.CanTimeBeSaved(timeEntry1), "The time entry should be able to save since the total duration is 24:00");
        }
    }
}
