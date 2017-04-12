using Common.Model;
using Common.Test.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Samples;
using PSA.Time.Model;
using PSA.Time.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Tasks = System.Threading.Tasks;

namespace PSA.Time.Tests
{
    [TestClass]
    public class TimeCollectionViewModelTest
    {
        /// <summary>
        /// Initial values on the TimeCollectionViewModel
        /// </summary>
        [TestMethod]
        public void CreateTimeCollectionViewModel()
        {
            TimeCollectionViewModel viewModel = new TimeCollectionViewModel();
            Assert.IsNotNull(viewModel);

            // initial values
            Assert.AreEqual("My Time", viewModel.Title, "Incorrect value for title.");

            // viewModel has never been updated.
            Assert.AreEqual(DateTime.MinValue, viewModel.LastUpdatedDateTime);
        }

        /// <summary>
        /// Can add time entries for multiple days.
        /// </summary>
        [TestMethod]
        public void LoadTimesFromMultipleDays()
        {
            DateTime dateTime1 = new DateTime(2015, 7, 28, 9, 58, 0);
            DateTime dateTime2 = new DateTime(2014, 1, 22, 23, 19, 0);

            msdyn_timeentry timeEntry1 = new msdyn_timeentry();
            timeEntry1.msdyn_timeentryId = new Guid();
            timeEntry1.msdyn_description = "entry 1";
            timeEntry1.msdyn_date = dateTime1;
            timeEntry1.msdyn_duration = 2;

            msdyn_timeentry timeEntry2 = new msdyn_timeentry();
            timeEntry2.msdyn_timeentryId = new Guid();
            timeEntry2.msdyn_description = "entry 2";
            timeEntry2.msdyn_date = dateTime2;
            timeEntry2.msdyn_duration = 2;

            TimeCollectionViewModel viewModel = new TimeCollectionViewModel();
            viewModel.AddTimeEntries(new msdyn_timeentry[] { timeEntry1, timeEntry2 });

            // Validate
            IEnumerable<TimeCollection> days = viewModel.Days;
            Assert.AreEqual(2, days.Count(), "Incorrect number of days");
        }

        /// <summary>
        /// Clear method empties the collection.
        /// </summary>
        [TestMethod]
        public void ClearRemovesAllDays()
        {
            DateTime dateTime1 = new DateTime(2015, 7, 28, 9, 58, 0);
            DateTime dateTime2 = new DateTime(2014, 1, 22, 23, 19, 0);

            msdyn_timeentry timeEntry1 = new msdyn_timeentry();
            timeEntry1.msdyn_timeentryId = new Guid();
            timeEntry1.msdyn_description = "entry 1";
            timeEntry1.msdyn_date = dateTime1;
            timeEntry1.msdyn_duration = 2;

            msdyn_timeentry timeEntry2 = new msdyn_timeentry();
            timeEntry2.msdyn_timeentryId = new Guid();
            timeEntry2.msdyn_description = "entry 2";
            timeEntry2.msdyn_date = dateTime2;
            timeEntry2.msdyn_duration = 2;

            TimeCollectionViewModel viewModel = new TimeCollectionViewModel();
            viewModel.AddTimeEntries(new msdyn_timeentry[] { timeEntry1, timeEntry2 });

            viewModel.ClearTimes();
            Assert.AreEqual(0, viewModel.Days.Count);
        }

        /// <summary>
        /// Test that the default date is from the view model's current month and not always today's date.
        /// </summary>
        [TestMethod]
        public async Tasks.Task DefaultDateFromCurrentFilter()
        {
            msdyn_timeentry timeEntry = new msdyn_timeentry();
            // Today - the day of the week (gives sunday) + 7 = Sunday next week.
            DateTime expectedDate = DateTime.Today - TimeSpan.FromDays((int)DateTime.Today.DayOfWeek) + TimeSpan.FromDays(7);

            // Act.
            TimeCollectionViewModel viewModel = new TimeCollectionViewModel();
            viewModel.DataAccess = new DataAccessTestable();
            await viewModel.IncrementDateFilter();

            // Assert.
            viewModel.DefaultDateFromCurrentFilter(timeEntry);          
        }

        /// <summary>
        /// Validate that RetrieveEntities is called on the data access class of the view model
        /// when the date filter is incremented.
        /// </summary>
        /// <returns>Async task.</returns>
        [TestMethod]
        public async Tasks.Task ChangingFilterRefreshesList()
        {
            DataAccessTestable dataAccessTestable = new DataAccessTestable();

            TimeCollectionViewModel viewModel = new TimeCollectionViewModel();
            viewModel.DataAccess = dataAccessTestable;

            // Load
            await viewModel.LoadTimes();

            // Subscribe to update
            bool retrieved = false;
            dataAccessTestable.addRetrieveEntitiesHandler((q, e, b) =>
            {
                retrieved = true;
            });

            // change filter
            await viewModel.IncrementDateFilter();

            // Verify updated
            Assert.IsTrue(retrieved);
        }

        /// <summary>
        /// Verify that HasItems is false before attempting to load any data.
        /// </summary>
        [TestMethod]
        public void HasItemsFalseInitially()
        {
            TimeCollectionViewModel vm = new TimeCollectionViewModel();
            
            Assert.IsFalse(vm.HasItems);
        }

        /// <summary>
        /// Verify that HasItems is false after attempting to load data but getting 0 results.
        /// </summary>
        [TestMethod]
        public async Tasks.Task HasItemsFalseWhenNoEntriesExist()
        {
            TimeCollectionViewModel vm = new TimeCollectionViewModel();
            vm.DataAccess = new DataAccessTestable();

            // DataAccessTestable gives back an empty list of entities by default, no need to add any overrides here.
            await vm.LoadTimes();

            Assert.IsFalse(vm.HasItems);
        }

        /// <summary>
        /// Verify that HasItem is true after loading a time entry.
        /// </summary>
        [TestMethod]
        public async Tasks.Task HasItemsTrueWhenEntriesFound()
        {
            DataAccessTestable da = new DataAccessTestable();
            
            TimeCollectionViewModel vm = new TimeCollectionViewModel();
            vm.DataAccess = da;

            List<msdyn_timeentry> timeEntries = new List<msdyn_timeentry>();
            timeEntries.Add(new msdyn_timeentry()
            {
                msdyn_date = DateTime.Today,
                msdyn_duration = 4
            });

            da.setRetrieveEntitiesResult(from entry in timeEntries select (Entity)entry);

            await vm.LoadTimes();

            Assert.IsTrue(vm.HasItems);
        }

        [TestMethod]
        public void MultiselectModeDefaultsFalse()
        {
            TimeCollectionViewModel vm = new TimeCollectionViewModel();
            Assert.IsFalse(vm.MultiselectModeEnabled);
        }
    }
}
