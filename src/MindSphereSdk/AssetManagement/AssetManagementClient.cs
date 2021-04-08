﻿using MindSphereSdk.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MindSphereSdk.AssetManagement
{
    /// <summary>
    /// Configuring, reading and managing assets, asset types and aspect types
    /// </summary>
    public class AssetManagementClient : SdkClient
    {
        private readonly string _baseUri = "/api/assetmanagement/v3";

        public AssetManagementClient(ICredentials credentials, HttpClient httpClient) : base(credentials, httpClient)
        {

        }

        /// <summary>
        /// List all available assets
        /// </summary>
        public async Task<IEnumerable<Asset>> ListAssetsAsync(ListAssetsRequest request = null)
        {
            // prepare query string
            string queryString = "?";
            queryString += request.Size != null ? $"size={request.Size}&" : "";
            queryString += request.Page != null ? $"page={request.Page}&" : "";
            queryString += request.Sort != null ? $"sort={request.Sort}&" : "";
            queryString += request.Filter != null ? $"filter={request.Filter}&" : "";

            string uri = _baseUri + "/assets" + queryString;

            string response = await HttpActionAsync(HttpMethod.Get, uri);
            var assetListWrapper = JsonConvert.DeserializeObject<MindSphereResourceWrapper<EmbeddedAssetList>>(response);
            var assetList = assetListWrapper.Embedded.Assets;

            return assetList;
        }

        /// <summary>
        /// Create an asset
        /// </summary>
        public async Task<Asset> AddAssetsAsync(AddAssetRequest request)
        {
            string uri = _baseUri + "/assets";
            StringContent body = new StringContent(JsonConvert.SerializeObject(request.Body), Encoding.UTF8, "application/json");

            string response = await HttpActionAsync(HttpMethod.Post, uri, body);
            var asset = JsonConvert.DeserializeObject<Asset>(response);

            return asset;
        }

        /// <summary>
        /// Read a single asset 
        /// </summary>
        public async Task<Asset> GetAssetAsync(GetAssetRequest request)
        {
            string uri = _baseUri + "/assets/" + request.Id;

            string response = await HttpActionAsync(HttpMethod.Get, uri);
            var asset = JsonConvert.DeserializeObject<Asset>(response);

            return asset;
        }

        /// <summary>
        /// Update an asset
        /// </summary>
        public async Task<Asset> UpdateAssetAsync(UpdateAssetRequest request)
        {
            string uri = _baseUri + "/assets/" + request.Id;
            // prepare HTTP request body
            string jsonString = JsonConvert.SerializeObject(request.Body, 
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            StringContent body = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // prepare HTTP request headers
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("If-Match", request.IfMatch));

            string response = await HttpActionAsync(HttpMethod.Put, uri, body, headers);
            var asset = JsonConvert.DeserializeObject<Asset>(response);
            
            return asset;
        }

        /// <summary>
        /// Patch an asset
        /// </summary>
        public async Task<Asset> PatchAssetAsync(UpdateAssetRequest request)
        {
            string uri = _baseUri + "/assets/" + request.Id;
            // prepare HTTP request body
            string jsonString = JsonConvert.SerializeObject(request.Body,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            StringContent body = new StringContent(jsonString, Encoding.UTF8, "application/merge-patch+json");

            // prepare HTTP request headers
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("If-Match", request.IfMatch));

            string response = await HttpActionAsync(new HttpMethod("PATCH"), uri, body, headers);
            var asset = JsonConvert.DeserializeObject<Asset>(response);

            return asset;
        }

        /// <summary>
        /// Delete an asset 
        /// </summary>
        public async Task DeleteAsync(DeleteAssetRequest request)
        {
            string uri = _baseUri + "/assets/" + request.Id;

            // prepare HTTP request headers
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("If-Match", request.IfMatch));

            await HttpActionAsync(HttpMethod.Delete, uri, headers: headers);
        }
    }
}
