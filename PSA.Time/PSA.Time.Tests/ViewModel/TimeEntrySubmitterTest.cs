using Common.Model;
using Common.Test.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Samples;
using PSA.Time.ViewModel;
using System;
using Tasks = System.Threading.Tasks;

namespace PSA.Time.Tests.ViewModel
{
    [TestClass]
    public class TimeEntrySubmitterTest
    {
        /// <summary>
        /// Validate that submitted time entries have msdyn_entryStatus updated to Submitted.
        /// </summary>
        /// <returns>Task to support async.</returns>
        [TestMethod]
        public async Tasks.Task SubmitMarksEntriesAsSubmitted()
        {
            DataAccessTestable dat = new DataAccessTestable();
            TimeEntrySubmitter submitter = new TimeEntrySubmitter();
            submitter.DataAccess = dat;

            msdyn_timeentry entry = new msdyn_timeentry();
            submitter.addTimeEntry(entry);

            dat.addExecuteHandler((OrganizationRequest or) => 
            {
                entry.msdyn_entryStatus = new OptionSetValue((int)msdyn_timeentry_msdyn_entrystatus.Submitted);
            });

            await submitter.ExecuteAction();

            Assert.AreEqual((int)msdyn_timeentry_msdyn_entrystatus.Submitted, entry.msdyn_entryStatus.Value);
        }

        /// <summary>
        /// Validate that multiple time entries have their IDs concatenated on submission.
        /// </summary>
        /// <returns>Task to support async.</returns>
        [TestMethod]
        public async Tasks.Task TimeEntryIDsAreConcatenatedForSubmit()
        {
            string guid1 = "BF65EFF9-89AF-4961-9C73-C178071A06AE";
            string guid2 = "B42423C5-F993-410B-95FC-B3085A91DE4B";

            msdyn_timeentry entry1 = new msdyn_timeentry();
            entry1.Id = Guid.Parse(guid1);
            msdyn_timeentry entry2 = new msdyn_timeentry();
            entry2.Id = Guid.Parse(guid2);

            TimeEntrySubmitter submitter = new TimeEntrySubmitter();
            submitter.addTimeEntry(entry1);
            submitter.addTimeEntry(entry2);

            DataAccessTestable dat = new DataAccessTestable();
            submitter.DataAccess = dat;

            string ids = "";
            dat.addExecuteHandler((OrganizationRequest or) =>
            {
                ids = (string)or.Parameters[TimeEntrySubmitter.TimeEntryIdsParameterName];
            });

            await submitter.ExecuteAction();

            // Validate
            Assert.AreEqual(string.Format("{0},{1}", entry1.Id, entry2.Id), ids);
        }
    }
}
