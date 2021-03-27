﻿using MindSphereSdk.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MindSphereSdk.Common
{
    /// <summary>
    /// General SDK client
    /// </summary>
    public abstract class SdkClient
    {
        private IMindSphereConnector _mindSphereConnector;

        public SdkClient(ICredentials credentials, HttpClient httpClient)
        {
            _mindSphereConnector = CreateConnector(credentials, httpClient);
        }

        /// <summary>
        /// Create specified MindSphere connector based on provided credentials
        /// </summary>
        private IMindSphereConnector CreateConnector(ICredentials credentials, HttpClient httpClient)
        {
            if (credentials is AppCredentials)
            {
                return new BasicConnector((AppCredentials) credentials, httpClient);
            }
            else
            {
                throw new InvalidOperationException("Invalid credentials");
            }
        }

        /// <summary>
        /// Sending HTTP request to the MindSphere API
        /// </summary>
        protected async Task<string> HttpActionAsync(HttpMethod method, string specUri, HttpContent body = null)
        {
            string response = await _mindSphereConnector.HttpActionAsync(method, specUri, body);
            return response;
        }
    }
}
