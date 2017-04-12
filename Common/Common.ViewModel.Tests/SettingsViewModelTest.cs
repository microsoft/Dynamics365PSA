using Common.Test.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Common.ViewModel.Tests
{
    [TestClass]
    public class SettingsViewModelTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Utilities.EnvironmentVariables.Initialize(AuthenticationTestable.AuthenticationClientId,
                AuthenticationTestable.AuthenticationRedirectUri);
        }

        /// <summary>
        /// When Https:// isn't present on the url entered by the user, it is appended.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AppendHttpsToServerUrl()
        {
            var auth = new AuthenticationTestable();
            SettingsViewModel vm = new SettingsViewModel(auth, null);

            string testUrl = "test.crm.dynamics.com";
            vm.ServiceUrl = testUrl;

            await vm.LogIn();
            
            Assert.AreEqual("https://test.crm.dynamics.com", auth.ActualLogInServiceUrl);
        }

        /// <summary>
        /// if user enters a URL with http:// is is changed to https://
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ChangeHttpToHttpsForServerUrl()
        {
            var auth = new AuthenticationTestable();
            SettingsViewModel vm = new SettingsViewModel(auth, null);

            string testUrl = "http://test.crm.dynamics.com";
            vm.ServiceUrl = testUrl;

            await vm.LogIn();

            Assert.AreEqual("https://test.crm.dynamics.com", auth.ActualLogInServiceUrl);
        }

        /// <summary>
        /// Validate that the correct DNS names are appended when there isn't a match.
        /// </summary>
        /// <returns>Task to support async.</returns>
        [TestMethod]
        public async Task AppendDynamicsComToServerUrl()
        {
            var auth = new AuthenticationTestable();
            SettingsViewModel vm = new SettingsViewModel(auth, null);

            string testUrl = "https://test";
            vm.ServiceUrl = testUrl;

            await vm.LogIn();

            Assert.AreEqual("https://test.dynamics.com", auth.ActualLogInServiceUrl);
        }

        /// <summary>
        /// Verify that the service URL is defaulted when setting the Authentication property.
        /// </summary>
        [TestMethod]
        public void ServiceUrlDefaultedFromAuthentication()
        {
            const string TESTURL = "MYTESTURL";
            
            SettingsViewModel vm = new SettingsViewModel(new AuthenticationTestable(), null);
            Assert.IsTrue(string.IsNullOrEmpty(vm.ServiceUrl), "Expect empty URL by default.");

            // Test Auth
            AuthenticationTestable auth = new AuthenticationTestable(TESTURL);

            vm = new SettingsViewModel(auth, null);
            Assert.AreEqual(TESTURL, vm.ServiceUrl, "Unexpected value for SettingsViewModel.ServiceUrl.");
        }

        /// <summary>
        /// Verifies that the correct values are added to the server url.
        /// </summary>
        /// <returns>Task to support async.</returns>
        [TestMethod]
        public async Task AppendAndPrependToServerUrl()
        {
            var auth = new AuthenticationTestable();
            SettingsViewModel vm = new SettingsViewModel(auth, null);

            string testUrl = "psatime.crm";
            vm.ServiceUrl = testUrl;

            await vm.LogIn();

            Assert.AreEqual("https://psatime.crm.dynamics.com", auth.ActualLogInServiceUrl);
        }

        [TestMethod]
        public async Task DoNotAppendServerUrlWhenDynamicsComIsPresent()
        {
            var auth = new AuthenticationTestable();
            SettingsViewModel vm = new SettingsViewModel(auth, null);

            string testUrl = "https://promxtestmicrosoft.crm4.dynamics.com";
            vm.ServiceUrl = testUrl;

            await vm.LogIn();

            Assert.AreEqual("https://promxtestmicrosoft.crm4.dynamics.com", auth.ActualLogInServiceUrl);
        }

        [TestMethod]
        public async Task DoNotAppendAppendServerUrlWhenCrmlivetieComIsPresent()
        {
            var auth = new AuthenticationTestable();
            SettingsViewModel vm = new SettingsViewModel(auth, null);

            string testUrl = "https://promxtestmicrosoft.crmlivetie.com";
            vm.ServiceUrl = testUrl;

            await vm.LogIn();

            Assert.AreEqual("https://promxtestmicrosoft.crmlivetie.com", auth.ActualLogInServiceUrl);
        }

        [TestMethod]
        public void GenerateSuggestedServiceUrlTest()
        {
            var auth = new AuthenticationTestable();
            SettingsViewModel vm = new SettingsViewModel(auth, null);

            vm.ServiceUrl = null;
            Assert.IsNull(vm.SuggestedServiceUrl);

            vm.ServiceUrl = "p";
            Assert.AreEqual("p.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml";
            Assert.AreEqual("psaml.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psa";
            Assert.AreEqual("psa.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.c";
            Assert.AreEqual("psaml.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm";
            Assert.AreEqual("psaml.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm.";
            Assert.AreEqual("psaml.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm.a";
            Assert.AreEqual("psaml.crm.a.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm.a ";
            Assert.AreEqual("psaml.crm.a.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "   psaml.crm.a";
            Assert.AreEqual("psaml.crm.a.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm.c";
            Assert.AreEqual("psaml.crm.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm.c   ";
            Assert.AreEqual("psaml.crm.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = " psaml.crm.c";
            Assert.AreEqual("psaml.crm.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm.d";
            Assert.AreEqual("psaml.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm.dynamics.com";
            Assert.IsNull(vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm.dynamics.com ";
            Assert.IsNull(vm.SuggestedServiceUrl);

            vm.ServiceUrl = "   psaml.crm.dynamics.com";
            Assert.IsNull(vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm.dynamics.co";
            Assert.AreEqual("psaml.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm.dynamics.co   ";
            Assert.AreEqual("psaml.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = " psaml.crm.dynamics.co";
            Assert.AreEqual("psaml.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm4";
            Assert.AreEqual("psaml.crm4.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm4.";
            Assert.AreEqual("psaml.crm4.crm.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm4.d";
            Assert.AreEqual("psaml.crm4.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.crm4.d";
            Assert.AreEqual("psaml.crm4.dynamics.com", vm.SuggestedServiceUrl);

            vm.ServiceUrl = "psaml.abc.crml";
            Assert.AreEqual("psaml.abc.crmlivetie.com", vm.SuggestedServiceUrl);
        }
    }
}
