using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Model.Tests
{
    [TestClass]
    public class msdyn_timeentryTest
    {
        /// <summary>
        /// Verify that getter for EntryStatus returns the correct enum value.
        /// </summary>
        [TestMethod]
        public void EntryStatusGetTest()
        {
            msdyn_timeentry timeEntry = new msdyn_timeentry();

            // Check msdyn_timeentry_msdyn_entrystatus.cs for values
            //Approved = 192350002,
            //Returned = 192350001,
            //Draft = 192350000,
            //Submitted = 192350003,
            timeEntry.msdyn_entryStatus = new Microsoft.Xrm.Sdk.Samples.OptionSetValue(192350000);

            Assert.AreEqual(msdyn_timeentry_msdyn_entrystatus.Draft, timeEntry.EntryStatus);
        }

        /// <summary>
        /// Verify that setter for EntryStatus puts the correct value in msdyn_timeentry.msdyn_entryStatus
        /// </summary>
        [TestMethod]
        public void EntryStatusSetTest()
        {
            // Change a Submitted to Returned and validate

            // Create a submitted entry
            msdyn_timeentry timeEntry = new msdyn_timeentry();
            //Submitted = 192350003,
            timeEntry.msdyn_entryStatus = new Microsoft.Xrm.Sdk.Samples.OptionSetValue(192350003);

            // Change to Returned
            //Returned = 192350001,
            timeEntry.EntryStatus = msdyn_timeentry_msdyn_entrystatus.Returned;

            // Validate 
            //Returned = 192350001,
            Assert.AreEqual(new Microsoft.Xrm.Sdk.Samples.OptionSetValue(192350001).Value, timeEntry.msdyn_entryStatus.Value);
        }


        /// <summary>
        /// Validate that we can set to null.
        /// </summary>
        [TestMethod]
        public void SetNullEntryStatus()
        {
            msdyn_timeentry timeEntry = new msdyn_timeentry();
            //Submitted = 192350003,
            timeEntry.msdyn_entryStatus = new Microsoft.Xrm.Sdk.Samples.OptionSetValue(192350003);

            // Change to Null
            timeEntry.EntryStatus = null;

            // Validate 
            Assert.IsNull(timeEntry.EntryStatus);
        }

        /// <summary>
        /// Validate that we can get a null status.
        /// </summary>
        [TestMethod]
        public void GetNullEntryStatus()
        {
            msdyn_timeentry timeEntry = new msdyn_timeentry();
            // Value is null by default

            Assert.IsNull(timeEntry.EntryStatus);
        }

        /// <summary>
        /// Verify that a time entry view model can be marked as "Selected".
        /// </summary>
        [TestMethod]
        public void CanMarkTimeEntryAsSelected()
        {
            msdyn_timeentry timeEntry = new msdyn_timeentry();

            timeEntry.Selected = true;
            Assert.IsTrue(timeEntry.Selected);
        }

        /// <summary>
        /// Verify that time entry view model are not selected by default.
        /// </summary>
        [TestMethod]
        public void TimeEntryNotSelectedByDefault()
        {
            msdyn_timeentry timeEntry = new msdyn_timeentry();
            Assert.IsFalse(timeEntry.Selected);
        }

    }
}
