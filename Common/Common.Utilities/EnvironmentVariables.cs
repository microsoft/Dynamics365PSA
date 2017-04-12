using System;

namespace Common.Utilities
{
    /// <summary>
    /// Provides variables specific for each app. The app needs to set these variables when launching
    /// </summary>
    public class EnvironmentVariables
    {
        public string AuthenticationClientId { get; protected set; }
        public string AuthenticationRedirectUri { get; protected set; }

        private static EnvironmentVariables instance;
        public static EnvironmentVariables Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("Please call Initialize() to initialize EnvironmentVariables");
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        protected EnvironmentVariables() { }

        public static void Initialize(string authenticationClientId, string authenticationRedirectUri)
        {
            instance = new EnvironmentVariables();
            instance.AuthenticationClientId = authenticationClientId;
            instance.AuthenticationRedirectUri = authenticationRedirectUri;
        }
    }
}
