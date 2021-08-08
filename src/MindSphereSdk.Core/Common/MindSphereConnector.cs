﻿using MindSphereSdk.Core.Authentication;
using MindSphereSdk.Core.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MindSphereSdk.Core.Common
{
    /// <summary>
    /// Connector to the MindSphere API
    /// </summary>
    public abstract class MindSphereConnector
    {
        protected HttpClient _httpClient;

        protected AccessToken _accessToken;

        private string _region = "eu1";
        private string _domain = "mindsphere.io";

        public MindSphereConnector(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Sending HTTP request to the MindSphere API
        /// </summary>
        public async Task<string> HttpActionAsync(HttpMethod method, string specUri, HttpContent body = null, List<KeyValuePair<string, string>> headers = null)
        {
            // always try to validate / renew token
            await RenewTokenAsync();

            // prepare HTTP request
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = method;
            request.RequestUri = GetFullUri(specUri);
            request.Headers.Add("Authorization", "Bearer " + _accessToken.Token);

            // headers from parametr
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            request.Content = body;

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            await MindSphereApiExceptionHandler.HandleUnsuccessfulResponseAsync(response);

            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        /// <summary>
        /// Acquire MindSphere access token
        /// </summary>
        public abstract Task AcquireTokenAsync();

        /// <summary>
        /// Renew MindSphere access token
        /// </summary>
        public async Task RenewTokenAsync()
        {
            if (_accessToken != null)
            {
                bool tokenValid = ValidateToken();
                if (!tokenValid)
                {
                    _accessToken = null;
                }
            }

            if (_accessToken == null)
            {
                await AcquireTokenAsync();
                bool tokenValid = ValidateToken();
                if (!tokenValid)
                {
                    throw new InvalidOperationException("Error in aquiering new token");
                }
            }
        }

        // TODO: implement token validation (https://developer.mindsphere.io/concepts/concept-authentication.html#token-validation)
        /// <summary>
        /// Validate MindSphere access token 
        /// </summary>
        public bool ValidateToken()
        {
            if (_accessToken == null) return false;

            double minutesSkew = 5.0;
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(_accessToken.Token);

            string expString = token.Claims.First(claim => claim.Type == "exp").Value;
            DateTime exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expString)).LocalDateTime;
            // if exp is in the past (with minutes skew)
            if (DateTime.Now.AddMinutes(minutesSkew) >= exp) return false;

            string iatString = token.Claims.First(claim => claim.Type == "iat").Value;
            DateTime iat = DateTimeOffset.FromUnixTimeSeconds(long.Parse(iatString)).LocalDateTime;
            // if iat is in the future (with minutes skew)
            if (DateTime.Now.AddMinutes(minutesSkew) <= iat) return false;

            return true;
        }

        /// <summary>
        /// Generate full URI
        /// </summary>
        protected Uri GetFullUri(string specUri)
        {
            string basePart = $"https://gateway.{_region}.{_domain}";
            return new Uri(basePart + specUri);
        }
    }
}