﻿using MindSphereSdk.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MindSphereSdk.Core.Common
{
    /// <summary>
    /// Connector to the MindSphere API
    /// </summary>
    internal abstract class MindSphereConnector
    {
        protected string _accessToken;
        private readonly ClientConfiguration _configuration;
        protected readonly HttpClient _httpClient;

        public MindSphereConnector(ClientConfiguration configuration)
        {
            _configuration = configuration;

            var handler = new HttpClientHandler();
            // proxy setting
            if (!string.IsNullOrWhiteSpace(_configuration.Proxy))
            {
                handler.Proxy = new WebProxy(_configuration.Proxy, false);
                handler.UseProxy = true;
            }
            _httpClient = new HttpClient(handler)
            {
                // timeout setting
                Timeout = _configuration.Timeout
            };
        }

        /// <summary>
        /// Sending HTTP request to the MindSphere API
        /// </summary>
        public async Task<string> HttpActionAsync(HttpMethod method, string specUri, HttpContent body = null, List<KeyValuePair<string, string>> headers = null)
        {
            // always try to validate / renew token
            await RenewTokenAsync();

            // prepare HTTP request
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = GetFullUri(specUri)
            };
            request.Headers.Add("Authorization", "Bearer " + _accessToken);

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
        protected abstract Task AcquireTokenAsync();

        /// <summary>
        /// Renew MindSphere access token
        /// </summary>
        private async Task RenewTokenAsync()
        {
            // if token exists
            if (_accessToken != null)
            {
                // if token is invalid
                if (!ValidateToken())
                {
                    // remove token
                    _accessToken = null;
                }
            }

            // if token does not exist
            if (_accessToken == null)
            {
                // get new token
                await AcquireTokenAsync();
                // if token is invalid
                if (!ValidateToken())
                {
                    throw new InvalidOperationException("Error in aquiering new token");
                }
            }
        }

        /// <summary>
        /// Validate MindSphere access token 
        /// </summary>
        private bool ValidateToken()
        {
            if (_accessToken == null) return false;

            double minutesSkew = 5.0;
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(_accessToken);

            string expString = token.Claims.First(claim => claim.Type == "exp").Value;
            DateTime exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expString)).UtcDateTime;
            // if exp is in the past (with minutes skew)
            if (DateTime.UtcNow.AddMinutes(minutesSkew) >= exp) return false;

            string iatString = token.Claims.First(claim => claim.Type == "iat").Value;
            DateTime iat = DateTimeOffset.FromUnixTimeSeconds(long.Parse(iatString)).UtcDateTime;
            // if iat is in the future (with minutes skew)
            if (DateTime.UtcNow.AddMinutes(minutesSkew) <= iat) return false;

            // check signiture algo
            if (token.SignatureAlgorithm != "RS256") return false;

            return true;
        }

        /// <summary>
        /// Generate full URI
        /// </summary>
        protected Uri GetFullUri(string specUri)
        {
            string basePart = $"https://gateway.{_configuration.Region}.{_configuration.Domain}";
            return new Uri(basePart + specUri);
        }
    }
}
