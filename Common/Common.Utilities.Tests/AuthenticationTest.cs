using Common.Test.Tools;
using Common.Utilities.Authentication;
using Common.Utilities.DataAccess;
using Common.Utilities.DataAccess.ServiceAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Common.Utilities.Tests
{
    [TestClass]
    public class AuthenticationTest
    {
        private DeviceDataAccessTestable dataAccess;

        [TestInitialize]
        public void TestInit()
        {
            EnvironmentVariables.Initialize(AuthenticationTestable.AuthenticationClientId, AuthenticationTestable.AuthenticationRedirectUri);
            dataAccess = DeviceDataAccess.Initialize<DeviceDataAccessTestable>();
        }

        /// <summary>
        /// When the use logs out, the CRM server URL should not be cleared.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task LogOutDoesNotForgetCRMServerURL()
        {
            AuthenticationTestable auth = new AuthenticationTestable();
            string url = "crmserverurl";

            OrganizationServiceProxy.Current.ServiceUrl = url;
            
            //auth.ServiceUrl = url;
            await auth.LogOut();

            Assert.AreEqual(url, auth.ServiceUrl);
        }

        /// <summary>
        /// Verify that Authentication.Current returns an instance that is ready.
        /// </summary>
        /// <returns>Task for async support.</returns>
        [TestMethod]
        public async Task AccessingCurrentAuthenticationReturnsInitializedInstance()
        {
            string serviceURL = "testServiceURL";
            string oAuthURL = "testOAuthURL";

            // Set fake data in storage
            await DeviceDataAccess.Current.WriteToLocal(Authentication.Authentication.ServiceUrlKey + ".dat", serviceURL);
            await DeviceDataAccess.Current.WriteToLocal(Authentication.Authentication.OAuthUrlKey + ".dat", oAuthURL);
         
            // Set a fake delay in the data access
            dataAccess.Delay = 10;

            AppUtilitiesTestable utils = new AppUtilitiesTestable();
            // In an earlier version of the code, the AppUtilities.Initialize was not awaitable, 
            // so you could attempt to access Authentication before it was ready.
            await utils.Initialize();

            Assert.IsFalse(Authentication.Authentication.Current.IsAuthenticationInfoMissing());
        }

        [TestMethod]
        public void TestUrlValidation()
        {
            ValidateCrmUrlTest("https://contoso.crm.dynamics.com");
            ValidateCrmUrlTest("https://Contoso.crm.dynamics.com");
            ValidateCrmUrlTest("https://contoso.crm1.dynamics.com");
            ValidateCrmUrlTest("https://Contoso.crm10.dynamics.com");
            ValidateCrmUrlTest("https://whatever-crm-psa.crm.dynamics.com");
            ValidateCrmUrlTest("https://whatever-CRM-psA.crm.dynamics.com");
            ValidateCrmUrlTest("https://Whatever-CRM.crm.dynamics.com");
            ValidateCrmUrlTest("https://Whatever-CRM-psA.crm2.dynamics.com");
            ValidateCrmUrlTest("https://Whatever-CRM-psA.crm20.dynamics.com");

            ValidateCrmUrlTest("https://contoso.crm10.crmlivetie.com");

            ValidateCrmUrlTest("https://whatever-CRM-psA.crmattacker.dynamics.com", false);
            ValidateCrmUrlTest("https://contoso.crm.dynamacs.com", false);

            ValidateCrmUrlTest("https://contoso.crm.dynamics.org", false);
            ValidateCrmUrlTest("https://contoso.crm.dynamacs.com", false);
            ValidateCrmUrlTest("https://c.crm.dynamacs.com", false);

            ValidateCrmUrlTest("http://contoso.crm.dynamics.com", false);

            ValidateCrmUrlTest("https://contoso.crm100.dynamics.com", false);

            ValidateCrmUrlTest("https://contoso.crma.dynamics.com", false);

            ValidateCrmUrlTest("https://contoso.attacker.crm.dynamics.com", false);

            ValidateCrmUrlTest("https://contoso_.crm.dynamics.com", false);
            ValidateCrmUrlTest("https://contoso^.crm.dynamics.com", false);
            ValidateCrmUrlTest("https://cont'oso.crm.dynamics.com", false);
            ValidateCrmUrlTest("https://cont[]oso.crm.dynamics.com", false);
            ValidateCrmUrlTest("https://conto\\so.crma.dynamics.com", false);

            //Should fail the test (testing the test)
            //Test("http://contoso.crm100.dynamics.com");
            //Test("https://Contoso.crm.dynamics.com", false);
            //Test("asdf");
            //Test("");
        }

        static void ValidateCrmUrlTest(string toMatch, bool validExpected = true)
        {
            Assert.AreEqual(CrmUrlVerification.ValidateCrmUrl(toMatch), validExpected, validExpected ? "Url should be valid!" : "Url should be invalid!");
        }
    }
}
