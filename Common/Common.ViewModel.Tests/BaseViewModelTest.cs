using Common.Utilities.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.ViewModel.Tests
{
    [TestClass]
    public class BaseViewModelTest
    {
        /// <summary>
        /// Verify that BaseViewModel automatically creates a DataAccess instance
        /// when you access the DataAccess property.
        /// </summary>
        [TestMethod]
        public void DefaultDataAccessClassType()
        {
            BaseViewModel vm = new BaseViewModelTestable();

            Assert.AreEqual(typeof(DataAccess), vm.DataAccess.GetType());
        }

        /// <summary>
        /// Verify that when the title property is changed, a PropertyChanged event is raised.
        /// </summary>
        [TestMethod]
        public void UpdateTitleRaisesPropertyChanged()
        {
            BaseViewModel vm = new BaseViewModelTestable();

            bool propertyChangeHandled = false;
            vm.PropertyChanged += (s, e) => { propertyChangeHandled = true; };

            vm.Title = "somevalue";

            Assert.IsTrue(propertyChangeHandled);
        }

        /// <summary>
        /// This class only exists so I can create an instance of BaseViewModel.
        /// </summary>
        private class BaseViewModelTestable : BaseViewModel
        {
        }
    }
}
