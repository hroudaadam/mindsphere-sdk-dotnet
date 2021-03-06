using MindSphereSdk.Core.Authentication;
using System.Threading.Tasks;

namespace MindSphereSdk.Core.Common
{
    /// <summary>
    /// Connector to the MindSphere API using user credentials
    /// </summary>
    internal class UserMindSphereConnector : MindSphereConnector
    {
        private readonly UserCredentials _credentials;

        /// <summary>
        /// Create a new instance of UserMindSphereConnector
        /// </summary>
        public UserMindSphereConnector(UserCredentials credentials, ClientConfiguration configuration)
            : base(configuration)
        {
            _credentials = credentials;
        }

        /// <summary>
        /// Acquire MindSphere access token (no need - it is handled by MindSphere)
        /// </summary>
        protected override Task AcquireTokenAsync()
        {
            _accessToken = _credentials.Token;
            return Task.CompletedTask;
        }
    }
}
