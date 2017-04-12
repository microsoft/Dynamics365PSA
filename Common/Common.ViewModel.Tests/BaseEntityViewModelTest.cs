using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.ViewModel.Tests
{
    [TestClass]
    public class BaseEntityViewModelTest
    {
        /// <summary>
        /// Validate that BaseEntityViewModel.Initialize returns a valid task.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InitializeReturnsValidTask()
        {
            BaseEntityViewModel baseEntityViewModel = new BaseEntityViewModelTestable();
            // This line just needs to run without throwing an error. 
            // baseEntityViewModel.Initialize used to return null and the line
            // would cause an exception.
            await baseEntityViewModel.Initialize();
        }

        /// <summary>
        /// Validate that there is no save pending for new objects.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task NothingToSaveAfterInitialize()
        {
            BaseEntityViewModel baseEntityViewModel = new BaseEntityViewModelTestable();
            await baseEntityViewModel.Initialize();

            Assert.IsFalse(baseEntityViewModel.HasPendingDataToSave);
        }
      
    }

    internal class BaseEntityViewModelTestable : BaseEntityViewModel
    {
        public override Task<bool> ForcedSave()
        {
            this.HasPendingDataToSave = false;
            return Task.FromResult(true);
        }

        public override Task LoadReferenceData()
        {
            this.HasPendingDataToSave = false;
            return Task.FromResult(true);
        }
    }
}
