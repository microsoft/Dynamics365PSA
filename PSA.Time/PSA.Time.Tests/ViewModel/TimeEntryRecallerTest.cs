using Common.Test.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Samples;
using PSA.Time.ViewModel;
using System.Threading.Tasks;

namespace PSA.Time.Tests.ViewModel
{
    [TestClass]
    public class TimeEntryRecallerTest
    {
        /// <summary>
        /// Verify the name of the Notes parameter when recalling time entries.
        /// </summary>
        [TestMethod]
        public async Task RecallNoteParameterName()
        {
            TimeEntryRecaller recaller = new TimeEntryRecaller();
            DataAccessTestable da = new DataAccessTestable();

            recaller.DataAccess = da;

            bool parameterFound = false;
            da.addExecuteHandler((OrganizationRequest or) =>
            {
                parameterFound = or.Parameters.Contains("Notes");
            });

            await recaller.ExecuteAction();

            Assert.IsTrue(parameterFound);
        }
    }
}
